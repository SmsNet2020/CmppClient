using DotNetty.Transport.Channels;
using Sms.Common;
using Sms.Protocol;
using Sms.Protocol.Cmpp2;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cmpp.Client.Handler.Sms
{
	public class CmppDeliverHandler : SimpleChannelInboundHandler<CmppMessageReceiveArgs<CmppHead, CmppDeliver>>
	{
		BaseCmppSmsHandler baseSmsHandler;

		public CmppDeliverHandler(BaseCmppSmsHandler baseSmsHandler, ClientLoggerFactory loggerFactory)
		{
			this.baseSmsHandler = baseSmsHandler;
		}

		protected override void ChannelRead0(IChannelHandlerContext ctx, CmppMessageReceiveArgs<CmppHead, CmppDeliver> msg)
		{
			baseSmsHandler.CmppDeliver(msg);
			var resp = GetConnectRequestPacket(msg.Message.MsgId, msg.Header.SequenceId);
			CmppTools.SendAsync(ctx, resp);
		}

		private SmsPacket GetConnectRequestPacket(ulong msgId,uint seq)
		{
			var date = DateTime.Now;
			IMessage message = new CmppDeliverResp()
			{
				MsgId = msgId,
				Result = 0
			};
			return CmppTools.GroupPacket(message, seq);
		}
	}
}
