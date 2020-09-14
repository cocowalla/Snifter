using System;
using System.Net;
using Snifter.Protocol.Transport;

namespace Snifter.Protocol.Internet
{
    /// <summary>
    /// An IPv4 packet, as described in RFC 791
    /// https://tools.ietf.org/html/rfc791
    /// 
    ///    0                   1                   2                   3
    ///    0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
    ///   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///   |Version|  IHL  |Type of Service|          Total Length         |
    ///   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///   |         Identification        |Flags|      Fragment Offset    |
    ///   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///   |  Time to Live |    Protocol   |         Header Checksum       |
    ///   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///   |                       Source Address                          |
    ///   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///   |                    Destination Address                        |
    ///   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///   |                    Options                    |    Padding    |
    ///   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    /// 
    /// </summary>
    public sealed class IpV4Packet : IIpPacket
    {
        public DateTime CaptureTime { get; }

        public IpVersion Version { get; } = IpVersion.Ipv4;
        
        /// <summary>Length of the IP header, in bytes</summary>
        public ushort HeaderLength { get; }
        public byte TypeOfService { get; }
        
        /// <summary>
        /// The total length of the packet, including header and payload, as stated in the Total Length field of the packet header 
        /// </summary>
        public ushort TotalLength { get; }
        
        public ushort Identification { get; }
        public FragmentationFlags FragmentationFlags { get; }
        public ushort FragmentOffset { get; }
        
        public byte TimeToLive { get; }
        public IpProtocol Protocol { get; }
        public ushort HeaderChecksum { get; }
        
        public IPAddress SourceAddress { get; }
        public IPAddress DestinationAddress { get; }
        
        /// <summary>Transport-layer packet contained within the payload</summary>
        public ITransportPacket TransportPacket { get; private set; }
        
        /// <summary>The packet payload</summary>
        public ReadOnlyMemory<byte> Payload { get; }
        
        /// <summary>The full, raw data that comprises the packet</summary>
        public ReadOnlyMemory<byte> RawData { get; }
            
        public IpV4Packet(ReadOnlyMemory<byte> data, DateTime? captureTime = null)
        {
            this.CaptureTime = captureTime ?? DateTime.UtcNow;
            
            this.RawData = data;
            var span = this.RawData.Span;
            
            // First byte contains both the IP Version and Header Length
            var versionAndLength = span[Offsets.VersionAndHeaderLength];
            var version = BinaryHelper.ReadBits(versionAndLength, 0, 4);

            // This is an IPv4 packet
            if (version != 4)
                return;

            // IHL is encoded as the number of 32-bit words (4 bytes)
            this.HeaderLength = (ushort)(BinaryHelper.ReadBits(versionAndLength, 4, 4) * 4);
            this.Payload = this.RawData.Slice(this.HeaderLength);
            
            this.TypeOfService = span[Offsets.TypeOfService];
            this.TotalLength = span.ReadUInt16BigEndian(Offsets.TotalLength);
            
            this.Identification = span.ReadUInt16BigEndian(Offsets.Identification);
            
            // ushort containing flags in the first 3 bits, and offset in the following 13 bits
            var flagsAndOffset = span.ReadUInt16BigEndian(Offsets.FlagsAndOffset);
            this.FragmentationFlags = (FragmentationFlags)BinaryHelper.ReadBits(flagsAndOffset, 0, 3);
            this.FragmentOffset = BinaryHelper.ReadBits(flagsAndOffset, 3, 13);
            
            this.TimeToLive = span[Offsets.Ttl];
            this.Protocol = (IpProtocol)span[Offsets.Protocol];
            this.HeaderChecksum = span.ReadUInt16BigEndian(Offsets.HeaderChecksum);
            
            this.SourceAddress = IPHelper.ReadIPv4Address(span.Slice(Offsets.SourceAddress, sizeof(uint)));
            this.DestinationAddress = IPHelper.ReadIPv4Address(span.Slice(Offsets.DestinationAddress, sizeof(uint)));

            this.TransportPacket = new RawPacket(this);
        }

        /// <summary>
        /// Parse the payload into a Transport Packet
        /// </summary>
        /// <param name="parser"></param>
        public void ParseTransportPacket(TransportPacketParser parser)
        {
            this.TransportPacket = parser.Parse(this);
        }

        private static class Offsets
        {
            public const int VersionAndHeaderLength = 0;
            public const int TypeOfService = 1;
            public const int TotalLength = 2;
            public const int Identification = 4;
            public const int FlagsAndOffset = 6;
            public const int Ttl = 8;
            public const int Protocol = 9;
            public const int HeaderChecksum = 10;
            public const int SourceAddress = 12;
            public const int DestinationAddress = 16;
            
            // Not often used in the real-world, so we don't parse it
            public const int Options = 20;
        }
    }
}
