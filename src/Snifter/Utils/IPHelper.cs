using System;
using System.Net;
using System.Runtime.CompilerServices;

// ReSharper disable once CheckNamespace
namespace Snifter
{
    public static class IPHelper
    {
        // IPAddress ctor that takes ReadOnlySpan<byte> only available from .NET Standard 2.1
        #if NETSTANDARD2_1

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IPAddress ReadIPv4Address(ReadOnlySpan<byte> address)
            => new IPAddress(address);
        
        #else
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IPAddress ReadIPv4Address(ReadOnlySpan<byte> address)
        {
            const int IPv4NumBytes = 4;

            if (address.Length != IPv4NumBytes)
            {
                throw new ArgumentException($"Invalid IPv4 address - expected 4 bytes, but got {address.Length}");
            }

            var intAddress = (uint)((address[3] << 24 | address[2] << 16 | address[1] << 8 | address[0]) & 0x0FFFFFFFF);
            return new IPAddress(intAddress);
        }
        
        #endif
    }
}