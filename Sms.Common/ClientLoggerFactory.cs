using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sms.Common
{
	public class ClientLoggerFactory
	{
		ILoggerFactory loggerFactory;
		ClientConfig clientConfig;
		public ClientLoggerFactory(ILoggerFactory loggerFactory, ClientConfig clientConfig)
		{
			this.loggerFactory = loggerFactory;
			this.clientConfig = clientConfig;
		}

		public ClientLoggerFactory(ClientConfig config) : this(DefaultLoggerFactory, config)
		{

		}

		public static ILoggerFactory DefaultLoggerFactory
		{
			get
			{
				ConsoleLoggerOptions options = new ConsoleLoggerOptions();
				MyIOptionsMonitor<ConsoleLoggerOptions> monitor = new MyIOptionsMonitor<ConsoleLoggerOptions>(options);
				ConsoleLoggerProvider provider = new ConsoleLoggerProvider(monitor);
				var loggerFactory = new LoggerFactory();
				loggerFactory.AddProvider(provider);
				return loggerFactory;
			}
		}
		public ClientLogger<T> CreateLogger<T>() => new ClientLogger<T>(loggerFactory.CreateLogger<T>(), clientConfig);
	}

	class MyIOptionsMonitor<T> : IOptionsMonitor<T>
	{

		T t;

		public MyIOptionsMonitor(T t)
		{
			this.t = t;
		}

		public T CurrentValue => t;

		public T Get(string name) => t;

		public IDisposable OnChange(Action<T, string> listener)
		{
			return null;
		}
	}
}
