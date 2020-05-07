using Cmpp.Client;
using Cmpp.Client.Handler.Sms;
using Sms.Common;
using Sms.Protocol.Cmpp2;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
namespace CmppClientTest
{
	class Program
	{
		static ILogger<Program> logger = ClientLoggerFactory.DefaultLoggerFactory.CreateLogger<Program>();
		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			List<CmppClient> clientList = new List<CmppClient>();
			for (int i = 0; i < 1; i++)
			{
				CmppClientConfig config = new CmppClientConfig()
				{
					ConnNum = 10,
					ClientId = "Test" + i,
					Ip = "127.0.0.1",
					Password = "123456",
					Port = 7890,
					SourceAddress = "614450",
					UserName = "614450"
				};
				CmppClient cmppClient = new CmppClient(config, new SmsHandler());
				clientList.Add(cmppClient);
			}
			Console.ReadLine();

			Sms.Common.Sms sms = new Sms.Common.Sms()
			{
				ExObj = "",
				ExtendedCode = "0",
				//Id = Guid.NewGuid().ToString(),
				Messgae = "【测试】123456",
				Mobile = "13800138000"
			};
			clientList[0].SubmitSmsAsync(sms).ContinueWith(t=> {
				if (t.Result)
				{
					Console.WriteLine("===================>发送完成");
				}
				else
				{
					Console.WriteLine("===================>发送失败");
				}
			});
			Console.ReadLine();

			ClientClose(clientList).ContinueWith(x =>
			{
				logger.LogInformation("=====================main>ClientClose task complete");
			});
			Console.ReadLine();
		}


		public static async Task ClientClose(List<CmppClient> clientList)
		{
			foreach (var item in clientList)
			{
				await item.DisposeAsync();
			}
		}
	}



	public class SmsHandler : BaseSmsHandler
	{

		protected override void Cmpp2SubmitResp(List<CmppMessageReceiveArgs<CmppHead, CmppSubmitResp, Item<MsgEx>>> msgs)
		{
			Console.WriteLine("SmsHandler接收数据: " + string.Join(",", msgs));
		}

		public override void CmppDeliver(CmppMessageReceiveArgs<CmppHead, CmppDeliver> msg)
		{
			Console.WriteLine("SmsHandler接收数据: " + msg);
		}

		public override void SubmitTimeOutHandle(Item<MsgEx> item)
		{
			Console.WriteLine("超时");
		}

		public override void ConnectSuccessCallBack()
		{

		}

		public override void ConnectCloseCallBack()
		{

		}
	}
}
