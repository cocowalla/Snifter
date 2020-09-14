using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.IO;

// ReSharper disable once CheckNamespace
namespace Snifter
{
    /// <summary>
    /// Thin abstraction over RecyclableMemoryStreamManager
    /// </summary>
    public static class MemoryStreamPool
    {
        private static readonly RecyclableMemoryStreamManager Default = new RecyclableMemoryStreamManager();

        public static MemoryStream Get([CallerMemberName]string name = null) =>
            Default.GetStream(name);

        public static MemoryStream Get(int len, [CallerMemberName]string name = null) =>
            Default.GetStream(name, len);

        public static MemoryStream Get(byte[] buffer, [CallerMemberName]string name = null) =>
            Default.GetStream(name, buffer, 0, buffer.Length);
    }
}