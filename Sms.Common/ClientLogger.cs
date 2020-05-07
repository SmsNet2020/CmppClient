using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sms.Common
{
	public class ClientLogger<T>
	{
		ClientConfig clientConfig;
		ILogger<T> logger;

		public ClientLogger(ILogger<T> logger, ClientConfig clientConfig)
		{
			this.logger = logger;
			this.clientConfig = clientConfig;
		}

		public void Debug(string msg, string channel)
		{
			logger.LogDebug($"{Formate(msg, channel)}");
		}
		public void Debug(string msg)
		{
			logger.LogDebug($"{Formate(msg)}");
		}
		public void Debug(string msg, string channel, Exception e)
		{
			logger.LogDebug(e, $"{Formate(msg, channel)}");
		}
		public void Debug(string msg, Exception e)
		{
			logger.LogDebug(e, $"{Formate(msg)}");
		}
		public void Debug(Exception e)
		{
			logger.LogDebug(e, $"{Formate(e)}");
		}

		public void Info(string msg, string channel)
		{
			logger.LogInformation($"{Formate(msg, channel)}");
		}
		public void Info(string msg)
		{
			logger.LogInformation($"{Formate(msg)}");
		}
		public void Info(string msg, string channel, Exception e)
		{
			logger.LogInformation(e, $"{Formate(msg, channel)}");
		}
		public void Info(string msg, Exception e)
		{
			logger.LogInformation(e, $"{Formate(msg)}");
		}
		public void Info(Exception e)
		{
			logger.LogInformation(e, $"{Formate(e)}");
		}



		public void Warn(string msg, string channel)
		{
			logger.LogWarning($"{Formate(msg, channel)}");
		}
		public void Warn(string msg)
		{
			logger.LogWarning($"{Formate(msg)}");
		}
		public void Warn(string msg, string channel, Exception e)
		{
			logger.LogWarning(e, $"{Formate(msg, channel)}");
		}
		public void Warn(string msg, Exception e)
		{
			logger.LogWarning(e, $"{Formate(msg)}");
		}
		public void Warn(Exception e)
		{
			logger.LogWarning(e, $"{Formate(e)}");
		}

		public void Error(string msg, string channel)
		{
			logger.LogError($"{Formate(msg, channel)}");
		}
		public void Error(string msg)
		{
			logger.LogError($"{Formate(msg)}");
		}
		public void Error(string msg, string channel, Exception e)
		{
			logger.LogError(e, $"{Formate(msg, channel)}");
		}
		public void Error(string msg, Exception e)
		{
			logger.LogError(e, $"{Formate(msg)}");
		}
		public void Error(Exception e)
		{
			logger.LogError(e, $"{Formate(e)}");
		}


		public string Formate(string msg, string channel)
		{
			return $"client_{clientConfig.ClientId}:{msg}, channel:{channel}: ";
		}
		public string Formate(string msg)
		{
			return $"client_{clientConfig.ClientId}: {msg}";
		}
		public string Formate(Exception e)
		{
			return $"client_{clientConfig.ClientId}: {e.Message}";
		}
	}
}
