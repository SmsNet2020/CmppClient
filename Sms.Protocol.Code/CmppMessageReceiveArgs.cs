using System;
using System.Collections.Generic;
using System.Text;

namespace Sms.Protocol.Code
{
    public class CmppMessageReceiveArgs<ICmppHead, ICmppMessage> 
    {
        /// <summary>
        /// Received header
        /// </summary>
        public ICmppHead Header { get; set; }
        /// <summary>
        /// Received message body
        /// </summary>
        public ICmppMessage Message { get; set; }
        public object Item { get; set; }

        public CmppMessageReceiveArgs(ICmppHead _header, ICmppMessage _message)
        {
            Header = _header;
            Message = _message;
        }

        public CmppMessageReceiveArgs(ICmppHead _header, ICmppMessage _message, object obj)
        {
            Header = _header;
            Message = _message;
            Item = obj;
        }

        public CmppMessageReceiveArgs()
        {
        }

        public override string ToString()
        {
            return $"{{{nameof(Header)}={Header}, {nameof(Message)}={Message}, {nameof(Item)}={Item}}}";
        }
    }

    public class CmppMessageReceiveArgs<ICmppHead, ICmppMessage,ExData>
    {
        /// <summary>
        /// Received header
        /// </summary>
        public ICmppHead Header { get; set; }
        /// <summary>
        /// Received message body
        /// </summary>
        public ICmppMessage Message { get; set; }
        public ExData Item { get; set; }

        public CmppMessageReceiveArgs(ICmppHead _header, ICmppMessage _message)
        {
            Header = _header;
            Message = _message;
        }

        public CmppMessageReceiveArgs(ICmppHead _header, ICmppMessage _message, ExData obj)
        {
            Header = _header;
            Message = _message;
            Item = obj;
        }

        public CmppMessageReceiveArgs()
        {
        }

        public override string ToString()
        {
            return $"{{{nameof(Header)}={Header}, {nameof(Message)}={Message}, {nameof(Item)}={Item}}}";
        }
    }
}
