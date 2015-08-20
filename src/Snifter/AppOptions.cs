using System;
using System.IO;
using System.Net;
using Snifter.Filter;

namespace Snifter
{
    public class AppOptions
    {
        private readonly OptionSet optionSet;

        public bool ShowHelp { get; set; }
        public string OptionsHelpText { get; private set; }

        public int? InterfaceId { get; set; }
        public string Filename { get; set; }

        public FilterOperator FilterOperator { get; set; }
        public int? FilterProtocol { get; set; }
        public IPAddress FilterSourceAddress { get; set; }
        public IPAddress FilterDestAddress { get; set; }
        public short? FilterSourcePort { get; set; }
        public short? FilterDestPort { get; set; }

        public AppOptions()
        {
            this.FilterOperator = FilterOperator.OR;
            this.Filename = "snifter.pcapng";

            this.optionSet = new OptionSet {
                { "i=|interface=", "ID of the interface to listen on", x => this.InterfaceId = Int32.Parse(x) },
                { "f=|filename", "Filename to output sniffed packets to. Defaults to snifter.pcapng", x => this.Filename = x },
                { "o=|operator", "Whether filters should be AND or OR. Defaults to OR", x => this.FilterOperator = (FilterOperator)Enum.Parse(typeof(FilterOperator), x.ToUpper()) },
                { "p=|protocol", "Filter packets by IANA registered protocol number", x => this.FilterProtocol = Int32.Parse(x) },
                { "s=|source-address", "Filter packets by source IP address", x => this.FilterSourceAddress = IPAddress.Parse(x) },
                { "d=|dest-address", "Filter packets by destination IP address", x => this.FilterDestAddress = IPAddress.Parse(x) },
                { "x=|source-port", "Filter packets by source port number", x => this.FilterSourcePort = Int16.Parse(x) },
                { "y=|dest-port", "Filter packets by destination port number", x => this.FilterDestPort = Int16.Parse(x) },
                { "h|?|help", "Show command line options", x => this.ShowHelp = x != null }
            };

            // Get help text for the available options
            var optionsHelpTextOutput = new StringWriter();
            this.optionSet.WriteOptionDescriptions(optionsHelpTextOutput);
            this.OptionsHelpText = optionsHelpTextOutput.ToString();
        }

        public void Parse(string[] args)
        {
            this.optionSet.Parse(args);
        }

        public Filters<IPPacket> BuildFilters()
        {
            var filters = new Filters<IPPacket>(this.FilterOperator);

            if (this.FilterProtocol.HasValue)
            {
                filters.PropertyFilters.Add(new PropertyFilter<IPPacket>(x => x.Protocol, this.FilterProtocol.Value));
            }

            if (this.FilterSourceAddress != null)
            {
                filters.PropertyFilters.Add(new PropertyFilter<IPPacket>(x => x.SourceAddress, this.FilterSourceAddress));
            }

            if (this.FilterDestAddress != null)
            {
                filters.PropertyFilters.Add(new PropertyFilter<IPPacket>(x => x.DestAddress, this.FilterDestAddress));
            }

            if (this.FilterSourcePort.HasValue)
            {
                filters.PropertyFilters.Add(new PropertyFilter<IPPacket>(x => x.SourcePort, this.FilterSourcePort.Value));
            }

            if (this.FilterDestPort.HasValue)
            {
                filters.PropertyFilters.Add(new PropertyFilter<IPPacket>(x => x.DestPort, this.FilterDestPort.Value));
            }

            return filters;
        }
    }
}
