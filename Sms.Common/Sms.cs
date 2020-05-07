using System;
using System.Collections.Generic;
using System.Text;

namespace Sms.Common
{
	public class Sms
	{
		/// <summary>
		/// sqlite table id
		/// </summary>
		public long Id { get; set; }

		public string Messgae { get; set; }
		public string ExtendedCode { get; set; }
		public string Mobile { get; set; }

		/// <summary>
		/// 跟随提交响应返回
		/// </summary>
		public string ExObj { get; set; }
	}
}
