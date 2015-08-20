using System;
using System.IO;

namespace Snifter.Outputs.PcapNg
{
    public class InterfaceDescriptionBlock : BaseBlock
    {
        private readonly NetworkInterfaceInfo nic;

        public InterfaceDescriptionBlock(NetworkInterfaceInfo nic)
        {
            this.nic = nic;
        }

        public override byte[] GetBytes()
        {
            byte[] blockData;

            // Block Type
            var blockType = new byte[] { 0x01, 0x00, 0x00, 0x00 };

            // Link Layer Type (Raw IP: http://www.tcpdump.org/linktypes.html)
            var linkLayer = new byte[] { 0x65, 0x00, 0x00, 0x00 };

            // SnapLen (65535 bytes)
            var snapLen = new byte[] { 0xff, 0xff, 0x00, 0x00 };

            // Options: Interface Name
            var interfaceNameOption = this.GetOptionBytes(2, String.Format("\\Device\\NPF_{0}", this.nic.Id));

            // Options: Timestamp Resolution Name (10^-3s == milliseconds)
            var tsResolution = new byte[] { 0x03 };
            var tsResolutionOption = this.GetOptionBytes(9, tsResolution);

            // End of options
            var endOptionCode = new byte[] { 0x00, 0x00 };
            var endOptionLength = new byte[] { 0x00, 0x00 };

            // Block Length
            var blockLength = 24 + interfaceNameOption.Length + tsResolutionOption.Length;

            using (var ms = new MemoryStream())
            {
                using (var writer = new BinaryWriter(ms))
                {
                    writer.Write(blockType);
                    writer.Write(blockLength);
                    writer.Write(linkLayer);
                    writer.Write(snapLen);
                    writer.Write(interfaceNameOption);
                    writer.Write(tsResolutionOption);
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
