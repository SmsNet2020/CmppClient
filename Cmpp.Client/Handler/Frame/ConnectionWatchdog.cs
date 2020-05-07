using DotNetty.Codecs;
using DotNetty.Common.Internal.Logging;
using DotNetty.Common.Utilities;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using Sms.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Cmpp.Client.Handler.Frame
{
	public class ConnectionWatchdog : ChannelHandlerAdapter, ITimerTask
	{
		private Bootstrap bootstrap;
		private ITimer timer;
		private CmppClientConfig config;
		readonly ClientLogger<ConnectionWatchdog> logger;
		ClientLoggerFactory loggerFactory;

		public int WaitConnectSecond => 5;
		SessionMap sessionMap;
		public ConnectionWatchdog(Bootstrap bootstrap, CmppClientConfig config, SessionMap sessionMap,  ITimer timer, ClientLoggerFactory loggerFactory)
		{
			this.bootstrap = bootstrap;
			this.timer = timer;
			this.config = config;
			this.sessionMap = sessionMap;
			this.logger = loggerFactory.CreateLogger<ConnectionWatchdog>();
			this.loggerFactory = loggerFactory;
		}

		public override void ChannelActive(IChannelHandlerContext ctx)
		{
			logger.Info($"channel active", ctx.Channel.ToString());
			ctx.FireChannelActive();
		}

		public override void ChannelInactive(IChannelHandlerContext ctx)
		{
			logger.Info($"channel Inactive", ctx.Channel.ToString());
			Reconnect();
			ctx.FireChannelInactive();
		}

		public void Reconnect()
		{
			if (config.AutoReConnect)
			{
				logger.Info($"reconnect in {WaitConnectSecond.ToString()} seconds");
				var delay = new TimeSpan(0, 0, WaitConnectSecond);
				timer.NewTimeout(this, delay);
			}
		}

		public void Run(ITimeout timeout)
		{
			try
			{
				logger.Info($"reconnect start");
				var channel = bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse(config.Ip), config.Port)).GetAwaiter().GetResult();
				var session = new Session(loggerFactory);
				session.Channel = channel;
				sessionMap.AddSession(session);
				logger.Info($"reconnect complete", channel.ToString());
			}
			catch (Exception e)
			{
				logger.Error($"reconnect exception", e);
				Reconnect();
			}
		}
	}
}
