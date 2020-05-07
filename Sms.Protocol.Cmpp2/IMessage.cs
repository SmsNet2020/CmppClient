namespace Sms.Protocol
{
    /// <summary>
    /// Cmpp message interface.
    /// </summary>
    public interface IMessage
    {

        /// <summary>
        /// Get command id of this message.
        /// </summary>
        /// <returns></returns>
        uint GetCommandId();
        /// <summary>
        /// Convert message to bytes.
        /// </summary>
        /// <returns></returns>
        byte[] ToBytes();
        /// <summary>
        /// Restore message from byte stream.
        /// </summary>
        /// <param name="body"></param>
        void FromBytes(byte[] body);
    }
}
