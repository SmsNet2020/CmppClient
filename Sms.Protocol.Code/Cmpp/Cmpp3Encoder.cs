using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Sms.Common;
using Sms.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sms.Protocol.Code.Cmpp
{
    public class Cmpp3Encoder : MessageToMessageEncoder<SmsPacket>
    {
        public Cmpp3Encoder()
        {

        }

        protected override void Encode(IChannelHandlerContext context, SmsPacket message, List<object> output)
        {
            //var buffer = message.Message.ToBytes();
            //var TotalLength = CmppConstants.HeaderSize + buffer.Length;
            //output.Add(context.Allocator.Buffer(4).WriteInt(TotalLength));
            //output.Add(context.Allocator.Buffer(4).WriteInt((int)message.Message.GetCommandId()));
            //output.Add(context.Allocator.Buffer(4).WriteInt((int)message.Head.SequenceId));
            //output.Add(context.Allocator.Buffer(buffer.Length).WriteBytes(buffer));
        }
    }
}
