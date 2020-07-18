using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Common.Internal.Logging;
using DotNetty.Transport.Channels;
using Sms.Common;
using Sms.Protocol.Cmpp2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sms.Protocol.Code.Cmpp
{
	public class Cmpp2Decoder : LengthFieldBasedFrameDecoder
	{
        readonly ClientLogger<Cmpp2Decoder> logger;
        public Cmpp2Decoder(ClientLoggerFactory loggerFactory, int maxFrameLength, int lengthFieldOffset, int lengthFieldLength) : base(maxFrameLength, lengthFieldOffset, lengthFieldLength)
		{
            this.logger = loggerFactory.CreateLogger<Cmpp2Decoder>();
        }

		public Cmpp2Decoder(ClientLoggerFactory loggerFactory, int maxFrameLength, int lengthFieldOffset, int lengthFieldLength, int lengthAdjustment, int initialBytesToStrip) : base(maxFrameLength, lengthFieldOffset, lengthFieldLength, lengthAdjustment, initialBytesToStrip)
		{
            this.logger = loggerFactory.CreateLogger<Cmpp2Decoder>();
        }

		public Cmpp2Decoder(ClientLoggerFactory loggerFactory, int maxFrameLength, int lengthFieldOffset, int lengthFieldLength, int lengthAdjustment, int initialBytesToStrip, bool failFast) : base(maxFrameLength, lengthFieldOffset, lengthFieldLength, lengthAdjustment, initialBytesToStrip, failFast)
		{
            this.logger = loggerFactory.CreateLogger<Cmpp2Decoder>();
        }

		public Cmpp2Decoder(ClientLoggerFactory loggerFactory, ByteOrder byteOrder, int maxFrameLength, int lengthFieldOffset, int lengthFieldLength, int lengthAdjustment, int initialBytesToStrip, bool failFast) : base(byteOrder, maxFrameLength, lengthFieldOffset, lengthFieldLength, lengthAdjustment, initialBytesToStrip, failFast)
		{
            this.logger = loggerFactory.CreateLogger<Cmpp2Decoder>();
        }

		protected override object Decode(IChannelHandlerContext context, IByteBuffer input)
		{
            var channelInfo = context.Channel.ToString();
            var obj = base.Decode(context, input);
			if (AcceptInboundMessage(obj))
			{
				IByteBuffer msg = (IByteBuffer)obj;
				byte[] array = new byte[msg.ReadableBytes];
				msg.GetBytes(0, array);
				var cmppHeader = new CmppHead().FromBytes0(array);

                if (cmppHeader.TotalLength == array.Length)
                {
                    object args = null;
                    switch (cmppHeader.CommandId)
                    {
                        case CmppConstants.CommandCode.Connect:

                            break;
                        case CmppConstants.CommandCode.ConnectResp:
                            var ConnectResp = new CmppConnectResp().FromBytes0(array);
                            args = new CmppMessageReceiveArgs<CmppHead, CmppConnectResp>(cmppHeader, ConnectResp);
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
                            args = new CmppMessageReceiveArgs<CmppHead, CmppSubmitResp>(cmppHeader, SubmitResp);
                            break;
                        case CmppConstants.CommandCode.Deliver:
                            var Deliver = new CmppDeliver().FromBytes0(array);
                            args = new CmppMessageReceiveArgs<CmppHead, CmppDeliver>(cmppHeader, Deliver);
                            break;
                        case CmppConstants.CommandCode.DeliverResp:
                            //var DeliverResp = new Cmpp2.CmppDeliverResp();
                            //DeliverResp.FromBytes(packet);
                            break;
                        case CmppConstants.CommandCode.ActiveTest:
                            var ActiveTest = new CmppActiveTest().FromBytes0(array);
                            args = new CmppMessageReceiveArgs<CmppHead, CmppActiveTest>(cmppHeader, ActiveTest);
                            break;
                        case CmppConstants.CommandCode.ActiveTestResp:
                            //var ActiveTestResp = new CmppActiveTestResp().FromBytes0(array);
                            //args = new CmppMessageReceiveArgs<CmppHead, CmppActiveTestResp>(cmppHeader, ActiveTestResp);
                            break;

                        default:
                            throw new NotSupportedException(cmppHeader.CommandId.ToString());
                    }
                    logger.Info($"receive cmpp data:{args?.ToString()} cmd:{cmppHeader.CommandId}", channelInfo);
                    return args;
                }
                else
                {
                    throw new InvalidDataException("The received cmpp package is invalid");
                }
            }

            logger.Info($"receive unknown data:{obj?.ToString()}", channelInfo);
            return obj;
		}

		public bool AcceptInboundMessage(object msg) => msg is IByteBuffer;
	}
}
