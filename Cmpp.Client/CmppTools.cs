using DotNetty.Common.Internal.Logging;
using DotNetty.Transport.Channels;
using Sms.Common;
using Sms.Protocol;
using Sms.Protocol.Cmpp2;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cmpp.Client
{
	public class CmppTools
	{
		private const int MaxMessageByteLength = 140;
		private const int LongMessageHeadLength = 6;
		public static uint HeaderLength = 12;//包头长
		public static uint HeaderMaxSeqID = 0x7FFFFFFE;//包头最大序列号
		public static uint MsgIdMaxSeqID = 0xFFFF;//msgId最大序列号
		public static int Sequence_Id = 0; //包头流水号
		public static int MsgSequenceId = 0; //msgId流水号
		public static int LongSmsSequenceId = 0; //长消息流水号
		public static int LongSmsMaxSeqID = 256;//长消息流水号最大序列号

		static object GetHeadSequence_IdLock = new object();
		static object GetLongSmsSequence_IdLock = new object();


		/// <summary>
		/// 取序号
		/// </summary>
		/// <returns></returns>
		public static uint GetHeadSequenceId()
		{
			lock (GetHeadSequence_IdLock)
			{
				if (Sequence_Id >= HeaderMaxSeqID)
				{
					Interlocked.CompareExchange(ref Sequence_Id, 0, (int)HeaderMaxSeqID); //原子操作，a参数和c参数比较，  相等b替换a，不相等不替换。
				}

				Interlocked.Increment(ref Sequence_Id);//线程安全，类似于LOCK，但要比LOCK性能高
				return (uint)Sequence_Id;
			}
		}

		/// <summary>
		/// 长短信取序号
		/// </summary>
		/// <returns></returns>
		public static byte GetLongSmsSequenceId()
		{
			lock (GetLongSmsSequence_IdLock)
			{
				//if (GlobalModel.LongSmsSequenceId >= GlobalModel.LongSmsMaxSeqID)
				//{
				Interlocked.CompareExchange(ref LongSmsSequenceId, 0, LongSmsMaxSeqID); //原子操作，a参数和c参数比较，  相等b替换a，不相等不替换。

				Interlocked.Increment(ref LongSmsSequenceId);//线程安全，类似于LOCK，但要比LOCK性能高 
			}
			return (byte)LongSmsSequenceId;
		}

		public static uint GestCmppHeadTimeStamp(DateTime date)
		{
			return uint.Parse(string.Format("{0:MMddHHmmss}", date));
		}

		public static uint GestCmppHeadTimeStamp()
		{
			return GestCmppHeadTimeStamp(DateTime.Now);
		}

		/// <summary>
		/// 计算 CMPP_CONNECT 包的 AuthenticatorSource 字段。
		/// </summary>
		/// <remarks>
		/// MD5(Source_Addr + 9字节的0 + shared secret + timestamp);
		/// </remarks>
		public static byte[] CreateAuthenticatorSource(DateTime timestamp, string username, string password)
		{
			var btContent = new byte[9 + username.Length + password.Length + 10];
			Array.Clear(btContent, 0, btContent.Length);

			// Source_Addr，SP的企业代码（6位）。
			var iPos = 0;
			foreach (var ch in username)
			{
				btContent[iPos] = (byte)ch;
				iPos++;
			}

			// 9字节的0。
			iPos += 9;

			// password，由 China Mobile 提供（长度不固定）。
			foreach (var ch in password)
			{
				btContent[iPos] = (byte)ch;
				iPos++;
			}

			// 时间戳（10位）。
			foreach (var ch in string.Format("{0:MMddHHmmss}", timestamp))
			{
				btContent[iPos] = (byte)ch;
				iPos++;
			}
			return new MD5CryptoServiceProvider().ComputeHash(btContent);
		}

		/// <summary>
		/// 用于鉴别ISMG。其值通过单向MD5 hash计算得出，表示如下：
		/// AuthenticatorISMG =MD5（Status+AuthenticatorICP+ Tls_available+shared secret）
		/// </summary>
		/// <param name="status"></param>
		/// <param name="AuthenticatorSource"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public static byte[] CreateAuthenticatorISMG(byte status, byte[] AuthenticatorSource, string password)
		{

			var btContent = new byte[17 + password.Length];
			Array.Clear(btContent, 0, btContent.Length);

			//status(1字节)
			var iPos = 0;
			btContent[iPos] = status;
			iPos++;

			//AuthenticatorSource(16字节)
			foreach (byte temp in AuthenticatorSource)
			{
				btContent[iPos] = temp;
				iPos++;
			}

			// password，由 China Mobile 提供（长度不固定）。
			foreach (var ch in password)
			{
				btContent[iPos] = (byte)ch;
				iPos++;
			}
			return new MD5CryptoServiceProvider().ComputeHash(btContent);
		}

		public static List<string> SplitLongMessage(string content)
		{
			var result = new List<string>();
			var messageUCS2 = Encoding.BigEndianUnicode.GetBytes(content);
			var messageUCS2Len = messageUCS2.Length;

			if (messageUCS2Len > MaxMessageByteLength)
			{
				// long message amount
				var messageUCS2Count = (messageUCS2Len - 1) / (MaxMessageByteLength - LongMessageHeadLength) + 1;
				var tpUdhiHead = new byte[LongMessageHeadLength];
				tpUdhiHead[0] = 0x05;
				tpUdhiHead[1] = 0x00;
				tpUdhiHead[2] = 0x03;
				tpUdhiHead[3] = CmppTools.GetLongSmsSequenceId();//(byte)(_sequenceId % 256);
				tpUdhiHead[4] = (byte)(messageUCS2Count);

				for (var i = 0; i < messageUCS2Count; i++)
				{
					// message sequence
					tpUdhiHead[5] = (byte)(i + 1);

					byte[] msgContent;
					if (i != messageUCS2Count - 1)
					{
						// not the last message
						msgContent = new byte[MaxMessageByteLength];
						Array.Copy(tpUdhiHead, msgContent, LongMessageHeadLength);
						Array.Copy(messageUCS2, i * (MaxMessageByteLength - LongMessageHeadLength), msgContent, LongMessageHeadLength, MaxMessageByteLength - LongMessageHeadLength);
					}
					else
					{
						// the last message
						msgContent = new byte[tpUdhiHead.Length + messageUCS2Len - i * (MaxMessageByteLength - LongMessageHeadLength)];
						Array.Copy(tpUdhiHead, msgContent, LongMessageHeadLength);
						Array.Copy(messageUCS2, i * (MaxMessageByteLength - LongMessageHeadLength), msgContent, LongMessageHeadLength, messageUCS2Len - i * (MaxMessageByteLength - LongMessageHeadLength));
					}
					result.Add(Encoding.BigEndianUnicode.GetString(msgContent));
				}
			}
			else
			{
				result.Add(content);
			}
			return result;
		}

		internal static async Task SendAsync(IChannel channel, SmsPacket packet)
		{
			if (!channel.Active)
			{
				throw new Exception($"channel未激活:{channel.ToString()}, data:{packet}");
			}

			//等待水位下降
			while (!channel.IsWritable)
			{
				await Task.Delay(5);
			}

			await channel.WriteAndFlushAsync(packet);
		}

		internal static Task SendAsync(IChannelHandlerContext context, SmsPacket packet)
		{
			return SendAsync(context.Channel, packet);
		}



		internal static SmsPacket GroupPacket(IMessage message)
		{
			var sequenceId = CmppTools.GetHeadSequenceId();
			return GroupPacket(message, sequenceId);
		}

		internal static SmsPacket GroupPacket(IMessage message, uint sequenceId)
		{
			var smsPacket = new SmsPacket
			{
				Head = new CmppHead
				{
					SequenceId = sequenceId,
					CommandId = message.GetCommandId()
				},
				Message = message
			};
			return smsPacket;
		}
	}
}
