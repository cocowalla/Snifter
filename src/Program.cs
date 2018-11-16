using System;
using System.Security.Principal;
using System.Threading;
using Snifter.Outputs.PcapNg;

namespace Snifter
{
    internal class Program
    {
        private static bool isStopping;

        private static void Main(string[] args)
        {
            // You can only create raw sockets with elevated privileges
            if (!SystemInformation.IsAdmin())
            {
                var message = SystemInformation.IsWindows
                    ? "Please run with elevated prilileges"
                    : "Please run using sudo";

                Console.WriteLine(message);
                Environment.Exit(1);
            }

            var appOptions = ParseCommandLine(args);

            if (appOptions.ShowHelp)
            {
                ShowHelp(appOptions);
                Environment.Exit(0);
            }

            var nics = NetworkInterfaceInfo.GetInterfaces();

            if (!appOptions.InterfaceId.HasValue || 
                appOptions.InterfaceId > nics.Count - 1 || 
                appOptions.InterfaceId < 0)
            {
                Console.WriteLine("Invalid interface ID");
                ShowHelp(appOptions);
                Environment.Exit(3);
            }

            var filters = appOptions.BuildFilters();
            var nic = nics[appOptions.InterfaceId.Value];

            // Start capturing packets
            var output = new PcapNgFileOutput(nic, appOptions.Filename);
            var sniffer = new SocketSniffer(nic, filters, output);
            sniffer.Start();

            Console.WriteLine();
            Console.WriteLine("Capturing on interface {0} ({1})", nic.Name, nic.IPAddress);
            Console.WriteLine("Saving to file {0}", appOptions.Filename);
            Console.WriteLine("Press CTRL+C to stop");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            // Shutdown gracefully on CTRL+C
            Console.CancelKeyPress += ConsoleOnCancelKeyPress;

            while (!isStopping)
            {
                Console.SetCursorPosition(0, Console.CursorTop - 2);
                Console.WriteLine("Packets Observed: {0}", sniffer.PacketsObserved);
                Console.WriteLine("Packets Captured: {0}", sniffer.PacketsCaptured);

                Thread.Sleep(200);
            }

            sniffer.Stop();
        }

        private static AppOptions ParseCommandLine(string[] args)
        {
            var appOptions = new AppOptions();

            try
            {
                appOptions.Parse(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(2);
            }

            return appOptions;
        }

        private static void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            isStopping = true;
        }

        private static void ShowHelp(AppOptions appOptions)
        {
            Console.WriteLine(@"      __.---,__                ");
            Console.WriteLine(@"   .-`         '-,__           ");
            Console.WriteLine(@" &/           ',_\ _\          ");
            Console.WriteLine(@" /               '',_          ");
            Console.WriteLine(@" |    .            ("")         ");
            Console.WriteLine(@" |__.`'-..--|__|--``   Snifter ");
            Console.WriteLine();

            Console.WriteLine("Usage:");
            Console.WriteLine("snifter -i x -f filename");
            Console.WriteLine();
            Console.WriteLine(appOptions.OptionsHelpText);
            Console.WriteLine();

            var nicInfos = NetworkInterfaceInfo.GetInterfaces();

            Console.WriteLine("ID\tIP Address\tName");
            Console.WriteLine("===========================================================");

            foreach (var nicInfo in nicInfos)
            {
                Console.WriteLine("{0}\t{1}\t{2}", nicInfo.Index, nicInfo.IPAddress, nicInfo.Name);
            }
        }
    }
}
