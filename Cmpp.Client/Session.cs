using DotNetty.Transport.Channels;
using Sms.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Cmpp.Client
{
	public class Session
	{
		ClientLogger<Session> logger;
		public CancellationTokenSource reConnect = new CancellationTokenSource();
		public Session( ClientLoggerFactory loggerFactory)
		{
			logger = loggerFactory.CreateLogger<Session>();
		}

		public IChannel Channel { get; set; }

		bool send;
		public bool Send
		{
			get => send;
			set
			{
				send = value;
				logger.Info($"update send status {send.ToString()}", Channel?.ToString());
			}
		}

		public bool Active => Channel != null && Channel.Active;
	}
}
