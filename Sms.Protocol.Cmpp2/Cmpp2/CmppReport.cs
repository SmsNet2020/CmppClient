using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Sms.Protocol.Cmpp2
{
    /// <summary>
    /// ISMG 向 SP 送交的状态报告（只有在 CMPP_SUBMIT 中的 RegisteredDelivery 被设置为1时，ISMG才会向SP发送状态报告）。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct CmppReport : IMessage
    {
        #region 字段
        /// <summary>
        /// 信息标识；SP提交短信（CMPP_SUBMIT）操作时，与SP相连的ISMG产生的Msg_Id。
        /// </summary>
        public ulong MsgId { get; set; }
        /// <summary>
        /// 发送短信的应答结果，含义详见表一（SP根据该字段确定CMPP_SUBMIT消息的处理状态）。
        /// </summary>
        public string Stat { get; set; }
        /// <summary>
        /// YYMMDDHHMM（YY为年的后两位00-99，MM：01-12，DD：01-31，HH：00-23，MM：00-59）。
        /// </summary>
        public string SubmitTime { get; set; }
        /// <summary>
        /// YYMMDDHHMM。
        /// </summary>
        public string DoneTime { get; set; }
        /// <summary>
        /// 目的终端MSISDN号码（SP发送CMPP_SUBMIT消息的目标终端）。
        /// </summary>
        public string DestTerminalId { get; set; }
        /// <summary>
        /// 取自SMSC发送状态报告的消息体中的消息标识。
        /// </summary>
        public uint SmscSequence { get; set; }
        #endregion

        public uint GetCommandId()
        {
            throw new NotImplementedException();
        }

        public byte[] ToBytes()
        {
            var buffer = new List<byte>(CmppConstants.PackageBodySize.CmppReport);
            buffer.AddRange(Convert.ToBytes(MsgId));
            buffer.AddRange(Convert.ToBytes(Stat, CmppConstants.Encoding.ASCII, 7));
            buffer.AddRange(Convert.ToBytes(SubmitTime, CmppConstants.Encoding.ASCII, 10));
            buffer.AddRange(Convert.ToBytes(DoneTime, CmppConstants.Encoding.ASCII, 10));
            buffer.AddRange(Convert.ToBytes(DestTerminalId, CmppConstants.Encoding.ASCII, 21));
            buffer.AddRange(Convert.ToBytes(SmscSequence));//4
            return buffer.ToArray();
        }

        public void FromBytes(byte[] buffer)
        {
			if (buffer == null || buffer.Length != CmppConstants.PackageBodySize.CmppReport)
			{
				throw new ArgumentException(string.Format("Invalid bytes to unmarshal for {0}.", GetType().Name));
			}
            var position = 0;
            MsgId = Convert.ToUInt64(buffer, 0);
            position += 8;

            Stat = Convert.ToString(buffer, position, 7, CmppEncoding.ASCII).TrimEnd('\0');
            position += 7;

            SubmitTime = Convert.ToString(buffer, position, 10, CmppEncoding.ASCII).TrimEnd('\0');
            position += 10;

            DoneTime = Convert.ToString(buffer, position, 10, CmppEncoding.ASCII).TrimEnd('\0');
            position += 10;

            DestTerminalId = Convert.ToString(buffer, position, 21, CmppEncoding.ASCII).TrimEnd('\0');
            position += 21;

            SmscSequence = Convert.ToUInt32(buffer, position);
        }

        public override string ToString()
        {
            return $"{{{nameof(MsgId)}={MsgId.ToString()}, " +
                $"{nameof(Stat)}={Stat}, " +
                $"{nameof(SubmitTime)}={SubmitTime}, " +
                $"{nameof(DoneTime)}={DoneTime}, " +
                $"{nameof(DestTerminalId)}={DestTerminalId}, " +
                $"{nameof(SmscSequence)}={SmscSequence.ToString()}}}";
        }
    }
}
