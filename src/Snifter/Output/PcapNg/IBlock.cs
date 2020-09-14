
namespace Snifter.Output.PcapNg
{
    public interface IBlock : IBinaryWritable
    {
        byte[] GetBytes();
    }
}
