using System;
using Snifter.Protocol.Transport;

namespace Snifter.Protocol.Internet
{
    public class IpPacketParser
    {
        private readonly TransportPacketParser transportPacketParser;

        /// <summary>
        /// Create an IP Packet Parser that will parse IP packets, without parsing the transport-level payload
        /// </summary>
        public IpPacketParser()
        {
            // Do nothing
        }
        
        /// <summary>
        /// Create an IP Packet Parser that will parse IP packets and their transport-level payload
        /// </summary>
        public IpPacketParser(TransportPacketParser transportPacketParser)
        {
            this.transportPacketParser = transportPacketParser;
        }
        
        /// <summary>
        /// Builds an IP packet from a raw packet capture
        /// </summary>
        /// <param name="data">Raw packet capture</param>
        /// <param name="captureTime">Time the packet was captured</param>
        /// <returns>A parsed IP packet</returns>
        public IIpPacket Parse(ReadOnlyMemory<byte> data, DateTime captureTime)
        {
            // First byte contains both the IP Version and Header Length
            var versionAndLength = data.Span[0];
            var version = BinaryHelper.ReadBits(versionAndLength, 0, 4);

            if (version == 4)
            {
                var packet = new IpV4Packet(data, captureTime);

                if (this.transportPacketParser != null)
                {
                    packet.ParseTransportPacket(this.transportPacketParser);
                }

                return packet;
            }
            if (version == 6)
            {
                // IPv6 packets not yet supported!
                return null;
            }
            
            throw new ArgumentOutOfRangeException($"Unexpected IP packet version: {version}");
        }
    }
}
