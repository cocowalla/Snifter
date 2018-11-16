using System.Runtime.InteropServices;
using System.Security.Principal;
using Mono.Unix.Native;

namespace Snifter
{
    public static class SystemInformation
    {
        public static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        public static bool IsLinux => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        public static bool IsAdmin()
        {
            if (IsWindows)
            {
                return new WindowsPrincipal(WindowsIdentity.GetCurrent())
                    .IsInRole(WindowsBuiltInRole.Administrator);
            }
            else
            {
                return Syscall.geteuid() == 0;
            }
        }
    }
}
