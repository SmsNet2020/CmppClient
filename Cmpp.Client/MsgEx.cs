using System;
using System.Collections.Generic;
using System.Text;

namespace Cmpp.Client
{
	public class MsgEx
	{
		/// <summary>
		/// 是否长短信
		/// </summary>
		public bool IsLong { get; set; }

		/// <summary>
		/// 短信条数
		/// </summary>
		public int Count { get; set; }

		/// <summary>
		/// 第几条
		/// </summary>
		public int Serial { get; set; }

		public string Id { get; set; }

		public object ExObj { get; set; }

		public override string ToString()
		{
			return $"{{{nameof(IsLong)}={IsLong.ToString()}, {nameof(Count)}={Count.ToString()}, {nameof(Serial)}={Serial.ToString()}, {nameof(Id)}={Id}, {nameof(ExObj)}={ExObj}}}";
		}
	}
}
