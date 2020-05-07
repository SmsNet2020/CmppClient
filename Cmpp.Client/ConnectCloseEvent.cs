using System;
using System.Collections.Generic;
using System.Text;

namespace Cmpp.Client
{
	public class ConnectCloseEvent
	{
		public ConnectCloseEvent(string eventName)
		{
			EventName = eventName;
		}

		public string EventName { get; set; }
	}
}
