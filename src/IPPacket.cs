using System;
using System.Net;

namespace Snifter
{
    // ReSharper disable once InconsistentNaming
    public class IPPacket
    {
        public int Version { get; }
        public int HeaderLength { get; }
        public int Protocol { get; }
        public IPAddress SourceAddress { get; }
        public IPAddress DestAddress { get; }
        public ushort SourcePort { get; }
        public ushort DestPort { get; }

        public IPPacket(byte[] data)
        {
            var versionAndLength = data[0];
            this.Version = versionAndLength >> 4;

            // Only parse IPv4 packets for now
            if (this.Version != 4)
                return;

            this.HeaderLength = (versionAndLength & 0x0F) << 2;

            this.Protocol = Convert.ToInt32(data[9]);
            this.SourceAddress = new IPAddress(BitConverter.ToUInt32(data, 12));
            this.DestAddress = new IPAddress(BitConverter.ToUInt32(data, 16));

            if (Enum.IsDefined(typeof(ProtocolsWithPort), this.Protocol))
            {
                // Ensure big-endian
                this.SourcePort = (ushort)((data[this.HeaderLength] << 8) | data[this.HeaderLength + 1]);
                this.DestPort = (ushort)((data[this.HeaderLength + 2] << 8) | data[this.HeaderLength + 3]);
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
