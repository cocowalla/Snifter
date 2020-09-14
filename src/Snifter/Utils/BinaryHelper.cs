using System;
using System.Runtime.CompilerServices;

// BinaryPrimitives only available from .NET Standard 2.1
#if NETSTANDARD2_1
using System.Buffers.Binary;
#else
using System.Runtime.InteropServices;
#endif

// ReSharper disable once CheckNamespace
namespace Snifter
{
    /// <summary>
    /// Helpers for reading big-endian integers
    /// </summary>
    public static class BinaryHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadUInt32BigEndian(this ReadOnlySpan<byte> span, int offset)
            => ReadUInt32BigEndian(span.Slice(offset, sizeof(uint)));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReadUInt16BigEndian(this ReadOnlySpan<byte> span, int offset)
            => ReadUInt16BigEndian(span.Slice(offset, sizeof(ushort)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ReadBits(byte value, int bitPosition, int bitLength)
        {
            const int bitsInByte = sizeof(byte) * 8;
            var bitShift = bitsInByte - bitPosition - bitLength;
            
            if (bitShift < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("Unable to read more than 8 bits from a byte");
            }
            
            return (byte)(((0xff >> bitPosition) & value) >> bitShift);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReadBits(ushort value, int bitPosition, int bitLength)
        {
            const int bitsInUshort = sizeof(ushort) * 8;
            var bitShift = bitsInUshort - bitPosition - bitLength;
            
            if (bitShift < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("Unable to read more than 16 bits from a ushort");
            }
            
            return (ushort)(((0xffff >> bitPosition) & value) >> bitShift);
        }
        
        // BinaryPrimitives only available from .NET Standard 2.1
        #if NETSTANDARD2_1
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadUInt32BigEndian(ReadOnlySpan<byte> source)
            => BinaryPrimitives.ReadUInt32BigEndian(source);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReadUInt16BigEndian(ReadOnlySpan<byte> source)
            => BinaryPrimitives.ReadUInt16BigEndian(source);
        
        #else
        
        /// <summary>
        /// Reads an Int32 out of a read-only span of bytes as big endian.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadUInt32BigEndian(ReadOnlySpan<byte> source)
        {
            var result = MemoryMarshal.Read<uint>(source);
            
            if (BitConverter.IsLittleEndian)
            {
                result = ReverseEndianness(result);
            }
            
            return result;
        }
        
        /// <summary>
        /// Reads an Int16 out of a read-only span of bytes as big endian.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReadUInt16BigEndian(ReadOnlySpan<byte> source)
        {
            var result = MemoryMarshal.Read<ushort>(source);
            
            if (BitConverter.IsLittleEndian)
            {
                result = ReverseEndianness(result);
            }
            
            return result;
        }
        
        /// <summary>Reverses a primitive value - performs an endianness swap</summary> 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReverseEndianness(ushort value)
        {
            // Don't need to AND with 0xFF00 or 0x00FF since the final
            // cast back to ushort will clear out all bits above [ 15 .. 00 ].
            // This is normally implemented via "movzx eax, ax" on the return.
            // Alternatively, the compiler could elide the movzx instruction
            // entirely if it knows the caller is only going to access "ax"
            // instead of "eax" / "rax" when the function returns.

            return (ushort)((value >> 8) + (value << 8));
        }
        
        /// <summary>Reverses a primitive value - performs an endianness swap</summary> 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint ReverseEndianness(uint value)
        {
            // This takes advantage of the fact that the JIT can detect
            // ROL32 / ROR32 patterns and output the correct intrinsic.
            //
            // Input: value = [ ww xx yy zz ]
            //
            // First line generates : [ ww xx yy zz ]
            //                      & [ 00 FF 00 FF ]
            //                      = [ 00 xx 00 zz ]
            //             ROR32(8) = [ zz 00 xx 00 ]
            //
            // Second line generates: [ ww xx yy zz ]
            //                      & [ FF 00 FF 00 ]
            //                      = [ ww 00 yy 00 ]
            //             ROL32(8) = [ 00 yy 00 ww ]
            //
            //                (sum) = [ zz yy xx ww ]
            //
            // Testing shows that throughput increases if the AND
            // is performed before the ROL / ROR.

            return RotateRight(value & 0x00FF00FFu, 8) // xx zz
                   + RotateLeft(value & 0xFF00FF00u, 8); // ww yy
        }
        
        /// <summary>
        /// Rotates the specified value right by the specified number of bits.
        /// Similar in behavior to the x86 instruction ROR.
        /// </summary>
        /// <param name="value">The value to rotate.</param>
        /// <param name="offset">The number of bits to rotate by.
        /// Any value outside the range [0..31] is treated as congruent mod 32.</param>
        /// <returns>The rotated value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint RotateRight(uint value, int offset)
            => (value >> offset) | (value << (32 - offset));
        
        /// <summary>
        /// Rotates the specified value left by the specified number of bits.
        /// Similar in behavior to the x86 instruction ROL.
        /// </summary>
        /// <param name="value">The value to rotate.</param>
        /// <param name="offset">The number of bits to rotate by.
        /// Any value outside the range [0..31] is treated as congruent mod 32.</param>
        /// <returns>The rotated value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint RotateLeft(uint value, int offset)
            => (value << offset) | (value >> (32 - offset));
        
        #endif
    }
}