using DotNetty.Buffers;
using DotNetty.Common.Internal.Logging;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using Sms.Common; 
using Sms.Protocol;
using Sms.Protocol.Cmpp2;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cmpp.Client.Handler.Frame
{
	public class ActiveIdleStateTrigger : ChannelHandlerAdapter
	{
		private static int TRY_TIMES = 3;
		private int currentTime = 0;
		readonly ClientLogger<ActiveIdleStateTrigger> logger;

		public ActiveIdleStateTrigger(ClientLoggerFactory loggerFactory)
		{
			logger = loggerFactory.CreateLogger<ActiveIdleStateTrigger>();
		}

		public override void Read(IChannelHandlerContext context)
		{
			currentTime = 0;
			base.Read(context);
		}

		public override void UserEventTriggered(IChannelHandlerContext context, object evt)
		{

			if (evt is IdleStateEvent)
			{
				IdleState state = ((IdleStateEvent)evt).State;
				if (state == IdleState.AllIdle)
				{

					IChannel channel = context.Channel;
					string channelInfo = channel.ToString();
					logger.Info($"exec idle event send active request", channelInfo);
					if (currentTime < TRY_TIMES)
					{
						currentTime++;
						SendActiveTest(context);
					}
					else
					{
						logger.Info($"active idle close", channelInfo);
						channel.CloseAsync();
					}
				}
			}
			else
			{
				base.UserEventTriggered(context, evt);
			}
		}

		private void SendActiveTest(IChannelHandlerContext context)
		{
			IMessage message = new CmppActiveTest();
			CmppTools.GroupPacket(message);
			SmsPacket connect = CmppTools.GroupPacket(message);
			CmppTools.SendAsync(context, connect);
		}
	}
}
