using System.IO;
using System.Text;

namespace Snifter.Output.PcapNg
{
    /// <summary>
    /// Interface Description Block. This block specifies the characteristics of the network
    /// interface on which the capture has been made.
    /// https://tools.ietf.org/html/draft-tuexen-opswg-pcapng-00#section-4.2
    ///
    ///       0                   1                   2                   3
    ///       0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
    ///      +---------------------------------------------------------------+
    ///    0 |                    Block Type = 0x00000001                    |
    ///      +---------------------------------------------------------------+
    ///    4 |                      Block Total Length                       |
    ///      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///    8 |           LinkType            |           Reserved            |
    ///      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///   12 |                            SnapLen                            |
    ///      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///   16 /                                                               /
    ///      /                      Options (variable)                       /
    ///      /                                                               /
    ///      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///      |                      Block Total Length                       |
    ///      +---------------------------------------------------------------+
    /// 
    /// </summary>
    public class InterfaceDescriptionBlock : IBlock
    {
        private static readonly byte[] BlockType = { 0x01, 0x00, 0x00, 0x00 };

        // SnapLen (65535 bytes)
        private static readonly byte[] SnapLen = { 0xff, 0xff, 0x00, 0x00 };

        // Link Layer Type (Raw IP: http://www.tcpdump.org/linktypes.html)
        private static readonly byte[] LinkType = { 0x65, 0x00, 0x00, 0x00 };

        // Options: Timestamp Resolution Name (10^-3s == milliseconds)
        private static readonly byte[] TsResolution = { 0x03 };
        private static readonly OptionalField TsResolutionOption = new OptionalField(OptionTypeCode.InterfaceTimestampResolution, TsResolution);
        private readonly OptionalField interfaceNameOption;
        private readonly OptionalField interfaceDescriptionOption;
        
        // Block Total Length
        public int TotalBlockLength { get; }

        public InterfaceDescriptionBlock(NetworkInterfaceInfo nic)
        {
            // Options: Interface Name (if_name)
            this.interfaceNameOption = new OptionalField(OptionTypeCode.InterfaceName, $"\\Device\\NPF_{nic.Id}");

            // Options: Interface Description (if_description)
            this.interfaceDescriptionOption = new OptionalField(OptionTypeCode.InterfaceDescription, nic.Name);

            this.TotalBlockLength =
                BlockType.Length + 
                sizeof(int) + 
                LinkType.Length + 
                SnapLen.Length + 
                this.interfaceNameOption.Length + 
                this.interfaceDescriptionOption.Length + 
                TsResolutionOption.Length +
                OptionalField.EndOfOptions.Length +
                sizeof(int);
        }

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(BlockType);
            writer.Write(this.TotalBlockLength);
            writer.Write(LinkType);
            writer.Write(SnapLen);

            writer.Write(this.interfaceNameOption);
            writer.Write(this.interfaceDescriptionOption);
            writer.Write(TsResolutionOption);
            writer.Write(OptionalField.EndOfOptions);

            writer.Write(this.TotalBlockLength);
        }
        
        public byte[] GetBytes()
        {
            byte[] blockData;
            
            using (var ms = MemoryStreamPool.Get(this.TotalBlockLength))
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
