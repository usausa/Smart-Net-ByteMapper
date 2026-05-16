namespace Smart.IO.ByteMapper.Helpers;

using System.Runtime.CompilerServices;
using System.Text;

internal static class EncodingByteHelper
{
    //--------------------------------------------------------------------------------
    // ASCII
    //--------------------------------------------------------------------------------

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] GetAsciiBytes(string str)
    {
        var bytes = new byte[str.Length];
        Ascii.FromUtf16(str, bytes, out _);
        return bytes;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetAsciiString(ReadOnlySpan<byte> bytes, int index, int length)
    {
        return Encoding.ASCII.GetString(bytes.Slice(index, length));
    }

    //--------------------------------------------------------------------------------
    // Unicode(UTF-16LE/.NET Internal)
    //--------------------------------------------------------------------------------

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe string GetUnicodeString(ReadOnlySpan<byte> buffer, int index, int length, bool trim, Padding padding, char filler)
    {
        if (trim)
        {
            var chars = System.Runtime.InteropServices.MemoryMarshal.Cast<byte, char>(buffer.Slice(index, length));
            if (padding == Padding.Left)
            {
                var idx = chars.IndexOfAnyExcept(filler);
                if (idx < 0)
                {
                    return string.Empty;
                }
                index += idx * 2;
                length -= idx * 2;
            }
            else
            {
                var idx = chars.LastIndexOfAnyExcept(filler);
                if (idx < 0)
                {
                    return string.Empty;
                }
                length = (idx + 1) * 2;
            }
        }
        if (length == 0)
        {
            return string.Empty;
        }

        var str = new string('\0', length / 2);
        fixed (byte* pBase = buffer)
        fixed (char* pDst = str)
        {
            var pSrc = pBase + index;
            Buffer.MemoryCopy(pSrc, pDst, length, length);
        }

        return str;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void CopyUnicodeBytes(string str, Span<byte> buffer, int index, int length, Padding padding, char filler)
    {
        var size = str.Length * 2;
        if (size >= length)
        {
            fixed (char* pSrc = str)
            fixed (byte* pBase = buffer)
            {
                var pDst = pBase + index;
                Buffer.MemoryCopy(pSrc, pDst, length, length);
            }
        }
        else if (padding == Padding.Right)
        {
            if (size > 0)
            {
                fixed (char* pSrc = str)
                fixed (byte* pBase = buffer)
                {
                    var pDst = pBase + index;
                    Buffer.MemoryCopy(pSrc, pDst, size, size);
                }
            }

            FillUnicode(buffer.Slice(index + size, length - size), filler);
        }
        else
        {
            if (size > 0)
            {
                fixed (char* pSrc = str)
                fixed (byte* pBase = buffer)
                {
                    var pDst = pBase + index + length - size;
                    Buffer.MemoryCopy(pSrc, pDst, size, size);
                }
            }

            FillUnicode(buffer.Slice(index, length - size), filler);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void FillUnicode(Span<byte> bytes, char filler)
    {
        System.Runtime.InteropServices.MemoryMarshal.Cast<byte, char>(bytes).Fill(filler);
    }
}
