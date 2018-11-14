﻿using System;
using System.IO;

namespace Snifter.Outputs.PcapNg
{
    public class EnhancedPacketBlock : BaseBlock
    {
        private readonly TimestampedData timestampedData;
        private static readonly byte[] BlockType = { 0x06, 0x00, 0x00, 0x00 };
        private static readonly byte[] InterfaceId = { 0x00, 0x00, 0x00, 0x00 };
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public EnhancedPacketBlock(TimestampedData timestampedData)
        {
            this.timestampedData = timestampedData;
        }

        public override byte[] GetBytes()
        {
            byte[] blockData;

            // Timestamp
            var timestamp = (long)(this.timestampedData.Timestamp - Epoch).TotalMilliseconds;
            var timestampHigh = (int)(timestamp >> 32);
            var timestampLow = (int)timestamp;

            // Captured Length
            var capturedLength = this.timestampedData.Data.Length;

            // Packet Length
            var packetLength = this.timestampedData.Data.Length;

            // Packet Data
            var packetData = PadToMultipleOf(this.timestampedData.Data, 4);

            // Block Length
            var blockLength = 32 + packetData.Length;

            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                writer.Write(BlockType);
                writer.Write(blockLength);
                writer.Write(InterfaceId);
                writer.Write(timestampHigh);
                writer.Write(timestampLow);
                writer.Write(capturedLength);
                writer.Write(packetLength);
                writer.Write(packetData);
                writer.Write(blockLength);

                blockData = ms.ToArray();
            }

            return blockData;
        }
    }
}
