using System;
using System.IO;

namespace Snifter.Outputs.PcapNg
{
    public class SectionHeaderBlock : BaseBlock
    {
        public override byte[] GetBytes()
        {
            byte[] blockData;

            // Block Type
            var blockType = new byte[] { 0x0a, 0x0d, 0x0d, 0x0a };

            // Byte-Order Magic
            var byteOrderMagic = new byte[] { 0x4d, 0x3c, 0x2b, 0x1a };

            // Version (makor version 1, minor version 0)
            var version = new byte[] { 0x01, 0x00, 0x00, 0x00 };

            // Section Length (unspecified)
            var sectionLength = new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff };

            // OS
            var osOption = this.GetOptionBytes(3, Environment.OSVersion.ToString());

            // Application
            var appOption = this.GetOptionBytes(4, "http://github.com/cocowalla/snifter");

            // End of options
            var endOptionCode = new byte[] { 0x00, 0x00 };
            var endOptionLength = new byte[] { 0x00, 0x00 };

            // Block Length
            var blockLength = 32 + osOption.Length + appOption.Length;

            using (var ms = new MemoryStream())
            {
                using (var writer = new BinaryWriter(ms))
                {
                    writer.Write(blockType);
                    writer.Write(blockLength);
                    writer.Write(byteOrderMagic);
                    writer.Write(version);
                    writer.Write(sectionLength);
                    writer.Write(osOption);
                    writer.Write(appOption);
                    writer.Write(endOptionCode);
                    writer.Write(endOptionLength);
                    writer.Write(blockLength);
                }

                blockData = ms.ToArray();
            }

            return blockData;
        }
    }
}
