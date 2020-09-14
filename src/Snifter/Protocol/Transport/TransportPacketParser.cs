using Snifter.Protocol.Internet;

namespace Snifter.Protocol.Transport
{
    public class TransportPacketParser
    {
        /// <summary>
        /// Builds a transport-layer packet from an IP packet payload
        /// </summary>
        public ITransportPacket Parse(IIpPacket ipPacket)
        {
            return ipPacket.Protocol switch
            {
                IpProtocol.TCP => new TcpPacket(ipPacket),
                IpProtocol.UDP => new UdpPacket(ipPacket),
                IpProtocol.ICMP => new IcmpPacket(ipPacket),
                
                // Other transport-layer packet types are not yet supported
                _ => null
            };
        }
    }
}
