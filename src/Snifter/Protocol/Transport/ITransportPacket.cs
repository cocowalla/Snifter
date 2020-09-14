using System;

namespace Snifter.Protocol.Transport
{
    public interface ITransportPacket
    {
        /// <summary>The full, raw data that comprises the packet</summary>
        public ReadOnlyMemory<byte> RawData { get; }
    }
    
    /// <summary>
    /// A transport-layer packet for a protocol with a port abstraction (e.g. TCP, UDP, SCTP)
    /// </summary>
    public interface IHasPorts
    {
        public ushort SourcePort { get; }
        public ushort DestinationPort { get; }
    }
}
