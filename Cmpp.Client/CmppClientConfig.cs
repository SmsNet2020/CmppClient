using Sms.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cmpp.Client
{
	public class CmppClientConfig : ClientConfig
	{

		public string UserName { get; set; }

		/// <summary>
		/// 连接数
		/// </summary>
		public int ConnNum { get; set; }

		/// <summary>
		/// ip
		/// </summary>
		public string Ip { get; set; }

		/// <summary>
		/// 端口
		/// </summary>
		public int Port { get; set; }

		/// <summary>
		/// 源地址，此处为SP_Id，即 SP 的企业代码（长度为 6 字节）。
		/// </summary>
		/// <remarks>
		/// SP_Id（SP 的企业代码）：网络中 SP 地址和身份的标识、地址翻译、计费、结算等均以企业代码为依据。企业代码以数字表示，共 6 位，从“9XY000”至“9XY999”，其中“XY”为各移动公司代码。
		/// </remarks>
		public string SourceAddress { get; set; }

		public string Password { get; set; }
		public string ServiceId { get; set; }

		public bool AutoReConnect { get; set; } = true;

		public CmppVersion Version { get; internal set; }
		public ClientStatus ClientStatus { get; internal set; }
	}
}
