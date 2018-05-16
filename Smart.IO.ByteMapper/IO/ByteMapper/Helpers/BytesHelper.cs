﻿namespace Smart.IO.ByteMapper.Helpers
{
    using System;
    using System.Runtime.CompilerServices;

    public static class BytesHelper
    {
        //--------------------------------------------------------------------------------
        // Enum
        //--------------------------------------------------------------------------------

        public static Type GetConvertEnumType(Type type)
        {
            var targetType = type.IsNullableType() ? Nullable.GetUnderlyingType(type) : type;
            return (targetType?.IsEnum ?? false) ? targetType : null;
        }

        //--------------------------------------------------------------------------------
        // Fill
        //--------------------------------------------------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Fill(byte[] byets, int index, int length, byte value)
        {
            for (var i = 0; i < length; i++)
            {
                byets[index + i] = value;
            }
        }

        //--------------------------------------------------------------------------------
        // Copy
        //--------------------------------------------------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TrimRange(byte[] buffer, ref int start, ref int size, Padding padding, byte filler)
        {
            if (padding == Padding.Left)
            {
                var end = start + size;
                while ((start < end) && (buffer[start] == filler))
                {
                    start++;
                    size--;
                }
            }
            else
            {
                while ((size > 0) && (buffer[start + size - 1] == filler))
                {
                    size--;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void CopyBytes(byte[] bytes, byte[] buffer, int index, int length, Padding padding, byte filler)
        {
            var size = bytes.Length;
            if (size >= length)
            {
                fixed (byte* pSrc = &bytes[0])
                fixed (byte* pDst = &buffer[index])
                {
                    Buffer.MemoryCopy(pSrc, pDst, length, length);
                }
            }
            else if (padding == Padding.Right)
            {
                if (size > 0)
                {
                    fixed (byte* pSrc = &bytes[0])
                    fixed (byte* pDst = &buffer[index])
                    {
                        Buffer.MemoryCopy(pSrc, pDst, size, size);
                    }
                }

                Fill(buffer, index + size, length - size, filler);
            }
            else
            {
                if (size > 0)
                {
                    fixed (byte* pSrc = &bytes[0])
                    fixed (byte* pDst = &buffer[index + length - size])
                    {
                        Buffer.MemoryCopy(pSrc, pDst, size, size);
                    }
                }

                Fill(buffer, index, length - size, filler);
            }
        }
    }
}
