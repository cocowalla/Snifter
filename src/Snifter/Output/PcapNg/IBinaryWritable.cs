using System.IO;

namespace Snifter.Output.PcapNg
{
    public interface IBinaryWritable
    {
        void WriteTo(BinaryWriter writer);
    }
}
