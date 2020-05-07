using System;
using System.Runtime.InteropServices;

namespace Sms.Protocol.Cmpp2
{
    /// <summary>
    /// CMPP_ACTIVE_TEST 定义（SP->ISMG 或 ISMG->SP）。
    /// </summary>
    /// <remarks>
    /// 心跳包 链路检测（CMPP_ACTIVE_TEST）操作：本操作仅适用于通信双方采用长连接通信方式时用于保持连接。
    /// </remarks>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct CmppActiveTest : IMessage
    {
        #region 消息体
        
        #endregion

        public uint GetCommandId()
        {
            return CmppConstants.CommandCode.ActiveTest;
        }

        public byte[] ToBytes()
        {
            return new byte[0];
        }

        public void FromBytes(byte[] body)
        {
            
        }

		public CmppActiveTest FromBytes0(byte[] array)
		{
            FromBytes(array);
            return this;
        }

        public override string ToString()
        {
            return "";
        }
    }
}
