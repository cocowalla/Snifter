using System;
using System.Net;
using Snifter.Protocol.Transport;

namespace Snifter.Protocol.Internet
{
    /// <summary>
    /// An Internet Protocol (IP) packet
    /// </summary>
    public interface IIpPacket
    {
        public DateTime CaptureTime { get; }
        
        /// <summary>IP Protocol Version (e.g. IPv4 or IPv6)</summary>
        public IpVersion Version { get; }
        
        public IpProtocol Protocol { get; }
        
        public IPAddress SourceAddress { get; }
        public IPAddress DestinationAddress { get; }
        
        /// <summary>Transport-layer packet contained within the payload</summary>
        public ITransportPacket TransportPacket { get; }
        
        /// <summary>The packet payload</summary>
        public ReadOnlyMemory<byte> Payload { get; }
        
        /// <summary>The full, raw data that comprises the packet</summary>
        public ReadOnlyMemory<byte> RawData { get; }
    }
}
