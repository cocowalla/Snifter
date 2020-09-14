
// ReSharper disable CommentTypo
// ReSharper disable InconsistentNaming
namespace Snifter.Protocol.Transport
{
    /// <summary>
    /// TCP Control flags, as specified in RFC 793
    /// </summary>
    public enum TcpControlFlags : ushort
    {
        /// <summary>No control flags are set</summary>
        None = 0x0000,

        /// <summary>No more data from sender</summary>
        FIN = 0x0001,

        /// <summary>Synchronize sequence numbers</summary>
        SYN = 0x0002,

        /// <summary>Reset the connection</summary>
        RST = 0x0004,

        /// <summary>Push Function</summary>
        PSH = 0x0008,
        
        /// <summary>Acknowledgment field significant</summary>
        ACK = 0x0010,

        /// <summary>Urgent Pointer field significant</summary>
        URG = 0x0020,

        /// <summary>Explicit congestion notification echo. Added to the IPv4 spec in RFC 3168</summary>
        ECE = 0x0040,

        /// <summary>
        /// Congestion Window Reduced. Added to the IPv4 spec in RFC 3168
        /// </summary>
        CWR = 0x0080,

        /// <summary>Nonce sum.  Added to the IPv4 spec in RFC 3540</summary>
        NS = 0x0100
    }
}
