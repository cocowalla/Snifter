using System;
using Snifter.Protocol.Internet;

namespace Snifter.Protocol.Transport
{
    /// <summary>
    /// An unparsed packet, simply providing access to the raw payload
    /// </summary>
    public sealed class RawPacket : ITransportPacket
    {
        /// <summary>The full, raw data that comprises the packet</summary>
        public ReadOnlyMemory<byte> RawData { get; }
        
        public RawPacket(IIpPacket ipPacket)
        {
            if (ipPacket == null) throw new ArgumentNullException(nameof(ipPacket));
            
            this.RawData = ipPacket.Payload;
        }
    }
}
