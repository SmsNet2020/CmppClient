﻿using System;
using System.Runtime.InteropServices;

namespace Sms.Protocol.Cmpp2
{
    /// <summary>
    /// CMPP_ACTIVE_TEST_RESP 定义（SP->ISMG 或 ISMG->SP）。
    /// </summary>
    /// <remarks>
    /// 链路检测（CMPP_ACTIVE_TEST）操作：本操作仅适用于通信双方采用长连接通信方式时用于保持连接。
    /// </remarks>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct CmppActiveTestResp : IMessage
    {
        #region 消息体
        public byte Reserved;
        #endregion

        public uint GetCommandId()
        {
            return CmppConstants.CommandCode.ActiveTestResp;
        }

        public byte[] ToBytes()
        {
            return new[] { Reserved };
        }

        public void FromBytes(byte[] body)
        {
			if (body == null || body.Length != (CmppConstants.HeaderSize + CmppConstants.PackageBodySize.CmppActiveTestResp))
				//throw new ArgumentException(string.Format("Invalid bytes to unmarshal for {0}.", GetType().Name));

			if (body.Length == 13)
			{
				Reserved = body[CmppConstants.HeaderSize + 0];
			}
        }

		public CmppActiveTestResp FromBytes0(byte[] array)
		{
            FromBytes(array);
            return this;
        }
	}
}
