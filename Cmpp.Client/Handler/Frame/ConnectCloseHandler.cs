using DotNetty.Common.Internal.Logging;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Sms.Common;

namespace Cmpp.Client.Handler.Frame
{
	public class ConnectCloseHandler : ChannelHandlerAdapter
	{
		SessionMap sessionMap;
		Action connctCloseCallBack;
		readonly ClientLogger<ConnectCloseHandler> logger;
		public ConnectCloseHandler(SessionMap sessionMap, Action connctCloseCallBack, ClientLoggerFactory loggerFactory)
		{
			logger = loggerFactory.CreateLogger<ConnectCloseHandler>();
			this.sessionMap = sessionMap;
			this.connctCloseCallBack = connctCloseCallBack;
		}

		public override void ChannelInactive(IChannelHandlerContext context)
		{
			logger.Info("channel inactive, channel clear own session", context.Channel.ToString());
			var session = sessionMap.GetSessionOrDefault(context);
			sessionMap.TryRemove(session);
			session.Send = false;
			connctCloseCallBack?.Invoke();
			base.ChannelInactive(context);
		}

	}
}
