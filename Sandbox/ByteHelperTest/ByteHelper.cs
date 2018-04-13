namespace ByteHelperTest
{
    using System;
    using System.Runtime.CompilerServices;

    public static class ByteHelper
    {
        //--------------------------------------------------------------------------------
        // Fill
        //--------------------------------------------------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] Fill(this byte[] array, int offset, int length, byte value)
        {
            if ((length <= 0) || (array == null))
            {
                return array;
            }

            array[offset] = value;

            int copy;
            for (copy = 1; copy <= length / 2; copy <<= 1)
            {
                Buffer.BlockCopy(array, offset, array, offset + copy, copy);
            }

            Buffer.BlockCopy(array, offset, array, offset + copy, length - copy);

            return array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe byte[] FillUnsafe(this byte[] array, int offset, int length, byte value)
        {
            // few cost
            if ((length <= 0) || (array == null))
            {
                return array;
            }

            fixed (byte* pSrc = &array[offset])
            {
                pSrc[offset] = value;

                int copy;
                for (copy = 1; copy <= length / 2; copy <<= 1)
                {
                    fixed (byte* pDst = &array[offset + copy])
                    {
                        Buffer.MemoryCopy(pSrc, pDst, length - copy, copy);
                    }
                }

                fixed (byte* pDst = &array[offset + copy])
                {
                    Buffer.MemoryCopy(pSrc, pDst, length - copy, length - copy);
                }
            }

            return array;
        }

        //--------------------------------------------------------------------------------
        // Encoding
        //--------------------------------------------------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe byte[] GetAsciiBytes(string str)
        {
            var length = str.Length;
            var bytes = new byte[length];

            fixed (char* pSrc = str)
            fixed (byte* pDst = &bytes[0])
            {
                for (var i = 0; i < length; i++)
                {
                    pDst[i] = (byte)pSrc[i];
                }
            }

            return bytes;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe string GetAsciiString(byte[] bytes)
        {
            var length = bytes.Length;

            fixed (byte* pSrc = &bytes[0])
            {
                char* pDst = stackalloc char[length];

                for (var i = 0; i < length; i++)
                {
                    pDst[i] = (char)pSrc[i];
                }

                return new string(pDst);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe string GetAsciiString2(byte[] bytes)
        {
            // Faster than stackalloc ?
            var length = bytes.Length;
            var chars = new char[length];

            fixed (byte* pSrc = &bytes[0])
            fixed (char* pDst = &chars[0])
            {
                for (var i = 0; i < length; i++)
                {
                    pDst[i] = (char)pSrc[i];
                }
            }

            return new string(chars);
        }

        //--------------------------------------------------------------------------------
        // Integer
        //--------------------------------------------------------------------------------

        // TODO format check

        // TODO parse 1, 2, 3

        // TODO format

        //// TODO check, minus, trim ?
        //public static int ParseInteger(byte[] bytes, int index, int count)
        //{
        //    var end = index + count;

        //    // No check, simple version
        //    var value = 0;
        //    for (var i = index; i < end; i++)
        //    {
        //        value *= 10;
        //        value += bytes[i] - '0';
        //    }

        //    return value;
        //}

        //--------------------------------------------------------------------------------
        // Decimal
        //--------------------------------------------------------------------------------

        // TODO format check

        // TODO parse 1, 2, 3

        // TODO format

        //--------------------------------------------------------------------------------
        // DateTime
        //--------------------------------------------------------------------------------

        // TODO format check

        // TODO parse

        // TODO format

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        private const long InvDivisor = 0x1999999A;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Div10Signed(int dividend)
        {
            // signed only
            return (int)((InvDivisor * dividend) >> 32);
        }
    }
}
