
namespace Snifter.Output.PcapNg
{
    public class OptionTypeCode
    {
        public short Value { get; }

        private OptionTypeCode(short value)
        {
            this.Value = value;
        }

        private static OptionTypeCode WithValue(short value)
            => new OptionTypeCode(value);

        // opt_endofopt
        public static readonly OptionTypeCode EndOfOptions = WithValue(0);
        
        // opt_comment
        public static readonly OptionTypeCode Comment = WithValue(1);
        
        // shb_hardware
        public static readonly OptionTypeCode SectionHeaderHardware = WithValue(2);
        
        // shb_os 
        public static readonly OptionTypeCode SectionHeaderOperatingSystem = WithValue(3);
        
        // shb_userappl
        public static readonly OptionTypeCode SectionHeaderUserApp = WithValue(2);
            
        // if_name
        public static readonly OptionTypeCode InterfaceName = WithValue(2);
            
        // if_description
        public static readonly OptionTypeCode InterfaceDescription = WithValue(3);
            
        // if_tsresol
        public static readonly OptionTypeCode InterfaceTimestampResolution = WithValue(9);
    }
}
