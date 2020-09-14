using System;
using System.IO;
using System.Text;

namespace Snifter.Output.PcapNg
{
    /// <summary>
    /// An Enhanced Packet Block. This is the standard container for storing captured packets.
    /// https://tools.ietf.org/html/draft-tuexen-opswg-pcapng-00#section-4.3
    ///
    ///       0                   1                   2                   3
    ///       0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
    ///       +---------------------------------------------------------------+
    ///     0 |                    Block Type = 0x00000006                    |
    ///       +---------------------------------------------------------------+
    ///     4 |                      Block Total Length                       |
    ///       +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///     8 |                         Interface ID                          |
    ///       +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///    12 |                        Timestamp (High)                       |
    ///       +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///    16 |                        Timestamp (Low)                        |
    ///       +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///    20 |                         Captured Len                          |
    ///       +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///    24 |                          Packet Len                           |
    ///       +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///    28 /                                                               /
    ///       /                          Packet Data                          /
    ///       /             variable length, aligned to 32 bits               /
    ///       /                                                               /
    ///       +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///       /                                                               /
    ///       /                      Options (variable)                       /
    ///       /                                                               /
    ///       +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///       |                      Block Total Length                       |
    ///       +---------------------------------------------------------------+
    /// </summary>
    public class EnhancedPacketBlock : IBlock
    {
        private readonly TimestampedData timestampedData;
        
        private static readonly byte[] BlockType = { 0x06, 0x00, 0x00, 0x00 };
        
        // Fixed at zero, since we only capture from a single interface 
        private static readonly byte[] InterfaceId = { 0x00, 0x00, 0x00, 0x00 };
        
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Number of bytes that are fixed in every block
        private const int FixedBlockSize = 32;

        public EnhancedPacketBlock(TimestampedData timestampedData)
        {
            this.timestampedData = timestampedData;
        }
        
        public void WriteTo(BinaryWriter writer)
        {
            // Timestamp
            var timestamp = (ulong)(this.timestampedData.Timestamp - Epoch).TotalMilliseconds;
            var timestampHigh = (uint)(timestamp >> 32);
            var timestampLow = (uint)timestamp;

            // Captured Length (number of bytes captured from the packet, unpadded - here the same as Packet Length)
            var capturedLength = this.timestampedData.Data.Length;

            // Packet Length
            var packetLength = this.timestampedData.Data.Length;

            // Packet Data (the block body, which must be written aligned to 32-bits)
            var packetData = this.timestampedData.Data;

            // Block Total Length
            var blockLength = FixedBlockSize + packetData.GetAlignedLength();
            
            writer.Write(BlockType);
            writer.Write(blockLength);
            writer.Write(InterfaceId);
            writer.Write(timestampHigh);
            writer.Write(timestampLow);
            writer.Write(capturedLength);
            writer.Write(packetLength);
            writer.WriteAligned(packetData);
            writer.Write(blockLength);
        }

        public byte[] GetBytes()
        {
            byte[] blockData;

            // Block Total Length
            var blockLength = FixedBlockSize + this.timestampedData.Data.GetAlignedLength();

            using (var ms = MemoryStreamPool.Get(blockLength))
            {
                using (var writer = new BinaryWriter(ms, Encoding.UTF8, true))
                {
                    writer.Write(this);
                }
                
                blockData = ms.ToArray();
            }

            return blockData;
        }
    }
}
