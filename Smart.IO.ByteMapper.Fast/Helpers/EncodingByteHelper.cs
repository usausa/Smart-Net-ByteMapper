namespace Smart.IO.ByteMapper.Fast.Helpers;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

internal static class EncodingByteHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetAsciiString(ReadOnlySpan<byte> bytes, int index, int length)
    {
        return Encoding.ASCII.GetString(bytes.Slice(index, length));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetUnicodeString(ReadOnlySpan<byte> buffer, int index, int length, bool trim, Padding padding, char filler)
    {
        var chars = MemoryMarshal.Cast<byte, char>(buffer.Slice(index, length));
        if (trim)
        {
            if (padding == Padding.Left)
            {
                var idx = chars.IndexOfAnyExcept(filler);
                if (idx < 0)
                {
                    return string.Empty;
                }

                chars = chars[idx..];
            }
            else
            {
                var idx = chars.LastIndexOfAnyExcept(filler);
                if (idx < 0)
                {
                    return string.Empty;
                }

                chars = chars[..(idx + 1)];
            }
        }

        return chars.Length == 0 ? string.Empty : new string(chars);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyUnicodeBytes(string str, Span<byte> buffer, int index, int length, Padding padding, char filler)
    {
        var destination = buffer.Slice(index, length);
        var size = str.Length * 2;
        if (size >= length)
        {
            MemoryMarshal.Cast<char, byte>(str.AsSpan(0, length / 2)).CopyTo(destination);
        }
        else if (padding == Padding.Right)
        {
            if (size > 0)
            {
                MemoryMarshal.Cast<char, byte>(str.AsSpan()).CopyTo(destination[..size]);
            }

            FillUnicode(destination[size..], filler);
        }
        else
        {
            if (size > 0)
            {
                MemoryMarshal.Cast<char, byte>(str.AsSpan()).CopyTo(destination[(length - size)..]);
            }

            FillUnicode(destination[..(length - size)], filler);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void FillUnicode(Span<byte> bytes, char filler)
    {
        MemoryMarshal.Cast<byte, char>(bytes).Fill(filler);
    }
}
