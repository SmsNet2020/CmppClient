using System;
using System.Collections.Generic;
using Sms.Protocol.Cmpp2;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Sms.Common;
using DotNetty.Common.Internal.Logging;
using Sms.Protocol;

namespace Cmpp.Client.Codecs
{
	public class Cmpp2Encoder : MessageToMessageEncoder<SmsPacket>
	{
		readonly ClientLogger<Cmpp2Encoder> logger;
		public Cmpp2Encoder(ClientLoggerFactory loggerFactory)
		{
			logger = loggerFactory.CreateLogger<Cmpp2Encoder>();
		}

		protected override void Encode(IChannelHandlerContext context, SmsPacket message, List<object> output)
		{
			logger.Info($"send data packet:{message}", context.Channel.ToString());
			var buffer = message.Message.ToBytes();
			var TotalLength = CmppConstants.HeaderSize + buffer.Length;
			IByteBuffer byteBuffer = context.Allocator.Buffer(4)
				.WriteInt(TotalLength)
				.WriteInt((int)message.Message.GetCommandId())
				.WriteInt((int)((CmppHead)(message.Head)).SequenceId)
				.WriteBytes(buffer, 0, buffer.Length);
			output.Add(byteBuffer);
		}
	}
}
