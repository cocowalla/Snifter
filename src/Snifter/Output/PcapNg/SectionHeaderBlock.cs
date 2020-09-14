using System;
using System.IO;
using System.Text;

namespace Snifter.Output.PcapNg
{
    /// <summary>
    /// Section Header Block (SHB). The SHB is mandatory, and identifies the beginning of a section
    /// of the capture dump file.
    /// https://tools.ietf.org/html/draft-tuexen-opswg-pcapng-00#section-4.1
    /// 
    ///       0                   1                   2                   3
    ///       0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
    ///       +---------------------------------------------------------------+
    ///     0 |                   Block Type = 0x0A0D0D0A                     |
    ///       +---------------------------------------------------------------+
    ///     4 |                      Block Total Length                       |
    ///       +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///     8 |                      Byte-Order Magic                         |
    ///       +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///    12 |          Major Version        |         Minor Version         |
    ///       +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///    16 |                                                               |
    ///       |                          Section Length                       |
    ///       |                                                               |
    ///       +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///    24 /                                                               /
    ///       /                      Options (variable)                       /
    ///       /                                                               /
    ///       +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///       |                      Block Total Length                       |
    ///       +---------------------------------------------------------------+
    /// </summary>
    public class SectionHeaderBlock : IBlock
    {
        private static readonly byte[] BlockType = { 0x0a, 0x0d, 0x0d, 0x0a };
        private static readonly byte[] ByteOrderMagic = { 0x4d, 0x3c, 0x2b, 0x1a };

        // PCAPNG format version (major version 1, minor version 0)
        private static readonly byte[] Version = { 0x01, 0x00, 0x00, 0x00 };

        // Section Length (0xffffffffffffffff means "unspecified")
        private static readonly byte[] SectionLength = { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff };

        private static readonly OptionalField OperatingSystem = new OptionalField(OptionTypeCode.SectionHeaderOperatingSystem, Environment.OSVersion.ToString());
        private static readonly OptionalField Application = new OptionalField(OptionTypeCode.SectionHeaderUserApp, "https://github.com/cocowalla/snifter");
        
        // Block Total Length
        private static readonly int TotalBlockLength = 
            BlockType.Length + 
            sizeof(int) + 
            ByteOrderMagic.Length + 
            Version.Length + 
            SectionLength.Length + 
            OperatingSystem.Length + 
            Application.Length + 
            OptionalField.EndOfOptions.Length + 
            sizeof(int);

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(BlockType);
            writer.Write(TotalBlockLength);
            writer.Write(ByteOrderMagic);
            writer.Write(Version);
            writer.Write(SectionLength);

            writer.Write(OperatingSystem);
            writer.Write(Application);
            writer.Write(OptionalField.EndOfOptions);

            writer.Write(TotalBlockLength);
        }

        public byte[] GetBytes()
        {
            byte[] blockData;

            using (var ms = MemoryStreamPool.Get(TotalBlockLength))
            {
                using (var writer = new BinaryWriter(ms, Encoding.UTF8, true))
                {
                    WriteTo(writer);
                }
                
                blockData = ms.ToArray();
            }

            return blockData;
        }
    }
}
