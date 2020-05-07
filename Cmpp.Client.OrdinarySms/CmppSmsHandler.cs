using Cmpp.Client.Handler.Sms;
using Microsoft.Extensions.Logging;
using Sms.Common;
using Sms.Protocol.Cmpp2;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cmpp.Client.OrdinarySms
{
	public class CmppSmsHandler : BaseCmppSmsHandler
	{
		ClientLogger<CmppSmsHandler> logger;
		public CmppSmsHandler(ClientLoggerFactory loggerFactory)
		{
			logger = loggerFactory.CreateLogger<CmppSmsHandler>();
		}

		protected override void Cmpp2SubmitResp(List<CmppMessageReceiveArgs<CmppHead, CmppSubmitResp, Item<MsgEx>>> msgs)
		{
			logger.Info("Cmpp2SubmitResp receive: " + string.Join(",", msgs));
		}

		public override void CmppDeliver(CmppMessageReceiveArgs<CmppHead, CmppDeliver> msg)
		{
			logger.Info("CmppDeliver receive: " + msg);
		}

		public override void SubmitTimeOutHandle(Item<MsgEx> item)
		{
			logger.Info("SubmitTimeOutHandle");
		}

		public override void ConnectSuccessCallBack()
		{
			Sms.Common.Sms sms = new Sms.Common.Sms()
			{
				ExObj = "",
				ExtendedCode = "0",
				//Id = Guid.NewGuid().ToString(),
				Messgae = "【test】123456",
				Mobile = "13800138000"
			};
			client.SubmitSmsAsync(sms).ContinueWith(t =>
			{
				if (t.Result)
				{
					logger.Info("===================>send complete");
				}
				else
				{
					logger.Info("===================>send fail");
				}
			});
		}

		public override void ConnectCloseCompleteCallBack()
		{

		}
	}
}
