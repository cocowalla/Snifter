using System.IO;
using System.Text;

namespace Snifter.Output.PcapNg
{
    /// <summary>
    /// Options Field
    /// https://tools.ietf.org/html/draft-tuexen-opswg-pcapng-00#section-3.5
    /// 
    ///    0                   1                   2                   3
    ///    0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
    ///   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///   |      Option Code              |         Option Length         |
    ///   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///   /                       Option Value                            /
    ///   /             variable length, aligned to 32 bits               /
    ///   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///   /                                                               /
    ///   /                 . . . other options . . .                     /
    ///   /                                                               /
    ///   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///   |   Option Code == opt_endofopt  |  Option Length == 0          |
    ///   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    /// </summary>
    public class OptionalField : IBinaryWritable
    {
        public static readonly OptionalField EndOfOptions = new OptionalField(OptionTypeCode.EndOfOptions, new byte[0]);

        public short Code { get; }
        public byte[] Value { get; }
        public int Length { get; }

        /// <summary>
        /// Create a new Optional Field from a byte[] value
        /// </summary>
        /// <param name="typeCode">Option type</param>
        /// <param name="value">The value, which will be padded to 32-bits if required</param>
        public OptionalField(OptionTypeCode typeCode, byte[] value)
        {
            this.Code = typeCode.Value;
            this.Value = value;
            
            // 2 bytes for Option Code, 2 bytes for Option Value Length (without padding), variable length for Value Length (including any padding)
            this.Length = sizeof(short) + sizeof(short) + this.Value.GetAlignedLength();
        }
        
        /// <summary>
        /// Create a new Optional Field from a string value
        /// </summary>
        /// <param name="typeCode">Option type</param>
        /// <param name="value">The value, which will be converted to a UTF8-encoded byte array and padded to 32-bits if required</param>
        public OptionalField(OptionTypeCode typeCode, string value)
            : this(typeCode, Encoding.UTF8.GetBytes(value))
        {
            // Do nothing
        }

        public byte[] GetBytes()
        {
            using (var ms = MemoryStreamPool.Get())
            {
                using (var writer = new BinaryWriter(ms, Encoding.UTF8, true))
                {
                    writer.Write(this);
                }

                return ms.ToArray();
            }
        }
        
        public void WriteTo(BinaryWriter writer)
        {
            // 2-byte Option Code
            writer.Write(this.Code);
            
            // 2-byte Option Length
            writer.Write((short)this.Value.Length);
            
            // Variable length Option Value
            writer.WriteAligned(this.Value);
        }
    }
}