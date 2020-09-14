using System;
using Snifter.Protocol.Internet;

namespace Snifter.Protocol.Transport
{
    /// <summary>
    /// An ICMP packet, as described in RFC 792
    /// https://tools.ietf.org/html/rfc792
    ///
    ///    0                   1                   2                   3
    ///    0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
    ///   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///   |     Type      |     Code      |          Checksum             |
    ///   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///   |                    Value, depending on Type                   |
    ///   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///   |                            Payload                            |
    ///   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    /// 
    /// </summary>
    public sealed class IcmpPacket : ITransportPacket
    {
        public ushort Checksum { get; }
        
        /// <summary>The packet payload</summary>
        public ReadOnlyMemory<byte> Payload { get; }
        
        /// <summary>The full, raw data that comprises the packet</summary>
        public ReadOnlyMemory<byte> RawData { get; }
        
        public IcmpPacket(IIpPacket ipPacket)
        {
            if (ipPacket == null) throw new ArgumentNullException(nameof(ipPacket));
            if (ipPacket.Protocol != IpProtocol.ICMP) throw new ArgumentOutOfRangeException(nameof(ipPacket.Protocol));

            this.RawData = ipPacket.Payload;
            var span = this.RawData.Span;
            
            // TODO: Parse Type, Code and Value
            
            this.Checksum = span.ReadUInt16BigEndian(Offsets.Checksum);
            this.Payload = this.RawData.Slice(Offsets.Payload);
        }
        
        private static class Offsets
        {
            public const int Type = 0;
            public const int Code = 2;
            public const int Checksum = 16;
            public const int Value = 32;
            public const int Payload = 64;
        }
    }
}
