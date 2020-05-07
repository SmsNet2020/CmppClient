using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Sms.Protocol.Cmpp2
{
    /// <summary>
    /// SP 向 ISMG 提交短信（CMPP_SUBMIT）操作（SP->ISMG）。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct CmppSubmit : IMessage
    {
        #region 消息体
        /// <summary>
        /// 信息标识（由ISMG生成，发送时不填，供ISMG传输时使用，可以从返回的RESP中获得本次发送的MSG_ID，通过 HEAD 中的 SequenceID 字段对应）。
        /// </summary>
        public ulong MsgId { get; set; }
        /// <summary>
        /// 相同 Msg_Id 的信息总条数，从 1 开始。
        /// </summary>
        public byte PkTotal { get; set; }
        /// <summary>
        /// 相同 Msg_Id 的信息序号，从 1 开始。
        /// </summary>
        public byte PkNumber { get; set; }
        /// <summary>
        /// 是否要求返回状态确认报告（0：不需要；1：需要）。
        /// </summary>
        public byte RegisteredDelivery { get; set; }
        /// <summary>
        /// 信息级别。
        /// </summary>
        public byte MsgLevel { get; set; }
        /// <summary>
        /// 业务标识，是数字、字母和符号的组合（长度为 10，SP的业务类型，数字、字母和符号的组合，由SP自定，如图片传情可定为TPCQ，股票查询可定义为11）。
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        public string ServiceId { get; set; }
        /// <summary>
        /// 计费用户类型字段（0：对目的终端MSISDN计费；1：对源终端MSISDN计费；2：对SP计费；3：表示本字段无效，对谁计费参见Fee_terminal_Id字段）。
        /// </summary>
        public byte FeeUserType { get; set; }
        /// <summary>
        /// 被计费用户的号码，当Fee_UserType为3时该值有效，当Fee_UserType为0、1、2时该值无意义。
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string FeeTerminalId { get; set; }
        /// <summary>
        /// 被计费用户的号码类型，0：真实号码；1：伪码。
        /// </summary>
        public byte FeeTerminalType { get; set; }
        /// <summary>
        /// GSM协议类型（详细解释请参考GSM03.40中的9.2.3.9）。
        /// </summary>
        public byte TPPId { get; set; }
        /// <summary>
        /// GSM协议类型（详细是解释请参考GSM03.40中的9.2.3.23,仅使用1位，右对齐）。
        /// </summary>
        public byte TPUdhi { get; set; }
        /// <summary>
        /// 信息格式（0：ASCII串；3：短信写卡操作；4：二进制信息；8：UCS2编码；15：含GB汉字）。
        /// </summary>
        public byte MsgFmt { get; set; }
        /// <summary>
        /// 信息内容来源（SP_Id：SP的企业代码：网络中SP地址和身份的标识、地址翻译、计费、结算等均以企业代码为依据。企业代码以数字表示，共6位，从“9XY000”至“9XY999”，其中“XY”为各移动公司代码）。
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 6)]
        public string MsgSrc { get; set; }
        /// <summary>
        /// 资费类别（01：对“计费用户号码”免费；02：对“计费用户号码”按条计信息费；03：对“计费用户号码”按包月收取信息费）。
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2)]
        public string FeeType { get; set; }
        /// <summary>
        /// 资费代码（以分为单位）。
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 6)]
        public string FeeCode { get; set; }
        /// <summary>
        /// 存活有效期，格式遵循SMPP3.3协议。
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 17)]
        public string ValidTime { get; set; }
        /// <summary>
        /// 定时发送时间，格式遵循SMPP3.3协议。
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 17)]
        public string AtTime { get; set; }
        /// <summary>
        /// 源号码（SP的服务代码或前缀为服务代码的长号码, 网关将该号码完整的填到SMPP协议Submit_SM消息相应的source_addr字段，该号码最终在用户手机上显示为短消息的主叫号码）。
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        public string SrcId { get; set; }
        /// <summary>
        /// 接收信息的用户数量（小于100个用户）。
        /// </summary>
        public byte DestUsrTl { get; set; }
        /// <summary>
        /// 接收短信的MSISDN号码。
        /// </summary>
        public string[] DestTerminalId { get; set; }
        /// <summary>
        /// 接收短信的用户的号码类型(0：真实号码；1：伪码）。
        /// </summary>
        public byte DestTerminalType { get; set; }
        /// <summary>
        /// 信息长度（Msg_Fmt值为0时：&lt; 160个字节；其它 &gt;= 140个字节)，取值大于或等于0。
        /// </summary>
        public byte MsgLength { get; set; }
        /// <summary>
        /// 信息内容。
        /// </summary>
        public string MsgContent { get; set; }
        /// <summary>
        /// 点播业务使用的LinkID，非点播类业务的MT流程不使用该字段。
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        //public string LinkId { get; set; }

        /// <summary>
        /// 保留
        /// </summary>
        public string Reserve { get; set; }
        #endregion

        public uint GetCommandId()
        {
            return CmppConstants.CommandCode.Submit;
        }

        public byte[] ToBytes()
        {
            if (DestTerminalId != null)
                DestUsrTl = (byte)DestTerminalId.Length;
            MsgLength = Convert.Length(MsgContent, MsgFmt);
            var buffer = new List<byte>(126 + 21 * DestUsrTl + MsgLength);

            buffer.AddRange(Convert.ToBytes(MsgId));
            buffer.Add(PkTotal);
            buffer.Add(PkNumber);
            buffer.Add(RegisteredDelivery);
            buffer.Add(MsgLevel);
            buffer.AddRange(Convert.ToBytes(ServiceId, CmppConstants.Encoding.ASCII, 10));
            buffer.Add(FeeUserType);
            buffer.AddRange(Convert.ToBytes(FeeTerminalId, CmppConstants.Encoding.ASCII, 21));
            buffer.Add(TPPId);
            buffer.Add(TPUdhi);
            buffer.Add(MsgFmt);
            buffer.AddRange(Convert.ToBytes(MsgSrc, CmppConstants.Encoding.ASCII, 6));
            buffer.AddRange(Convert.ToBytes(FeeType, CmppConstants.Encoding.ASCII, 2));
            buffer.AddRange(Convert.ToBytes(FeeCode, CmppConstants.Encoding.ASCII, 6));
            buffer.AddRange(Convert.ToBytes(ValidTime, CmppConstants.Encoding.ASCII, 17));
            buffer.AddRange(Convert.ToBytes(AtTime, CmppConstants.Encoding.ASCII, 17));
            buffer.AddRange(Convert.ToBytes(SrcId, CmppConstants.Encoding.ASCII, 21));
            buffer.Add(DestUsrTl);

            if (DestTerminalId != null)
                foreach (var dest in DestTerminalId)
                    buffer.AddRange(Convert.ToBytes(dest, CmppConstants.Encoding.ASCII, 21));

            //buffer.Add(DestTerminalType);
            buffer.Add(MsgLength);
            buffer.AddRange(Convert.ToBytes(MsgContent, MsgFmt));
            buffer.AddRange(Convert.ToBytes(Reserve, CmppConstants.Encoding.ASCII, 8));

            return buffer.ToArray();
        }

        public void FromBytes(byte[] body)
        {
            //string q = "";
            if (body == null)
                throw new ArgumentException(string.Format("Invalid bytes to unmarshal for {0}.", GetType().Name));
            int position = CmppConstants.HeaderSize;
            MsgId = Convert.ToUInt64(body, position);
            position += 8;
            PkTotal = body[position];
            position++;
            
            PkNumber = body[position];
            position++;

            RegisteredDelivery = body[position];
            position++;

            MsgLevel = body[position];
            position++;

            ServiceId = Convert.ToString(body, position, 10, CmppEncoding.ASCII);
            position += 10;

            FeeUserType = body[position];
            position++;

            FeeTerminalId = Convert.ToString(body, position, 21, CmppEncoding.ASCII);
            position += 21;

            TPPId = body[position];
            position++;

            TPUdhi = body[position];
            position++;

            MsgFmt = body[position];
            position++;

            MsgSrc = Convert.ToString(body, position, 6, CmppEncoding.ASCII);
            position += 6;

            FeeType = Convert.ToString(body, position, 2, CmppEncoding.ASCII);
            position += 2;

            FeeCode = Convert.ToString(body, position, 6, CmppEncoding.ASCII);
            position += 6;

            ValidTime = Convert.ToString(body, position, 17, CmppEncoding.ASCII);
            position += 17;

            AtTime = Convert.ToString(body, position, 17, CmppEncoding.ASCII);
            position += 17;

            SrcId = Convert.ToString(body, position, 21, CmppEncoding.ASCII).TrimEnd('\0');
            position += 21;

            DestUsrTl = body[position];
            position++;

            DestTerminalId = Convert.ToStringArray(body, position, out position, 21, DestUsrTl);

            MsgLength = body[position];
            position++;

            MsgContent = Convert.ToString(body, position, MsgLength, (CmppEncoding)MsgFmt);
            position += MsgLength;

            Reserve = Convert.ToString(body, position, 8, CmppEncoding.ASCII);

        }

        #region 重载
        public bool IsLongMessage()
        {
            return TPUdhi == 1;
        }

        public override string ToString()
        {
            return $"{{{nameof(MsgId)}={MsgId}, " +
                $"{nameof(PkTotal)}={PkTotal}, " +
                $"{nameof(PkNumber)}={PkNumber}, " +
                $"{nameof(RegisteredDelivery)}={RegisteredDelivery}, " +
                $"{nameof(MsgLevel)}={MsgLevel}, " +
                $"{nameof(ServiceId)}={ServiceId?.TrimEnd('\0')}, " +
                $"{nameof(FeeUserType)}={FeeUserType}, " +
                $"{nameof(FeeTerminalId)}={FeeTerminalId.TrimEnd('\0')}, " +
                $"{nameof(FeeTerminalType)}={FeeTerminalType}, " +
                $"{nameof(TPPId)}={TPPId}, {nameof(TPUdhi)}={TPUdhi}, " +
                $"{nameof(MsgFmt)}={MsgFmt}, {nameof(MsgSrc)}={MsgSrc}, " +
                $"{nameof(FeeType)}={FeeType.TrimEnd('\0')}, " +
                $"{nameof(FeeCode)}={FeeCode.TrimEnd('\0')}, " +
                $"{nameof(ValidTime)}={ValidTime.TrimEnd('\0')}, " +
                $"{nameof(AtTime)}={AtTime.TrimEnd('\0')}, " +
                $"{nameof(SrcId)}={SrcId.TrimEnd('\0')}, " +
                $"{nameof(DestUsrTl)}={DestUsrTl}, " +
                $"{nameof(DestTerminalId)}={(DestTerminalId.Length>0?DestTerminalId[0].TrimEnd('\0'):"")}, " +
                $"{nameof(DestTerminalType)}={DestTerminalType}, " +
                $"{nameof(MsgLength)}={MsgLength}, " +
                $"{nameof(MsgContent)}={MsgContent}, " +
                //$"{nameof(LinkId)}={LinkId?.TrimEnd('\0')}, " +
                $"{nameof(Reserve)}={Reserve?.TrimEnd('\0')}}}";
        }

        #endregion
    }
}
