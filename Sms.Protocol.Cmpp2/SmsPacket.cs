using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Sms.Protocol
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public class SmsPacket
    {

        public IMessage Head { get; set; }
        public IMessage Message { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(Head)}={Head}, {nameof(Message)}={Message}}}";
        }
    }
}
