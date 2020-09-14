using System.Runtime.InteropServices;

// ReSharper disable once CheckNamespace
namespace Snifter
{
    public static class SystemInformation
    {
        public static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        public static bool IsLinux => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    }
}
