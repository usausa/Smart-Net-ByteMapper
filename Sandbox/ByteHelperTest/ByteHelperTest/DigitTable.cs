namespace ByteHelperTest
{
    using System;

    internal static partial class DigitTable
    {
        public static unsafe int GetIntBuffer12(byte* pBuffer, int value)
        {
            fixed (byte* pSrc = &Table[0])
            {
                int length;

                // 1
                var offset = (value % 10000) * 5;
                value = value / 10000;

                if (value == 0)
                {
                    length = *(pSrc + offset + 4);
                    Buffer.MemoryCopy(pSrc + offset, pBuffer, length, length);
                    return length;
                }

                Buffer.MemoryCopy(pSrc + offset, pBuffer, 4, 4);

                // 2
                offset = (value % 10000) * 5;
                value = value / 10000;

                if (value == 0)
                {
                    length = *(pSrc + offset + 4);
                    Buffer.MemoryCopy(pSrc + offset, pBuffer + 4, length, length);
                    return 4 + length;
                }

                Buffer.MemoryCopy(pSrc + offset, pBuffer + 4, 4, 4);

                // 3
                offset = (value % 10000) * 5;

                length = *(pSrc + offset + 4);
                Buffer.MemoryCopy(pSrc + offset, pBuffer + 8, length, length);
                return 8 + length;
            }
        }

        public static unsafe int GetLongBuffer20(byte* pBuffer, long value)
        {
            fixed (byte* pSrc = &Table[0])
            {
                int length;

                // 1
                var offset = (value % 10000) * 5;
                value = value / 10000;

                if (value == 0)
                {
                    length = *(pSrc + offset + 4);
                    Buffer.MemoryCopy(pSrc + offset, pBuffer, length, length);
                    return length;
                }

                Buffer.MemoryCopy(pSrc + offset, pBuffer, 4, 4);

                // 2
                offset = (value % 10000) * 5;
                value = value / 10000;

                if (value == 0)
                {
                    length = *(pSrc + offset + 4);
                    Buffer.MemoryCopy(pSrc + offset, pBuffer + 4, length, length);
                    return 4 + length;
                }

                Buffer.MemoryCopy(pSrc + offset, pBuffer + 4, 4, 4);

                // 3
                offset = (value % 10000) * 5;
                value = value / 10000;

                if (value == 0)
                {
                    length = *(pSrc + offset + 4);
                    Buffer.MemoryCopy(pSrc + offset, pBuffer + 8, length, length);
                    return 8 + length;
                }

                Buffer.MemoryCopy(pSrc + offset, pBuffer + 8, 4, 4);

                // 4
                offset = (value % 10000) * 5;
                value = value / 10000;

                if (value == 0)
                {
                    length = *(pSrc + offset + 4);
                    Buffer.MemoryCopy(pSrc + offset, pBuffer + 12, length, length);
                    return 12 + length;
                }

                Buffer.MemoryCopy(pSrc + offset, pBuffer + 12, 4, 4);

                // 5
                offset = (value % 10000) * 5;

                length = *(pSrc + offset + 4);
                Buffer.MemoryCopy(pSrc + offset, pBuffer + 16, length, length);
                return 16 + length;
            }
        }

        public static unsafe int GetLongBuffer16(byte* pBuffer, long value)
        {
            fixed (byte* pSrc = &Table[0])
            {
                int length;

                // 1
                var offset = (value % 10000) * 5;
                value = value / 10000;

                if (value == 0)
                {
                    length = *(pSrc + offset + 4);
                    Buffer.MemoryCopy(pSrc + offset, pBuffer, length, length);
                    return length;
                }

                Buffer.MemoryCopy(pSrc + offset, pBuffer, 4, 4);

                // 2
                offset = (value % 10000) * 5;
                value = value / 10000;

                if (value == 0)
                {
                    length = *(pSrc + offset + 4);
                    Buffer.MemoryCopy(pSrc + offset, pBuffer + 4, length, length);
                    return 4 + length;
                }

                Buffer.MemoryCopy(pSrc + offset, pBuffer + 4, 4, 4);

                // 3
                offset = (value % 10000) * 5;
                value = value / 10000;

                if (value == 0)
                {
                    length = *(pSrc + offset + 4);
                    Buffer.MemoryCopy(pSrc + offset, pBuffer + 8, length, length);
                    return 8 + length;
                }

                Buffer.MemoryCopy(pSrc + offset, pBuffer + 8, 4, 4);

                // 4
                offset = (value % 10000) * 5;

                length = *(pSrc + offset + 4);
                Buffer.MemoryCopy(pSrc + offset, pBuffer + 12, length, length);
                return 12 + length;
            }
        }
    }
}
