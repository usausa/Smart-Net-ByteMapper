namespace Smart.IO.ByteMapper.Helpers
{
    using System;
    using System.Runtime.CompilerServices;

    internal static class EncodingByteHelper
    {
        //--------------------------------------------------------------------------------
        // ASCII
        //--------------------------------------------------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe byte[] GetAsciiBytes(string str)
        {
            var length = str.Length;
            var bytes = new byte[length];

            fixed (char* pSrc = str)
            fixed (byte* pDst = &bytes[0])
            {
                var ps = pSrc;
                var pd = pDst;

                for (var i = 0; i < length; i++)
                {
                    *pd = (byte)*ps;
                    ps++;
                    pd++;
                }
            }

            return bytes;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe string GetAsciiString(byte[] bytes, int index, int length)
        {
            var str = new string('\0', length);

            fixed (byte* pSrc = &bytes[index])
            fixed (char* pDst = str)
            {
                var ps = pSrc;
                var pd = pDst;

                for (var i = 0; i < length; i++)
                {
                    *pd = (char)*ps;
                    ps++;
                    pd++;
                }
            }

            return str;
        }

        //--------------------------------------------------------------------------------
        // Unicode(UTF-16LE/.NET Internal)
        //--------------------------------------------------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe string GetUnicodeString(byte[] buffer, int index, int length, bool trim, Padding padding, char filler)
        {
            var filler1 = (byte)(filler & 0xff);
            var filler2 = (byte)((filler >> 8) & 0xff);

            if (trim)
            {
                if (padding == Padding.Left)
                {
                    var end = index + length;
                    while ((index + 1 < end) && (buffer[index] == filler1) && (buffer[index + 1] == filler2))
                    {
                        index += 2;
                        length -= 2;
                    }
                }
                else
                {
                    while ((length > 1) && (buffer[index + length - 2] == filler1) && (buffer[index + length - 1] == filler2))
                    {
                        length -= 2;
                    }
                }
            }

            if (length == 0)
            {
                return string.Empty;
            }

            var str = new string('\0', length / 2);
            fixed (byte* pSrc = &buffer[index])
            fixed (char* pDst = str)
            {
                Buffer.MemoryCopy(pSrc, pDst, length, length);
            }

            return str;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void CopyUnicodeBytes(string str, byte[] buffer, int index, int length, Padding padding, char filler)
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

                FillUnicode(buffer, index + size, length - size, filler);
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

                FillUnicode(buffer, index, length - size, filler);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FillUnicode(byte[] bytes, int index, int length, char filler)
        {
            var filler1 = (byte)(filler & 0xff);
            var filler2 = (byte)((filler >> 8) & 0xff);

            for (var i = 0; i < length; i += 2)
            {
                bytes[index++] = filler1;
                bytes[index++] = filler2;
            }
        }
    }
}
