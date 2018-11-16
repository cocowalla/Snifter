using System.IO;

namespace Snifter.Outputs.PcapNg
{
    public class InterfaceDescriptionBlock : BaseBlock
    {
        private readonly NetworkInterfaceInfo nic;
        private static readonly byte[] BlockType = { 0x01, 0x00, 0x00, 0x00 };

        // SnapLen (65535 bytes)
        private static readonly byte[] SnapLen = { 0xff, 0xff, 0x00, 0x00 };

        // Link Layer Type (Raw IP: http://www.tcpdump.org/linktypes.html)
        private static readonly byte[] LinkLayer = { 0x65, 0x00, 0x00, 0x00 };

        // Options: Timestamp Resolution Name (10^-3s == milliseconds)
        private static readonly byte[] TsResolution = { 0x03 };
        private static readonly byte[] TsResolutionOption = GetOptionBytes(9, TsResolution);

        // End of options
        private static readonly byte[] EndOptionCode = { 0x00, 0x00 };
        private static readonly byte[] EndOptionLength = { 0x00, 0x00 };

        public InterfaceDescriptionBlock(NetworkInterfaceInfo nic)
        {
            this.nic = nic;
        }

        public override byte[] GetBytes()
        {
            byte[] blockData;

            // Options: Interface Name
            var interfaceNameOption = GetOptionBytes(2, $"\\Device\\NPF_{this.nic.Id}");

            // Block Length
            var blockLength = 24 + interfaceNameOption.Length + TsResolutionOption.Length;

            using (var ms = new MemoryStream())
            {
                using (var writer = new BinaryWriter(ms))
                {
                    writer.Write(BlockType);
                    writer.Write(blockLength);
                    writer.Write(LinkLayer);
                    writer.Write(SnapLen);
                    writer.Write(interfaceNameOption);
                    writer.Write(TsResolutionOption);
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
