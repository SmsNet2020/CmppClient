using Cmpp.Client.Codecs;
using Cmpp.Client.Handler.Frame;
using Cmpp.Client.Handler.Sms;
using DotNetty.Codecs;
using DotNetty.Common.Concurrency;
using DotNetty.Common.Internal.Logging;
using DotNetty.Common.Utilities;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Polly;
using Sms.Common;
using Sms.Protocol.Cmpp2;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cmpp.Client
{
	public class CmppClient : IClient
	{
		SqliteContext dbContext;
		SubmitSmsMatchPool<MsgEx> matchQueue;
		IEventExecutorGroup eventExecutorGroup;
		Bootstrap bootstrap = new Bootstrap();
		MultithreadEventLoopGroup group;
		BaseCmppSmsHandler smsHandler;
		ClientLoggerFactory loggerFactory;
		ClientLogger<CmppClient> logger;
		CancellationTokenSource tokenSource = new CancellationTokenSource();
		HashedWheelTimer timer = new HashedWheelTimer();
		SessionMap sessionMap = new SessionMap();
		ConcurrentQueue<Sms.Common.Sms> smsQueue = new ConcurrentQueue<Sms.Common.Sms>();

		public CmppClientConfig Config { get; set; }

		public CmppClient(
			CmppClientConfig config,
			BaseCmppSmsHandler smsHandler,
			ClientLoggerFactory loggerFactory)
		{
			this.loggerFactory = loggerFactory;
			this.logger = loggerFactory.CreateLogger<CmppClient>();
			config.ClientStatus = ClientStatus.WAITING_CONNECT;
			config.Version = CmppVersion.CMPP20;
			Config = config;
			smsHandler.client = this;
			this.smsHandler = smsHandler;
			matchQueue = new SubmitSmsMatchPool<MsgEx>(32, 60 * 1000);
			if (smsHandler != null)
			{
				matchQueue.timeOutHandle = smsHandler.SubmitTimeOutHandle;
			}
			eventExecutorGroup = new MultithreadEventLoopGroup();
			group = new MultithreadEventLoopGroup();
			dbContext = new SqliteContext(Config.ClientId);
			InitClient();

		}

		public CmppClient(
			CmppClientConfig config,
			BaseCmppSmsHandler smsHandler,
			ILoggerFactory loggerFactory) : this(config, smsHandler, new ClientLoggerFactory(loggerFactory, config))
		{

		}

		public CmppClient(
		CmppClientConfig config,
		BaseCmppSmsHandler smsHandler) : this(config, smsHandler, new ClientLoggerFactory(config))
		{

		}

		public void InitClient()
		{
			try
			{
				bootstrap
					.Group(group)
					.Channel<TcpSocketChannel>()
					.Option(ChannelOption.TcpNodelay, true)
					.Option(ChannelOption.ConnectTimeout, TimeSpan.FromSeconds(3))
					.Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
					{
						IChannelPipeline pipeline = channel.Pipeline;

						//pipeline.AddLast("log", new LoggingHandler("Log"));

						pipeline.AddLast("timeout", new IdleStateHandler(0, 0, 30));

						pipeline.AddLast("connectionWatchdog", new ConnectionWatchdog(bootstrap, Config, sessionMap, timer, loggerFactory));

						pipeline.AddLast("activeIdleStateTrigger", new ActiveIdleStateTrigger(loggerFactory));

						pipeline.AddLast("enc", new Cmpp2Encoder(loggerFactory));

						pipeline.AddLast("dec", new Cmpp2Decoder(loggerFactory, ushort.MaxValue, 0, 4, -4, 0));

						pipeline.AddLast(eventExecutorGroup, "submit", new CmppSubmitHandler(smsHandler, matchQueue, loggerFactory));

						pipeline.AddLast(eventExecutorGroup, "deliver", new CmppDeliverHandler(smsHandler, loggerFactory));

						pipeline.AddLast(eventExecutorGroup, "connetc", new CmppConnectHandler(Config, sessionMap, ConnectCallBack, timer, loggerFactory));

						pipeline.AddLast(eventExecutorGroup, "active", new CmppActiveHandler(loggerFactory));

						pipeline.AddLast(eventExecutorGroup, "connectClose", new ConnectCloseHandler(sessionMap, smsHandler.ConnectCloseCompleteCallBack, loggerFactory));

					}));

				if (Config.AutoReConnect)
				{
					ForeverReConnect();
				}
				else
				{
					//ConnectAsync().GetAwaiter().GetResult();
				}
			}
			catch (Exception e)
			{
				logger.Error(e);
				group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
			}
		}

		public void ForeverReConnect()
		{
			int connNum = Config.ConnNum - sessionMap.Count;

			var policy = Policy
				  .Handle<Exception>()
				  .WaitAndRetryForeverAsync((retryCount) => TimeSpan.FromSeconds(5), (e, rc, ts) =>
				  {
					  Config.ClientStatus = ClientStatus.CONNECT_FAIL;
					  logger.Error($"open {Config.Ip}:{Config.Port} fali, retry {rc}", e);
				  });

			for (int i = 0; i < connNum; i++)
			{
				Session session = new Session(loggerFactory);
				sessionMap.AddSession(session);
				policy.ExecuteAsync((token) =>
				{
					return bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse(Config.Ip), Config.Port));

				}, session.reConnect.Token).ContinueWith(t =>
				{
					IChannel channel = t.Result;
					logger.Info($"open {Config.Ip}:{Config.Port} success", channel.ToString());
					session.Channel = channel;
				});
			}
		}

		//public async Task<bool> ConnectAsync()
		//{
		//	int connNum = Config.ConnNum - sessionMap.Count;
		//	for (int i = 0; i < connNum; i++)
		//	{
		//		try
		//		{
		//			await bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse(Config.Ip), Config.Port));
		//		}
		//		catch (Exception e)
		//		{
		//			ClientStatus = ClientStatus.CONNECT_FAIL;
		//			logger.Error($"connect {Config.Ip}:{Config.Port} exception", e);
		//			return false;
		//		}
		//	}
		//	return true;
		//}

		//void StartSubmit()
		//{
		//	Stopwatch stopwatch = new Stopwatch();
		//	stopwatch.Start();
		//	Task.Factory.StartNew(() =>
		//   {
		//	   logger.Info($"客户端{Config.ClientId}发送任务开始");
		//	   //重试设置 提交返回失败时重试两次 
		//	   var policy = Policy
		//			  .HandleResult<bool>((r) => !r)
		//			  .RetryAsync(2, async (dr, retryCount) =>
		//			  {
		//				  logger.Info($"客户端{Config.ClientId}发送失败尝试重试, 第{retryCount.ToString()}次重试");
		//				  await Task.CompletedTask;
		//			  });
		//	   Sms.Common.Sms sms = null;
		//	   while (!tokenSource.IsCancellationRequested)
		//	   {
		//		   try
		//		   {
		//			   if (ClientStatus == ClientStatus.CONNECT_SUCCESS && !smsQueue.IsEmpty && smsQueue.TryDequeue(out sms))
		//			   {
		//				   policy.ExecuteAsync(async () => await SubmitSmsAsync(sms))
		//				   .ContinueWith(t =>
		//				   {
		//					   if (t.IsFaulted || !t.Result)
		//					   {
		//						   logger.Error($"客户端{Config.ClientId}发送短信失败, sms:{sms?.ToString()}, exception:{t.Exception}");
		//					   }
		//				   });
		//			   }
		//			   else
		//			   {
		//				   Thread.Sleep(100);
		//			   }
		//		   }
		//		   catch (Exception e)
		//		   {
		//			   logger.Error(e);
		//		   }

		//	   }
		//   }, tokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default).ContinueWith(t =>
		//   {
		//	   logger.Info($"客户端{Config.ClientId}发送任务完成, 运行时间:{stopwatch.ElapsedMilliseconds / 1000}s");
		//   });

		//}


		public async Task<bool> SubmitSmsAsync(Sms.Common.Sms sms)
		{
			var session = sessionMap.GetLoginSuccSessionRandom();
			if (session == null && !session.Send)
			{
				return false;
			}
			List<string> list = CmppTools.SplitLongMessage(sms.Messgae);
			var isLongMessage = list.Count > 1;
			string[] destinations = new string[] { sms.Mobile };
			string longid = Config.ClientId.ToString() + CmppTools.GetHeadSequenceId().ToString();
			for (int i = 0; i < list.Count; i++)
			{
				var content = list[i];
				var serial = i + 1;
				var count = list.Count;
				var submit = Cmpp2CmppSubmitPack(content, sms.ExtendedCode, destinations, isLongMessage, count, serial);
				MsgEx msgEx = new MsgEx()
				{
					Count = list.Count,
					ExObj = sms.ExObj,
					Id = longid,
					IsLong = isLongMessage,
					Serial = i
				};
				var sequenceId = CmppTools.GetHeadSequenceId();
				var smsPacket = CmppTools.GroupPacket(submit, sequenceId);
				while (!matchQueue.Add(session.Channel, smsPacket, msgEx, sequenceId.ToString()))
				{
					logger.Debug($"wait sms add match cache");
					Thread.Sleep(1);
				}
				logger.Info($"sms {longid} add match cache");

				await CmppTools.SendAsync(session.Channel, smsPacket);
			}
			return true;
		}

		public void ConnectCallBack()
		{
			if (sessionMap.Count == Config.ConnNum)
			{
				Config.ClientStatus = ClientStatus.CONNECT_SUCCESS;
				smsHandler?.ConnectSuccessCallBack();
				//StartSubmit();
			}
		}

		public CmppSubmit Cmpp2CmppSubmitPack(string content,
			string extendedCode,
			string[] destinations,
			bool isLongMessage,
			int count,
			int serial)
		{
			var needStatusReport = true;
			CmppSubmit submit = new CmppSubmit();
			submit.MsgContent = content;
			submit.MsgFmt = (byte)(CmppEncoding.UCS2);
			submit.SrcId = extendedCode;
			submit.DestTerminalId = destinations;
			submit.ServiceId = Config.ServiceId;
			submit.RegisteredDelivery = (byte)(needStatusReport ? 1 : 0);
			submit.FeeType = string.Format("{0:D2}", (int)FeeType.Free);
			submit.FeeUserType = (byte)FeeUserType.SP;
			submit.FeeTerminalId = "0";
			submit.FeeCode = "05";
			submit.MsgLevel = 0;
			submit.TPPId = 0;
			submit.TPUdhi = (byte)(isLongMessage ? 1 : 0);
			submit.PkTotal = (byte)count;
			submit.PkNumber = (byte)serial;
			submit.MsgSrc = Config.SourceAddress;
			submit.ValidTime = "";
			submit.AtTime = "";
			submit.Reserve = "0";

			return submit;
		}

		public async ValueTask DisposeAsync()
		{
			logger.Info("disposeAsync start");
			Config.AutoReConnect = false;
			Config.ClientStatus = ClientStatus.CONNECT_CLOSE;
			tokenSource?.Cancel();
			await sessionMap.DisposeAsync();
			if (!smsQueue.IsEmpty)
			{
				dbContext?.Sms?.AddRange(smsQueue.ToArray());
				var row = dbContext?.SaveChanges();
				logger.Info($"save not send sms {row} to sqlite");
				dbContext?.Dispose();
			}
			smsQueue = null;
			await group?.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
			logger.Info("disposeAsync complete");
		}
	}
}
