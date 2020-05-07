using DotNetty.Common.Internal.Logging;
using DotNetty.Transport.Channels;
using Sms.Common;
using Sms.Protocol;
using Sms.Protocol.Cmpp2;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cmpp.Client.Handler.Sms
{
	public class CmppActiveHandler : SimpleChannelInboundHandler<CmppMessageReceiveArgs<CmppHead, CmppActiveTest>>
	{
		readonly ClientLogger<CmppActiveHandler> logger;

		public CmppActiveHandler(ClientLoggerFactory loggerFactory)
		{
			logger = loggerFactory.CreateLogger<CmppActiveHandler>();
		}

		protected override void ChannelRead0(IChannelHandlerContext ctx, CmppMessageReceiveArgs<CmppHead, CmppActiveTest> msg)
		{
			logger.Info($"receive active request: {msg}", ctx.Channel.ToString());
			SmsPacket activeResp = GetActiveRespPacket(msg.Header);
			CmppTools.SendAsync(ctx, activeResp);
		}

		private SmsPacket GetActiveRespPacket(CmppHead head)
		{
			IMessage message = new CmppActiveTestResp();
			return CmppTools.GroupPacket(message, head.SequenceId);
		}
	}
}
