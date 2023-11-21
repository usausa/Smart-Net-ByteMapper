namespace Smart.IO.ByteMapper.Helpers;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Ignore")]
public static class BytesHelper
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    private struct DoubleBit
    {
        [FieldOffset(0)]
        public double DoubleValue;
        [FieldOffset(0)]
        public long LongValue;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    private struct FloatBit
    {
        [FieldOffset(0)]
        public float FloatValue;
        [FieldOffset(0)]
        public int IntValue;
    }

    //--------------------------------------------------------------------------------
    // Convert
    //--------------------------------------------------------------------------------

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Int64ToDouble(long value)
    {
        var bit = new DoubleBit
        {
            LongValue = value
        };
        return bit.DoubleValue;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long DoubleToInt64(double value)
    {
        var bit = new DoubleBit
        {
            DoubleValue = value
        };
        return bit.LongValue;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Int32ToFloat(int value)
    {
        var bit = new FloatBit
        {
            IntValue = value
        };
        return bit.FloatValue;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int FloatToInt32(float value)
    {
        var bit = new FloatBit
        {
            FloatValue = value
        };
        return bit.IntValue;
    }

    //--------------------------------------------------------------------------------
    // Fill
    //--------------------------------------------------------------------------------

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Fill(byte[] bytes, int index, int length, byte value)
    {
        for (var i = 0; i < length; i++)
        {
            bytes[index + i] = value;
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
