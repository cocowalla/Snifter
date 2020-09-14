using System.Collections.Generic;
using System.Runtime.CompilerServices;

// ReSharper disable once CheckNamespace
namespace Snifter
{
    public static class ArrayExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetAlignedLength(this IReadOnlyCollection<byte> source, int alignment = 4)
        {
            var alignedLength = (source.Count + alignment - 1) / alignment * alignment;
            return alignedLength;
        }
    }
}
