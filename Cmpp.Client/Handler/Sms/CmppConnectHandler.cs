using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using Sms.Common;
using Sms.Protocol.Cmpp2;
using System.Linq;
using DotNetty.Common.Internal.Logging;
using DotNetty.Common.Utilities;
using Sms.Protocol;

namespace Cmpp.Client.Handler.Sms
{
	public class CmppConnectHandler : SimpleChannelInboundHandler<CmppMessageReceiveArgs<CmppHead, CmppConnectResp>>, ITimerTask
	{
		Action connectCallBack_OK;
		CmppClientConfig config;
		int disconnectSecond = 5;
		SessionMap sessionMap;
		readonly ClientLogger<CmppConnectHandler> logger;
		byte[] authenticatorSource;
		ITimer timer;
		IChannelHandlerContext context;
		ClientLoggerFactory loggerFactory;
		public CmppConnectHandler(CmppClientConfig config, SessionMap sessionMap, Action connectCallBack_OK, ITimer timer, ClientLoggerFactory loggerFactory)
		{
			this.config = config;
			this.sessionMap = sessionMap;
			this.connectCallBack_OK = connectCallBack_OK;
			this.timer = timer;
			logger = loggerFactory.CreateLogger<CmppConnectHandler>();
			this.loggerFactory = loggerFactory;
		}

		public override void ChannelActive(IChannelHandlerContext context)
		{
			logger.Info($"start login user:{config.UserName}, pwd:{config.Password}");
			this.context = context;
			SmsPacket connect = GetConnectRequestPacket(config.UserName, config.Password);
			CmppTools.SendAsync(context, connect);
			var delay = new TimeSpan(0, 0, disconnectSecond);
			timer.NewTimeout(this, delay);
			base.ChannelActive(context);
		}

		protected override void ChannelRead0(IChannelHandlerContext ctx, CmppMessageReceiveArgs<CmppHead, CmppConnectResp> msg)
		{
			var connectBody = msg.Message;
			byte connectSuccess = 0;
			var channel = ctx.Channel;
			if (connectBody.Status.Equals(connectSuccess) && connectBody.AuthenticatorISMG != null)
			{
				var authenticatorISMG
					= CmppTools.CreateAuthenticatorISMG(connectSuccess, authenticatorSource, config.Password);
				if (authenticatorISMG.SequenceEqual(authenticatorISMG))
				{
					logger.Info($"login success, return:{msg.Message.Status.ToString()}", ctx.Channel.ToString());
					var session = sessionMap.GetSessionOrDefault(ctx);
					if (session == null) ctx.Channel.CloseAsync();
					session.Send = true;
					connectCallBack_OK?.Invoke();
					return;
				}
			}
			logger.Warn($"login fail, return:{msg.Message.Status.ToString()}", ctx.Channel.ToString());
			ctx.Channel.CloseAsync();
		}

		private SmsPacket GetConnectRequestPacket(string userName, string password)
		{
			var date = DateTime.Now;
			authenticatorSource = CmppTools.CreateAuthenticatorSource(date, userName, password);
			IMessage message = new CmppConnect()
			{
				TimeStamp = uint.Parse(string.Format("{0:MMddHHmmss}", date)),
				AuthenticatorSource = authenticatorSource,
				Version = CmppConstants.Version,
				SourceAddress = userName,
			};
			return CmppTools.GroupPacket(message);
		}

		public void Run(ITimeout timeout)
		{
			var session = sessionMap.GetSessionOrDefault(context);

			if (session == null && config.ClientStatus != ClientStatus.CONNECT_CLOSE)
			{
				logger.Info("login timeout", context.Channel.ToString());
				context.Channel.CloseAsync();
			}
		}
	}
}
