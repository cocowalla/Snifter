using System;

// ReSharper disable once CheckNamespace
namespace Snifter
{
    /// <summary>
    /// Methods with a throw statement can't be inlined - a "throw helper" works around that limitation
    /// </summary>
    internal static class ThrowHelper
    {
        internal static void ThrowArgumentOutOfRangeException(string message) 
            => throw new ArgumentOutOfRangeException(message); 
    }
}
