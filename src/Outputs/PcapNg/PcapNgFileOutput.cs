using System;
using System.IO;

namespace Snifter.Outputs.PcapNg
{
    /// <summary>
    /// Outputs files in PCAPNG file format
    /// https://tools.ietf.org/html/draft-tuexen-opswg-pcapng-00 
    /// </summary>
    public class PcapNgFileOutput : IOutput, IDisposable
    {
        private readonly NetworkInterfaceInfo nic;
        private readonly FileStream fileStream;
        private readonly BinaryWriter writer;

        public PcapNgFileOutput(NetworkInterfaceInfo nic, string filename)
        {
            this.nic = nic;
            this.fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
            this.writer = new BinaryWriter(this.fileStream);
            this.WriteHeader();
        }

        public void Output(TimestampedData timestampedData)
        {
            var block = new EnhancedPacketBlock(timestampedData);
            var blockData = block.GetBytes();

            this.writer.Write(blockData);
            this.writer.Flush();
        }

        public void Dispose()
        {
            this.writer.Close();
            this.writer.Dispose();
            this.fileStream.Close();
            this.fileStream.Dispose();
        }

        private void WriteHeader()
        {
            var sectionHeaderBlock = new SectionHeaderBlock();
            this.writer.Write(sectionHeaderBlock.GetBytes());

            var interfaceDescriptionBlock = new InterfaceDescriptionBlock(this.nic);
            this.writer.Write(interfaceDescriptionBlock.GetBytes());
        }
    }
}
