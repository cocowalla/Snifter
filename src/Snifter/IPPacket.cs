using System;
using System.Net;

namespace Snifter
{
    public class IPPacket
    {
        public int Version { get; private set; }
        public int HeaderLength { get; private set; }
        public int Protocol { get; private set; }
        public IPAddress SourceAddress { get; private set; }
        public IPAddress DestAddress { get; private set; }
        internal UInt16 SourcePort { get; private set; }
        internal UInt16 DestPort { get; private set; }

        public IPPacket(byte[] data)
        {
            var versionAndLength = data[0];
            this.Version = versionAndLength >> 4;

            // Only parse IPv4 packets for now
            if (this.Version == 4)
            {
                this.HeaderLength = (versionAndLength & 0x0F) << 2;

                this.Protocol = Convert.ToInt32(data[9]);
                this.SourceAddress = new IPAddress(BitConverter.ToUInt32(data, 12));
                this.DestAddress = new IPAddress(BitConverter.ToUInt32(data, 16));

                if (Enum.IsDefined(typeof (ProtocolsWithPort), this.Protocol))
                {
                    this.SourcePort = BitConverter.ToUInt16(data, this.HeaderLength);
                    this.DestPort = BitConverter.ToUInt16(data, this.HeaderLength + 2);
                }
            }
        }
    }

    /// <summary>
    /// Protocols that have a port abstraction
    /// </summary>
    internal enum ProtocolsWithPort
    {
        TCP = 6,
        UDP = 17,
        SCTP = 132
    }
}
