using System;
using System.Collections.Generic;
using System.Text;

namespace Sms.Common
{
	public enum ClientStatus
	{
		/// <summary>
		/// 连接完成
		/// </summary>
		CONNECT_SUCCESS,
		/// <summary>
		/// 连接错误 一般为打开socket出错
		/// </summary>
		CONNECT_FAIL,
		/// <summary>
		/// 验证出错
		/// </summary>
		LOGIN_FAIL,
		/// <summary>
		/// 等待连接
		/// </summary>
		WAITING_CONNECT,
		/// <summary>
		/// 关闭连接释放connect
		/// </summary>
		CONNECT_CLOSE
	}
}
