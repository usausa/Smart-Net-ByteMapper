namespace ByteHelperTest
{
    using System;

    internal static partial class IntegerTable
    {
        public static unsafe void GetULongBuffer20(byte* pBuffer, ulong value)
        {
            fixed (byte* pSrc = &Table[0])
            {
                // 1
                var mod = value % 10000;
                value = value / 10000;
                Buffer.MemoryCopy(pSrc + (mod * 4), pBuffer, 4, 4);
                if (value > 0)
                {
                    // 2
                    mod = value % 10000;
                    value = value / 10000;
                    Buffer.MemoryCopy(pSrc + (mod * 4), pBuffer + 4, 4, 4);
                    if (value > 0)
                    {
                        // 3
                        mod = value % 10000;
                        value = value / 10000;
                        Buffer.MemoryCopy(pSrc + (mod * 4), pBuffer + 8, 4, 4);
                        if (value > 0)
                        {
                            // 4
                            mod = value % 10000;
                            value = value / 10000;
                            Buffer.MemoryCopy(pSrc + (mod * 4), pBuffer + 12, 4, 4);
                            if (value > 0)
                            {
                                // 5
                                mod = value % 10000;
                                Buffer.MemoryCopy(pSrc + (mod * 4), pBuffer + 16, 4, 4);
                            }
                        }
                    }
                }
            }
        }

        public static unsafe void GetLongBuffer16(byte* pBuffer, long value)
        {
            fixed (byte* pSrc = &Table[0])
            {
                // 1
                var mod = value % 10000;
                value = value / 10000;
                Buffer.MemoryCopy(pSrc + (mod * 4), pBuffer, 4, 4);
                if (value > 0)
                {
                    // 2
                    mod = value % 10000;
                    value = value / 10000;
                    Buffer.MemoryCopy(pSrc + (mod * 4), pBuffer + 4, 4, 4);
                    if (value > 0)
                    {
                        // 3
                        mod = value % 10000;
                        value = value / 10000;
                        Buffer.MemoryCopy(pSrc + (mod * 4), pBuffer + 8, 4, 4);
                        if (value > 0)
                        {
                            // 4
                            mod = value % 10000;
                            Buffer.MemoryCopy(pSrc + (mod * 4), pBuffer + 12, 4, 4);
                        }
                    }
                }
            }
        }

        public static void GetLongBuffer16(byte[] buffer, int offset, long value)
        {
            // 1
            var mod = (int)(value % 10000);
            value = value / 10000;
            Buffer.BlockCopy(Table, mod * 4, buffer, offset, 4);
            if (value > 0)
            {
                // 2
                mod = (int)(value % 10000);
                value = value / 10000;
                Buffer.BlockCopy(Table, mod * 4, buffer, offset + 4, 4);
                if (value > 0)
                {
                    // 3
                    mod = (int)(value % 10000);
                    value = value / 10000;
                    Buffer.BlockCopy(Table, mod * 4, buffer, offset + 8, 4);
                    if (value > 0)
                    {
                        // 4
                        mod = (int)(value % 10000);
                        Buffer.BlockCopy(Table, mod * 4, buffer, offset + 12, 4);
                    }
                }
            }
        }
    }
}
