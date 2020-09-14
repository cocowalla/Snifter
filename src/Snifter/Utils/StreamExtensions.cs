using System;
using System.Buffers;
using System.IO;

// ReSharper disable once CheckNamespace
namespace Snifter
{
    public static class StreamExtensions
    {
        /// <summary>
        /// Writes data aligned to <paramref name="alignment"/> bytes
        /// </summary>
        /// <param name="stream">Stream to write aligned data to</param>
        /// <param name="value">Value to write to the stream, aligned to <paramref name="alignment"/> bytes</param>
        /// <param name="alignment">Number of bytes to align to - defaults to 4 (32-bits)</param>
        public static void WriteAligned(this Stream stream, byte[] value, int alignment = 4)
        {
            if (value.Length == 0)
                return;
            
            stream.Write(value, 0, value.Length);
            
            // Determine how much padding is required to align, if any
            var alignedLen = (value.Length + alignment - 1) / alignment * alignment;

            // Is padding required?
            if (value.Length == alignedLen)
                return;

#if NETSTANDARD2_1
            Span<byte> padding = stackalloc byte[alignedLen - value.Length];
            stream.Write(padding);
#else
            var paddingLen = alignedLen - value.Length;
            var padding = ArrayPool<byte>.Shared.Rent(paddingLen);
            
            stream.Write(padding, 0, padding.Length);
            
            ArrayPool<byte>.Shared.Return(padding);
#endif
        }
    }
}
