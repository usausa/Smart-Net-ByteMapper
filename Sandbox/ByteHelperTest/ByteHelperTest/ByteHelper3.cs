namespace ByteHelperTest;

using System;
using System.Runtime.CompilerServices;

internal static class ByteHelper3
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe string GetUtf16String(byte[] buffer, int index, int length, Padding padding, char filler)
    {
        var filler1 = (byte)(filler & 0xff);
        var filler2 = (byte)((filler >> 8) & 0xff);

        var start = index;
        if (padding == Padding.Left)
        {
            var end = start + length;
            while ((start + 1 < end) && (buffer[start] == filler1) && (buffer[start + 1] == filler2))
            {
                start += 2;
                length -= 2;
            }
        }
        else
        {
            while ((length > 1) && (buffer[start + length - 2] == filler1) && (buffer[start + length - 1] == filler2))
            {
                length -= 2;
            }
        }

        if (length == 0)
        {
            return string.Empty;
        }

        var str = new string('\0', length / 2);
        fixed (byte* pSrc = &buffer[start])
        fixed (char* pDst = str)
        {
            Buffer.MemoryCopy(pSrc, pDst, length, length);
        }

        return str;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void CopyUtf16Bytes(string str, byte[] buffer, int index, int length, Padding padding, char filler)
    {
        var size = str.Length * 2;
        if (size >= length)
        {
            fixed (char* pSrc = str)
            fixed (byte* pDst = &buffer[index])
            {
                Buffer.MemoryCopy(pSrc, pDst, length, length);
            }
        }
        else if (padding == Padding.Right)
        {
            if (size > 0)
            {
                fixed (char* pSrc = str)
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
                fixed (char* pSrc = str)
                fixed (byte* pDst = &buffer[index + length - size])
                {
                    Buffer.MemoryCopy(pSrc, pDst, size, size);
                }
            }

            Fill(buffer, index, length - size, filler);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Fill(byte[] byets, int index, int length, char filler)
    {
        var filler1 = (byte)(filler & 0xff);
        var filler2 = (byte)((filler >> 8) & 0xff);

        for (var i = 0; i < length; i += 2)
        {
            byets[index++] = filler1;
            byets[index++] = filler2;
        }
    }
}
