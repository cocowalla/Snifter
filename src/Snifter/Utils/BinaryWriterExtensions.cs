using System.IO;
using Snifter.Output.PcapNg;

// ReSharper disable once CheckNamespace
namespace Snifter
{
    public static class BinaryWriterExtensions
    {
        public static void Write(this BinaryWriter writer, IBinaryWritable value)
            => value.WriteTo(writer);

        public static void WriteAligned(this BinaryWriter writer, byte[] value)
            => writer.BaseStream.WriteAligned(value);
    }
}
