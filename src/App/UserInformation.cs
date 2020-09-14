using System.Security.Principal;
using Mono.Unix.Native;

namespace Snifter.App
{
    public static class UserInformation
    {
        public static bool IsAdmin()
        {
            if (SystemInformation.IsWindows)
            {
                return new WindowsPrincipal(WindowsIdentity.GetCurrent())
                    .IsInRole(WindowsBuiltInRole.Administrator);
            }

            return Syscall.geteuid() == 0;
        }
    }
}
