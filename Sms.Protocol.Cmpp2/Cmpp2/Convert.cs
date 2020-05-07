using System;
using System.Text;

namespace Sms.Protocol.Cmpp2
{
    /// <summary>
    /// 编码帮助类。
    /// </summary>
    public static class Convert
    {
        #region 公有方法
        /// <summary>
        /// 字节流解码。
        /// </summary>
        public static string ToString(byte[] buffer, int startIndex, int length, CmppEncoding encoding)
        {
            switch (encoding)
            {
                case CmppEncoding.GBK:
                    return Encoding.GetEncoding("GB2312").GetString(buffer, startIndex, length);
                case CmppEncoding.ASCII:
                    return Encoding.ASCII.GetString(buffer, startIndex, length);
                case CmppEncoding.UCS2:
                    return Encoding.BigEndianUnicode.GetString(buffer, startIndex, length);
                default:
                    return "";
            }
        }
        /// <summary>
        /// 字节流编码。
        /// </summary>
        public static byte[] ToBytes(string value, byte coding)
        {
            if (string.IsNullOrEmpty(value)) return null;
            switch (coding)
            {
                case CmppConstants.Encoding.GBK:
                    return Encoding.GetEncoding("GB2312").GetBytes(value);
                case CmppConstants.Encoding.ASCII:
                case CmppConstants.Encoding.Binary:
                    return Encoding.ASCII.GetBytes(value);
                case CmppConstants.Encoding.UCS2:
                case CmppConstants.Encoding.Special:
                case CmppConstants.Encoding.Flash:
                    return Encoding.BigEndianUnicode.GetBytes(value);
                default:
                    return null;
            }
        }

        public static byte[] ToBytes(string value, byte encoding, int byteLen)
        {
            if (null == value)
            {
                return new byte[byteLen];
            }
            Encoding encode;
            switch (encoding)
            {
                case CmppConstants.Encoding.GBK:
                    encode = Encoding.GetEncoding("GB2312");
                    break;
                case CmppConstants.Encoding.ASCII:
                    encode = Encoding.ASCII;
                    break;
                case CmppConstants.Encoding.UCS2:
                    encode = Encoding.BigEndianUnicode;
                    break;
                default:
                    return null;
            }
            var buffer = new byte[byteLen];
            var bytes = encode.GetBytes(value);
            Array.Copy(bytes, 0, buffer, 0, Math.Min(bytes.Length, byteLen));
            return buffer;
        }
        /// <summary>
        /// 计算字符串长度（该长度为转换为字节流后的长度）。
        /// </summary>
        public static byte Length(string value, byte coding)
        {
            var buffer = ToBytes(value, coding);
            return (byte)(buffer == null ? 0 : buffer.Length);
        }
        /// <summary>
        /// 字节流编码。
        /// </summary>
        public static byte[] ToBytes(uint value)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            return bytes;
        }
        /// <summary>
        /// 字节流编码。
        /// </summary>
        public static byte[] ToBytes(ulong value)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            return bytes;
        }
        /// <summary>
        /// 字节流解码。
        /// </summary>
        public static uint ToUInt(byte[] bytes, int index)
        {
            Array.Reverse(bytes, index, 1);
            return System.Convert.ToUInt32(bytes[index]);
        }
        /// <summary>
        /// 字节流解码。
        /// </summary>
        public static uint ToUInt32(byte[] bytes, int index)
        {
            Array.Reverse(bytes, index, 4);
            return BitConverter.ToUInt32(bytes, index);
        }
        /// <summary>
        /// 字节流解码。
        /// </summary>
        public static ulong ToUInt64(byte[] bytes, int index)
        {
            Array.Reverse(bytes, index, 8);
            return BitConverter.ToUInt64(bytes, index);
        }
        /// <summary>
        /// 字节流解码。
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <param name="position">bytes读取的位置</param>
        /// <param name="index">position</param>
        /// <param name="byteLen">一个string的byte长度</param>
        /// <param name="number">数量</param>
        /// <returns></returns>
        public static string[] ToStringArray(byte[] bytes,int position, out int index, int byteLen, int number)
        {
            index = position;
            string[] strArray = new string[number];
            for (int i = 0; i < number; i++)
            {
                strArray[i] = Convert.ToString(bytes, position, byteLen, CmppEncoding.ASCII);
                index += byteLen;
            }

            return strArray;
        }
        #endregion
    }
}
