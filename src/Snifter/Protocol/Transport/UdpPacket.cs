using System;
using Snifter.Protocol.Internet;

namespace Snifter.Protocol.Transport
{
    /// <summary>
    /// A UDP packet, as described in RFC 768
    /// https://tools.ietf.org/html/rfc768
    ///
    ///    0                   1                   2                   3
    ///    0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
    ///   +---------------+---------------+---------------+---------------+
    ///   |          Source Port          |        Destination Port       |
    ///   +---------------+---------------+---------------+----------------
    ///   |             Length            |            Checksum           |
    ///   +---------------+---------------+---------------+----------------
    ///   |                             Data                              |
    ///   +---------------+---------------+---------------+---------------+
    /// 
    /// </summary>
    public sealed class UdpPacket : ITransportPacket, IHasPorts
    {
        public ushort SourcePort { get; }
        public ushort DestinationPort { get; }
        
        /// <summary>Total length of the packet, in bytes (includes both header and data)</summary>
        public ushort Length { get; }
        
        public ushort Checksum { get; }
        
        /// <summary>The packet payload</summary>
        public ReadOnlyMemory<byte> Payload { get; }
        
        /// <summary>The full, raw data that comprises the packet</summary>
        public ReadOnlyMemory<byte> RawData { get; }
        
        public UdpPacket(IIpPacket ipPacket)
        {
            if (ipPacket == null) throw new ArgumentNullException(nameof(ipPacket));
            if (ipPacket.Protocol != IpProtocol.UDP) throw new ArgumentOutOfRangeException(nameof(ipPacket.Protocol));

            this.RawData = ipPacket.Payload;
            var span = this.RawData.Span;
            
            this.SourcePort = span.ReadUInt16BigEndian(Offsets.SourcePort);
            this.DestinationPort = span.ReadUInt16BigEndian(Offsets.DestinationPort);
            this.Length = span.ReadUInt16BigEndian(Offsets.Length);
            this.Checksum = span.ReadUInt16BigEndian(Offsets.Checksum);
            this.Payload = this.RawData.Slice(Offsets.Payload);
        }
        
        private static class Offsets
        {
            public const int SourcePort = 0;
            public const int DestinationPort = 2;
            public const int Length = 4;
            public const int Checksum = 6;
            public const int Payload = 8;
        }
    }
}
