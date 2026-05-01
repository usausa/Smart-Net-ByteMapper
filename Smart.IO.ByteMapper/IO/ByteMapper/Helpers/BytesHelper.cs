namespace Smart.IO.ByteMapper.Helpers;

using System.Runtime.CompilerServices;

#pragma warning disable CA1062
public static class BytesHelper
{
    //--------------------------------------------------------------------------------
    // Convert
    //--------------------------------------------------------------------------------

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Int64ToDouble(long value)
    {
        return Unsafe.As<long, double>(ref value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long DoubleToInt64(double value)
    {
        return Unsafe.As<double, long>(ref value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Int32ToFloat(int value)
    {
        return Unsafe.As<int, float>(ref value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int FloatToInt32(float value)
    {
        return Unsafe.As<float, int>(ref value);
    }

    //--------------------------------------------------------------------------------
    // Fill
    //--------------------------------------------------------------------------------

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Fill(Span<byte> bytes, byte value)
    {
        bytes.Fill(value);
    }

    //--------------------------------------------------------------------------------
    // Copy
    //--------------------------------------------------------------------------------

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void TrimRange(ReadOnlySpan<byte> buffer, ref int start, ref int size, Padding padding, byte filler)
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
    public static void CopyBytes(ReadOnlySpan<byte> bytes, Span<byte> buffer, int length, Padding padding, byte filler)
    {
        var size = bytes.Length;
        if (size >= length)
        {
            bytes[..length].CopyTo(buffer[..length]);
        }
        else if (padding == Padding.Right)
        {
            if (size > 0)
            {
                bytes.CopyTo(buffer[..size]);
            }

            Fill(buffer[size..length], filler);
        }
        else
        {
            if (size > 0)
            {
                bytes.CopyTo(buffer[(length - size)..length]);
            }

            Fill(buffer[..(length - size)], filler);
        }
    }
}
