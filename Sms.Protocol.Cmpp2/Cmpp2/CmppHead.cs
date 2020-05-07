﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Sms.Protocol.Cmpp2
{
    /// <summary>
    /// 消息头格式（Message Header）。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct CmppHead : IMessage
    {
        #region 字段
        /// <summary>
        /// 消息总长度(含消息头及消息体)。
        /// </summary>
        public uint TotalLength { get; set; }
        /// <summary>
        /// 命令或响应类型。
        /// </summary>
        public uint CommandId { get; set; }
        /// <summary>
        /// 消息流水号,顺序累加,步长为1,循环使用（每对请求和应答消息的流水号必须相同）。
        /// </summary>
        public uint SequenceId { get; set; }
        #endregion

        public uint GetCommandId()
        {
            throw new NotImplementedException();
        }

        public byte[] ToBytes()
        {
            var buffer = new List<byte>(CmppConstants.HeaderSize);
            buffer.AddRange(Convert.ToBytes(TotalLength));
            buffer.AddRange(Convert.ToBytes(CommandId));
            buffer.AddRange(Convert.ToBytes(SequenceId));
            return buffer.ToArray();
        }

        public void FromBytes(byte[] body)
        {
            TotalLength = Convert.ToUInt32(body, 0);
            CommandId = Convert.ToUInt32(body, 4);
            SequenceId = Convert.ToUInt32(body, 8);
        }

        public CmppHead FromBytes0(byte[] body)
        {
            FromBytes(body);
            return this;
        }

        public override string ToString()
        {
            return $"{{{nameof(TotalLength)}={TotalLength}, {nameof(CommandId)}={CommandId}, {nameof(SequenceId)}={SequenceId}}}";
        }
    }
}
