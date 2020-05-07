using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Sms.Common;
using Sms.Protocol;
using Sms.Protocol.Cmpp2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Cmpp.Client.Codecs
{
	public class Cmpp3Decoder : LengthFieldBasedFrameDecoder
	{
		public Cmpp3Decoder(int maxFrameLength, int lengthFieldOffset, int lengthFieldLength) : base(maxFrameLength, lengthFieldOffset, lengthFieldLength)
		{
		}

		public Cmpp3Decoder(int maxFrameLength, int lengthFieldOffset, int lengthFieldLength, int lengthAdjustment, int initialBytesToStrip) : base(maxFrameLength, lengthFieldOffset, lengthFieldLength, lengthAdjustment, initialBytesToStrip)
		{
		}

		public Cmpp3Decoder(int maxFrameLength, int lengthFieldOffset, int lengthFieldLength, int lengthAdjustment, int initialBytesToStrip, bool failFast) : base(maxFrameLength, lengthFieldOffset, lengthFieldLength, lengthAdjustment, initialBytesToStrip, failFast)
		{
		}

		public Cmpp3Decoder(ByteOrder byteOrder, int maxFrameLength, int lengthFieldOffset, int lengthFieldLength, int lengthAdjustment, int initialBytesToStrip, bool failFast) : base(byteOrder, maxFrameLength, lengthFieldOffset, lengthFieldLength, lengthAdjustment, initialBytesToStrip, failFast)
		{
		}

		protected override object Decode(IChannelHandlerContext context, IByteBuffer input)
		{
			var obj = base.Decode(context, input);
			if (AcceptInboundMessage(obj))
			{
				IByteBuffer msg = (IByteBuffer)obj;
				byte[] array = new byte[msg.ReadableBytes];
				msg.GetBytes(0, array);
				var cmppHeader = new CmppHead().FromBytes0(array);

                if (cmppHeader.TotalLength == array.Length)
                {
                    CmppMessageReceiveArgs<IMessage, IMessage> args = null;
                    switch (cmppHeader.CommandId)
                    {
                        case CmppConstants.CommandCode.Connect:

                            break;
                        case CmppConstants.CommandCode.ConnectResp:
                            var ConnectResp = new CmppConnectResp().FromBytes0(array);
                            args = new CmppMessageReceiveArgs<IMessage, IMessage>(cmppHeader, ConnectResp);
                            break;
                        case CmppConstants.CommandCode.Terminate:
                            //var Terminate = new CmppTerminate();
                            //Terminate.FromBytes(packet);
                            break;
                        case CmppConstants.CommandCode.TerminateResp:
                            //var TerminateResp = new CmppTerminateResp();
                            //TerminateResp.FromBytes(packet);
                            break;
                        case CmppConstants.CommandCode.Submit:
                            //var Submit = new Cmpp2.CmppSubmit();
                            //Submit.FromBytes(packet);
                            break;
                        case CmppConstants.CommandCode.SubmitResp:
                            var SubmitResp = new CmppSubmitResp().FromBytes0(array);
                            args = new CmppMessageReceiveArgs<IMessage, IMessage>(cmppHeader, SubmitResp);
                            break;
                        case CmppConstants.CommandCode.Deliver:
                            var Deliver = new CmppDeliver().FromBytes0(array);
                            args = new CmppMessageReceiveArgs<IMessage, IMessage>(cmppHeader, Deliver);
                            break;
                        case CmppConstants.CommandCode.DeliverResp:
                            //var DeliverResp = new Cmpp2.CmppDeliverResp();
                            //DeliverResp.FromBytes(packet);
                            break;
                        case CmppConstants.CommandCode.ActiveTest:
                            var ActiveTest = new CmppActiveTest().FromBytes0(array);
                            args = new CmppMessageReceiveArgs<IMessage, IMessage>(cmppHeader, ActiveTest);
                            break;
                        case CmppConstants.CommandCode.ActiveTestResp:
                            var ActiveTestResp = new CmppActiveTestResp().FromBytes0(array);
                            args = new CmppMessageReceiveArgs<IMessage, IMessage>(cmppHeader, ActiveTestResp);
                            break;

                        default:
                            throw new NotSupportedException(cmppHeader.CommandId.ToString());
                    }
                    return args;
                }
                else
                {
                    throw new InvalidDataException("The received cmpp package is invalid");
                }
            }
			return obj;
		}

		public bool AcceptInboundMessage(object msg) => msg is IByteBuffer;
	}
}
