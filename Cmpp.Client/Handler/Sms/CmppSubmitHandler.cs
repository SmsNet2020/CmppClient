using DotNetty.Common.Internal.Logging;
using DotNetty.Transport.Channels;
using Sms.Common;
using Sms.Protocol;
using Sms.Protocol.Cmpp2;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Cmpp.Client.Handler.Sms
{
	public class CmppSubmitHandler : SimpleChannelInboundHandler<CmppMessageReceiveArgs<CmppHead, CmppSubmitResp>>
	{
		SubmitSmsMatchPool<MsgEx> matchQueue;
		BaseCmppSmsHandler smsHandler;
		readonly ClientLogger<CmppSubmitHandler> logger;

		public CmppSubmitHandler(BaseCmppSmsHandler smsHandler, SubmitSmsMatchPool<MsgEx> matchQueue, ClientLoggerFactory loggerFactory)
		{
			this.matchQueue = matchQueue;
			this.smsHandler = smsHandler;
			logger = loggerFactory.CreateLogger<CmppSubmitHandler>();
		}

		protected override void ChannelRead0(IChannelHandlerContext ctx, CmppMessageReceiveArgs<CmppHead, CmppSubmitResp> msg)
		{
			if (matchQueue.Match(ctx.Channel, msg.Header.SequenceId.ToString(), out Item<MsgEx> item))//匹配流水号
			{
				msg.Item = item;
				CmppMessageReceiveArgs<CmppHead, CmppSubmitResp, Item<MsgEx>> msgEx
					= new CmppMessageReceiveArgs<CmppHead, CmppSubmitResp, Item<MsgEx>>(msg.Header, msg.Message, item);
				smsHandler?.Cmpp2LongSmsRespondAssembly(msgEx);
			}
			else
			{
				logger.Warn($"sms is removed from match cache{msg}", ctx.Channel.ToString());
			}
		}

		/// <summary>是否匹配响应</summary>
		/// <param name="request"></param>
		/// <param name="response"></param>
		/// <returns></returns>
		private Boolean Cmpp2IsMatch(Object request, Object response)
		{
			bool match = false;
			match = request is SmsPacket req_pack &&
				response is CmppHead res_head &&
				req_pack.Head is CmppHead req_head &&
				req_head.SequenceId == res_head.SequenceId;
			return match;
		}

		/// <summary>是否匹配响应</summary>
		/// <param name="request"></param>
		/// <param name="response"></param>
		/// <returns></returns>
		private Boolean Cmpp3IsMatch(Object request, Object response)
		{
			bool match = false;
			match = request is SmsPacket req_pack &&
				response is SmsPacket res_pack &&
				req_pack.Head is CmppHead req_head &&
				res_pack.Head is CmppHead res_head &&
				req_head.SequenceId == res_head.SequenceId;
			return match;
		}
	}
}
