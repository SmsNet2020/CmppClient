using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Sms.Protocol.Cmpp2
{
    /// <summary>
    /// CMPP_CONNECT_RESP 消息定义（ISMG->SP）。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct CmppConnectResp : IMessage
    {
        #region 消息体
        /// <summary>
        /// 状态（0：正确；1：消息结构错；2：非法源地址；3：认证错；4：版本太高；5~ ：其他错误）。
        /// </summary>
        public byte Status { get; set; }
        /// <summary>
        /// ISMG 认证码，用于鉴别 ISMG。其值通过单向 MD5 hash 计算得出，表示如下：AuthenticatorISMG = MD5(Status + AuthenticatorSource + shared secret)，Shared secret 由中国移动与源地址实体事先商定，AuthenticatorSource 为源地址实体发送给 ISMG 的对应消息 CMPP_Connect 中的值。认证出错时，此项为空。
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        private byte[] authenticatorISMG;
        public byte[] AuthenticatorISMG
        {
            get
            {
                if (authenticatorISMG == null)
                {
                    return new byte[16];
                }
                return authenticatorISMG;
            }
            set
            {
                authenticatorISMG = value;
            }
        }
        /// <summary>
        /// 服务器支持的最高版本号，对于 3.0 的版本，高 4 bit 为 3，低 4 位为 0。
        /// </summary>
        public byte Version;
        #endregion

        #region 公有方法
        public override string ToString()
        {
            return $"{{{nameof(Status)}={Status}, " +
                $"{nameof(AuthenticatorISMG)}={string.Join(",", AuthenticatorISMG) }}}";
        }

		public CmppConnectResp FromBytes0(byte[] body)
		{
            FromBytes(body);
            return this;
        }

		//public override string ToString()
		//{
		//    //switch (Status)
		//    //{
		//    //    case 0:
		//    //        return "成功";
		//    //    case 1:
		//    //        return "消息结构错";
		//    //    case 2:
		//    //        return "非法源地址";
		//    //    case 3:
		//    //        return "认证错";
		//    //    case 4:
		//    //        return "版本太高";
		//    //    default:
		//    //        return string.Format("其他错误（错误码：{0}）", Status);
		//    //}


		//}


		#endregion

		public uint GetCommandId()
        {
            return CmppConstants.CommandCode.ConnectResp;
        }

        public byte[] ToBytes()
        {
            var buffer = new List<byte>(CmppConstants.PackageBodySize.CmppConnectResp);
            buffer.Add((byte)Status);
            buffer.AddRange(AuthenticatorISMG);
            buffer.Add(Version);
            return buffer.ToArray();
        }

        public void FromBytes(byte[] body)
        {
            if (body == null || body.Length != (CmppConstants.HeaderSize + CmppConstants.PackageBodySize.CmppConnectResp))
                throw new ArgumentException(string.Format("Invalid bytes to unmarshal for {0}.", GetType().Name));
            Status = body[CmppConstants.HeaderSize];//Convert.ToUInt(body, CmppConstants.HeaderSize + 0);
            AuthenticatorISMG = new byte[16];
            Array.Copy(body, CmppConstants.HeaderSize + 1, AuthenticatorISMG, 0, 16);
            Version = body[CmppConstants.HeaderSize + CmppConstants.PackageBodySize.CmppConnectResp - 1];
        }


    }
}
