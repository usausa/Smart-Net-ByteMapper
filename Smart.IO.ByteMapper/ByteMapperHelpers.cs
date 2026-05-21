namespace Smart.IO.ByteMapper;

using System;
using System.Runtime.CompilerServices;

public static class ByteMapperHelpers
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Fill(Span<byte> destination, byte value) =>
        destination.Fill(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void TrimRange(ReadOnlySpan<byte> buffer, ref int start, ref int size, Padding padding, byte filler)
    {
        if (padding == Padding.Left)
        {
            var slice = buffer.Slice(start, size);
            var idx = slice.IndexOfAnyExcept(filler);
            if (idx < 0)
            {
                start += size;
                size = 0;
            }
            else
            {
                start += idx;
                size -= idx;
            }
        }
        else
        {
            var slice = buffer.Slice(start, size);
            var idx = slice.LastIndexOfAnyExcept(filler);
            size = idx < 0 ? 0 : idx + 1;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyWithPadding(ReadOnlySpan<byte> source, Span<byte> destination, int length, Padding padding, byte filler)
    {
        var size = source.Length;
        if (size >= length)
        {
            source[..length].CopyTo(destination[..length]);
        }
        else if (padding == Padding.Right)
        {
            if (size > 0)
            {
                source.CopyTo(destination[..size]);
            }
            destination[size..length].Fill(filler);
        }
        else
        {
            if (size > 0)
            {
                source.CopyTo(destination[(length - size)..length]);
            }
            destination[..(length - size)].Fill(filler);
        }
    }
}
