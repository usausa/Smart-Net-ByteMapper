namespace ByteHelperTest
{
    using System;
    using System.Runtime.CompilerServices;

    public static class ByteHelper2
    {
        private const byte Minus = (byte)'-';
        private const byte Num0 = (byte)'0';
        //private const byte Dot = (byte)'.';
        //private const byte Comma = (byte)',';
        //private const int DotDiff = Dot - Num0;
        //private const int CommaDiff = Comma - Num0;

        private const int Int32MinValueDiv10 = Int32.MinValue / 10;
        private const byte Int32MinValueMod10 = Num0 + -(Int32.MinValue % 10);
        private const long Int64MinValueDiv10 = Int64.MinValue / 10;
        private const byte Int64MinValueMod10 = (byte)(Num0 + -(Int64.MinValue % 10));

        //private const int NegativeBitFlag = unchecked((int)0x80000000);

        //--------------------------------------------------------------------------------
        // Integer
        //--------------------------------------------------------------------------------

        // Int32

        public static unsafe void FormatInt32WithTable(byte[] bytes, int index, int length, int value, Padding padding, bool zerofill, byte filler)
        {
            fixed (byte* pBytes = &bytes[index])
            {
                var i = 0;

                if ((value == Int32.MinValue) && (i < length))
                {
                    *(pBytes + i++) = Int32MinValueMod10;
                    value = Int32MinValueDiv10;
                }

                var negative = value < 0;
                if (negative)
                {
                    value = -value;
                }

                var work = stackalloc byte[12];
                var writed = DigitTable.GetIntBuffer12(work, value);

                var copy = writed > length - i ? length - i : writed;
                Buffer.MemoryCopy(work, pBytes + i, copy, copy);
                i += copy;

                if (zerofill)
                {
                    var max = negative ? length - 1 : length;
                    while (i < max)
                    {
                        *(pBytes + i++) = Num0;
                    }

                    if (negative && (i < length))
                    {
                        *(pBytes + i) = Minus;
                    }

                    ReverseBytes(pBytes, length);
                }
                else if (padding == Padding.Left)
                {
                    if (negative && (i < length))
                    {
                        *(pBytes + i++) = Minus;
                    }

                    while (i < length)
                    {
                        *(pBytes + i++) = filler;
                    }

                    ReverseBytes(pBytes, length);
                }
                else
                {
                    if (negative && (i < length))
                    {
                        *(pBytes + i++) = Minus;
                    }

                    ReverseBytes(pBytes, i);

                    while (i < length)
                    {
                        *(pBytes + i++) = filler;
                    }
                }
            }
        }

        public static unsafe void FormatInt32(byte[] bytes, int index, int length, int value, Padding padding, bool zerofill, byte filler)
        {
            fixed (byte* pBytes = &bytes[index])
            {
                if ((padding == Padding.Left) || zerofill)
                {
                    var i = length - 1;

                    if ((value == Int32.MinValue) && (i >= 0))
                    {
                        *(pBytes + i--) = Int32MinValueMod10;
                        value = Int32MinValueDiv10;
                    }

                    var negative = value < 0;
                    if (negative)
                    {
                        value = -value;
                    }

                    while (i >= 0)
                    {
                        *(pBytes + i--) = (byte)(Num0 + (value % 10));

                        value /= 10;
                        if (value == 0)
                        {
                            break;
                        }
                    }

                    if (zerofill)
                    {
                        while (i >= (negative ? 1 : 0))
                        {
                            *(pBytes + i--) = Num0;
                        }

                        if (negative && (i >= 0))
                        {
                            *pBytes = Minus;
                        }
                    }
                    else
                    {
                        if (negative && (i >= 0))
                        {
                            *(pBytes + i--) = Minus;
                        }

                        while (i >= 0)
                        {
                            *(pBytes + i--) = filler;
                        }
                    }
                }
                else
                {
                    var i = 0;

                    if ((value == Int32.MinValue) && (i < length))
                    {
                        *(pBytes + i++) = Int32MinValueMod10;
                        value = Int32MinValueDiv10;
                    }

                    var negative = value < 0;
                    if (negative)
                    {
                        value = -value;
                    }

                    while (i < length)
                    {
                        *(pBytes + i++) = (byte)(Num0 + (value % 10));

                        value /= 10;
                        if (value == 0)
                        {
                            break;
                        }
                    }

                    if (negative && (i < length))
                    {
                        *(pBytes + i++) = Minus;
                    }

                    ReverseBytes(pBytes, i);

                    while (i < length)
                    {
                        *(pBytes + i++) = filler;
                    }
                }
            }
        }

        // Int 64

        public static unsafe void FormatInt64(byte[] bytes, int index, int length, long value, Padding padding, bool zerofill, byte filler)
        {
            fixed (byte* pBytes = &bytes[index])
            {
                if ((padding == Padding.Left) || zerofill)
                {
                    var i = length - 1;

                    if ((value == Int64.MinValue) && (i >= 0))
                    {
                        *(pBytes + i--) = Int64MinValueMod10;
                        value = Int64MinValueDiv10;
                    }

                    var negative = value < 0;
                    if (negative)
                    {
                        value = -value;
                    }

                    while (i >= 0)
                    {
                        *(pBytes + i--) = (byte)(Num0 + (value % 10));

                        value /= 10;
                        if (value == 0)
                        {
                            break;
                        }
                    }

                    if (zerofill)
                    {
                        while (i >= (negative ? 1 : 0))
                        {
                            *(pBytes + i--) = Num0;
                        }

                        if (negative && (i >= 0))
                        {
                            *pBytes = Minus;
                        }
                    }
                    else
                    {
                        if (negative && (i >= 0))
                        {
                            *(pBytes + i--) = Minus;
                        }

                        while (i >= 0)
                        {
                            *(pBytes + i--) = filler;
                        }
                    }
                }
                else
                {
                    var i = 0;

                    if ((value == Int64.MinValue) && (i < length))
                    {
                        *(pBytes + i++) = Int64MinValueMod10;
                        value = Int64MinValueDiv10;
                    }

                    var negative = value < 0;
                    if (negative)
                    {
                        value = -value;
                    }

                    while (i < length)
                    {
                        *(pBytes + i++) = (byte)(Num0 + (value % 10));

                        value /= 10;
                        if (value == 0)
                        {
                            break;
                        }
                    }

                    if (negative && (i < length))
                    {
                        *(pBytes + i++) = Minus;
                    }

                    ReverseBytes(pBytes, i);

                    while (i < length)
                    {
                        *(pBytes + i++) = filler;
                    }
                }
            }
        }

        public static unsafe void FormatInt64WithTable(byte[] bytes, int index, int length, long value, Padding padding, bool zerofill, byte filler)
        {
            fixed (byte* pBytes = &bytes[index])
            {
                var i = 0;

                if ((value == Int64.MinValue) && (i < length))
                {
                    *(pBytes + i++) = Int64MinValueMod10;
                    value = Int64MinValueDiv10;
                }

                var negative = value < 0;
                if (negative)
                {
                    value = -value;
                }

                var work = stackalloc byte[20];
                var writed = DigitTable.GetLongBuffer20(work, value);

                var copy = writed > length - i ? length - i : writed;
                Buffer.MemoryCopy(work, pBytes + i, copy, copy);
                i += copy;

                if (zerofill)
                {
                    var max = negative ? length - 1 : length;
                    while (i < max)
                    {
                        *(pBytes + i++) = Num0;
                    }

                    if (negative && (i < length))
                    {
                        *(pBytes + i) = Minus;
                    }

                    ReverseBytes(pBytes, length);
                }
                else if (padding == Padding.Left)
                {
                    if (negative && (i < length))
                    {
                        *(pBytes + i++) = Minus;
                    }

                    while (i < length)
                    {
                        *(pBytes + i++) = filler;
                    }

                    ReverseBytes(pBytes, length);
                }
                else
                {
                    if (negative && (i < length))
                    {
                        *(pBytes + i++) = Minus;
                    }

                    ReverseBytes(pBytes, i);

                    while (i < length)
                    {
                        *(pBytes + i++) = filler;
                    }
                }
            }
        }

        //--------------------------------------------------------------------------------
        // Decimal
        //--------------------------------------------------------------------------------

        public static unsafe void FormatDecimal(
            byte[] bytes,
            int offset,
            int length,
            decimal value)
        {
            var bits = Decimal.GetBits(value);

            var lo = 0L;
            var hi = 0L;
            DecimalTable.AddBitBlockValue(ref lo, ref hi, 0, (bits[0] >> 0) & 0b11111111);
            DecimalTable.AddBitBlockValue(ref lo, ref hi, 1, (bits[0] >> 8) & 0b11111111);
            DecimalTable.AddBitBlockValue(ref lo, ref hi, 2, (bits[0] >> 16) & 0b11111111);
            DecimalTable.AddBitBlockValue(ref lo, ref hi, 3, (bits[0] >> 24) & 0b11111111);
            DecimalTable.AddBitBlockValue(ref lo, ref hi, 4, (bits[1] >> 0) & 0b11111111);
            DecimalTable.AddBitBlockValue(ref lo, ref hi, 5, (bits[1] >> 8) & 0b11111111);
            DecimalTable.AddBitBlockValue(ref lo, ref hi, 6, (bits[1] >> 16) & 0b11111111);
            DecimalTable.AddBitBlockValue(ref lo, ref hi, 7, (bits[1] >> 24) & 0b11111111);
            DecimalTable.AddBitBlockValue(ref lo, ref hi, 8, (bits[2] >> 0) & 0b11111111);
            DecimalTable.AddBitBlockValue(ref lo, ref hi, 9, (bits[2] >> 8) & 0b11111111);
            DecimalTable.AddBitBlockValue(ref lo, ref hi, 10, (bits[2] >> 16) & 0b11111111);
            DecimalTable.AddBitBlockValue(ref lo, ref hi, 11, (bits[2] >> 24) & 0b11111111);

            fixed (byte* pBytes = &bytes[offset])
            {
                var writed = 0;

                // Lo
                while ((lo > 0) && (writed < length))
                {
                    pBytes[offset + writed] = (byte)((lo % 10) + 0x30);
                    lo /= 10;
                    writed++;
                }

                var last = length < 16 ? length : 16;
                while (writed < last)
                {
                    pBytes[offset + writed] = 0x30;
                    writed++;
                }

                // Hi
                while ((hi > 0) && (writed < length))
                {
                    pBytes[offset + writed] = (byte)((hi % 10) + 0x30);
                    hi /= 10;
                    writed++;
                }

                last = length < 29 ? length : 29;
                while (writed < last)
                {
                    pBytes[offset + writed] = 0x30;
                    writed++;
                }

                // Etc
                while (writed < length)
                {
                    pBytes[offset + writed] = 0x30;
                    writed++;
                }

                ReverseBytes(pBytes, length);
            }
        }

        public static unsafe void FormatDecimalWithTable(
            byte[] bytes,
            int offset,
            int length,
            decimal value)
        {
            var bits = Decimal.GetBits(value);

            var lo = 0L;
            var hi = 0L;
            DecimalTable.AddBitBlockValue(ref lo, ref hi, 0, (bits[0] >> 0) & 0b11111111);
            DecimalTable.AddBitBlockValue(ref lo, ref hi, 1, (bits[0] >> 8) & 0b11111111);
            DecimalTable.AddBitBlockValue(ref lo, ref hi, 2, (bits[0] >> 16) & 0b11111111);
            DecimalTable.AddBitBlockValue(ref lo, ref hi, 3, (bits[0] >> 24) & 0b11111111);
            DecimalTable.AddBitBlockValue(ref lo, ref hi, 4, (bits[1] >> 0) & 0b11111111);
            DecimalTable.AddBitBlockValue(ref lo, ref hi, 5, (bits[1] >> 8) & 0b11111111);
            DecimalTable.AddBitBlockValue(ref lo, ref hi, 6, (bits[1] >> 16) & 0b11111111);
            DecimalTable.AddBitBlockValue(ref lo, ref hi, 7, (bits[1] >> 24) & 0b11111111);
            DecimalTable.AddBitBlockValue(ref lo, ref hi, 8, (bits[2] >> 0) & 0b11111111);
            DecimalTable.AddBitBlockValue(ref lo, ref hi, 9, (bits[2] >> 8) & 0b11111111);
            DecimalTable.AddBitBlockValue(ref lo, ref hi, 10, (bits[2] >> 16) & 0b11111111);
            DecimalTable.AddBitBlockValue(ref lo, ref hi, 11, (bits[2] >> 24) & 0b11111111);

            var work = stackalloc byte[32];
            var writed = DigitTable.GetLongBuffer16(work, lo);
            writed += DigitTable.GetLongBuffer16(work + 16, hi);

            fixed (byte* pBytes = &bytes[offset])
            {
                var copy = writed > length ? length : writed;
                Buffer.MemoryCopy(work, pBytes, copy, copy);

                for (var i = writed; i < length; i++)
                {
                    pBytes[offset + i] = 0x30;
                }

                ReverseBytes(pBytes, length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void ReverseBytes(byte* ptr, int length)
        {
            var start = ptr;
            var end = ptr + length - 1;
            while (start < end)
            {
                var tmp = *start;
                *start = *end;
                *end = tmp;
                start++;
                end--;
            }
        }
    }
}
