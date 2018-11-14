using System;
using System.IO;

namespace Snifter.Outputs.PcapNg
{
    public class SectionHeaderBlock : BaseBlock
    {
        private static readonly byte[] BlockType = { 0x0a, 0x0d, 0x0d, 0x0a };
        private static readonly byte[] ByteOrderMagic = { 0x4d, 0x3c, 0x2b, 0x1a };

        // Version (makor version 1, minor version 0)
        private static readonly byte[] Version = { 0x01, 0x00, 0x00, 0x00 };

        // Section Length (unspecified)
        private static readonly byte[] SectionLength = { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff };

        private static readonly byte[] OperatingSystem = GetOptionBytes(3, Environment.OSVersion.ToString());
        private static readonly byte[] Application = GetOptionBytes(4, "https://github.com/cocowalla/snifter");

        // End of options
        private static readonly byte[] EndOptionCode = { 0x00, 0x00 };
        private static readonly byte[] EndOptionLength = { 0x00, 0x00 };

        public override byte[] GetBytes()
        {
            byte[] blockData;

            // Block Length
            var blockLength = 32 + OperatingSystem.Length + Application.Length;

            using (var ms = new MemoryStream())
            {
                using (var writer = new BinaryWriter(ms))
                {
                    writer.Write(BlockType);
                    writer.Write(blockLength);
                    writer.Write(ByteOrderMagic);
                    writer.Write(Version);
                    writer.Write(SectionLength);
                    writer.Write(OperatingSystem);
                    writer.Write(Application);
                    writer.Write(EndOptionCode);
                    writer.Write(EndOptionLength);
                    writer.Write(blockLength);
                }

                blockData = ms.ToArray();
            }

            return blockData;
        }
    }
}
