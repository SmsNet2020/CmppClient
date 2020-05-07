using Cmpp.Client;
using Cmpp.Client.Handler.Sms;
using Microsoft.Extensions.Logging;
using Sms.Common;
using Sms.Protocol.Cmpp2;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
namespace Cmpp.Client.OrdinarySms
{
	class Program
	{
		static ClientLoggerFactory loggerFactory = new ClientLoggerFactory(ClientLoggerFactory.DefaultLoggerFactory, GetCmppClientConfig());

		static async Task Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			CmppClientConfig config = GetCmppClientConfig();
			CmppClient cmppClient = new CmppClient(config, new CmppSmsHandler(loggerFactory), loggerFactory);
			Console.ReadLine();
			await cmppClient.DisposeAsync();
		}

		public static CmppClientConfig GetCmppClientConfig()
		{
			CmppClientConfig config = new CmppClientConfig()
			{
				ConnNum = 10,
				ClientId = "Test",
				Ip = "127.0.0.1",
				Password = "123456",
				Port = 7890,
				SourceAddress = "614450",
				UserName = "614450"
			};
			return config;
		}
	}
}
