using System;
using Snifter.Protocol.Internet;

namespace Snifter.Protocol.Transport
{
    /// <summary>
    /// A TCP packet, as described in RFC 793
    /// https://tools.ietf.org/html/rfc793
    ///
    ///    0                   1                   2                   3
    ///    0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
    ///   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///   |          Source Port          |       Destination Port        |
    ///   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///   |                        Sequence Number                        |
    ///   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///   |                    Acknowledgment Number                      |
    ///   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///   |  Data | Res |N|C|E|U|A|P|R|S|F|                               |
    ///   | Offset| erv |S|W|C|R|C|S|S|Y|I|            Window             |
    ///   |       | ed  | |R|E|G|K|H|T|N|N|                               |
    ///   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///   |           Checksum            |         Urgent Pointer        |
    ///   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///   |                    Options                    |    Padding    |
    ///   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///   |                             Data                              |
    ///   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    /// 
    /// </summary>
    public sealed class TcpPacket : ITransportPacket, IHasPorts
    {
        public ushort SourcePort { get; }
        public ushort DestinationPort { get; }
        
        /// <summary>
        /// The sequence number of the first data octet in this segment (except when SYN is present).
        /// If SYN is present the sequence number is the initial sequence number (ISN) and the first data octet is ISN+1.
        /// </summary>
        public uint SequenceNumber { get; }
        
        /// <summary>
        /// If the ACK control bit is set, this field contains the value of the next sequence number that the sender
        /// of the segment is expecting to receive. Once a connection is established this is always sent.
        /// </summary>
        public uint AcknowledgmentNumber { get; }
        
        /// <summary>Length of the TCP header, in bytes. This indicates where the data begins</summary>
        public ushort DataOffset { get; }
        
        /// <summary>Control flags</summary>
        public TcpControlFlags ControlFlags { get; }
        
        /// <summary>
        /// The number of data octets, beginning with the one indicated in the acknowledgment field, which the sender
        /// of this segment is willing to accept
        /// </summary>
        public ushort Window { get; }
        public ushort Checksum { get; }
        
        /// <summary>
        /// The current value of the urgent pointer as a positive offset from the sequence number in this segment.
        /// The urgent pointer points to the sequence number of the octet following the urgent data.
        /// This field is only be interpreted in segments with the URG control bit set.
        /// </summary>
        public ushort UrgentPointer { get; }

        /// <summary>The packet payload</summary>
        public ReadOnlyMemory<byte> Payload { get; }
        
        /// <summary>The full, raw data that comprises the packet</summary>
        public ReadOnlyMemory<byte> RawData { get; }
        
        public TcpPacket(IIpPacket ipPacket)
        {
            if (ipPacket == null) throw new ArgumentNullException(nameof(ipPacket));
            if (ipPacket.Protocol != IpProtocol.TCP) throw new ArgumentOutOfRangeException(nameof(ipPacket.Protocol));
            
            this.RawData = ipPacket.Payload;
            var span = this.RawData.Span;
            
            this.SourcePort = span.ReadUInt16BigEndian(Offsets.SourcePort);
            this.DestinationPort = span.ReadUInt16BigEndian(Offsets.DestinationPort);
            this.SequenceNumber = span.ReadUInt32BigEndian(Offsets.SequenceNumber);
            this.AcknowledgmentNumber = span.ReadUInt32BigEndian(Offsets.AcknowledgmentNumber);
            
            // ushort containing Data Offset (aka Header Length) in the first 4 bits, and Control Flags in the following 12 bits
            var dataOffsetAndFlags = span.ReadUInt16BigEndian(Offsets.DataOffsetAndFlags);
            
            // Data Offset (aka Header Length) is encoded as the number of 32-bit words (4 bytes)
            this.DataOffset = (ushort)(BinaryHelper.ReadBits(dataOffsetAndFlags, 0, 4) * 4);
            this.Payload = this.RawData.Slice(this.DataOffset);

            this.ControlFlags = (TcpControlFlags)BinaryHelper.ReadBits(dataOffsetAndFlags, 4, 12);
            
            this.Window = span.ReadUInt16BigEndian(Offsets.Window);
            this.Checksum = span.ReadUInt16BigEndian(Offsets.Checksum);
            this.UrgentPointer = span.ReadUInt16BigEndian(Offsets.UrgentPointer);
            
            // TODO: Parse Options
        }
        
        // Byte offsets
        private static class Offsets
        {
            public const int SourcePort = 0;
            public const int DestinationPort = 2;
            public const int SequenceNumber = 4;
            public const int AcknowledgmentNumber = 8;
            public const int DataOffsetAndFlags = 12;
            public const int Window = 14;
            public const int Checksum = 16;
            public const int UrgentPointer = 18;
            public const int Options = 20;
        }
    }
}
