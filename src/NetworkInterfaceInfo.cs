using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Snifter
{
    public class NetworkInterfaceInfo
    {
        public int Index { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public IPAddress IPAddress { get; set; }

        public static IList<NetworkInterfaceInfo> GetInterfaces()
        {
            var nicInfos = new List<NetworkInterfaceInfo>();
            var nics = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var nic in nics)
            {
                var ipAddresses = nic.GetIPProperties().UnicastAddresses.Where(x =>
                    x.Address != null && x.Address.AddressFamily == AddressFamily.InterNetwork);

                foreach (var ipAddress in ipAddresses)
                {
                    var nicInfo = new NetworkInterfaceInfo
                    {
                        Index = nicInfos.Count,
                        Id = nic.Id,
                        Name = nic.Name,
                        IPAddress = ipAddress.Address
                    };

                    nicInfos.Add(nicInfo);
                }
            }

            return nicInfos;
        }
    }
}
