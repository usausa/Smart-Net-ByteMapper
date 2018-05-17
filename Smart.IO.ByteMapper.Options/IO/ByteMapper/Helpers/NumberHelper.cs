namespace Smart.IO.ByteMapper.Helpers
{
    using System;
    using System.Runtime.CompilerServices;

    internal static partial class NumberHelper
    {
        private const byte Minus = (byte)'-';
        private const byte Num0 = (byte)'0';
        private const byte Dot = (byte)'.';
        private const byte Comma = (byte)',';
        private const int DotDiff = Dot - Num0;
        private const int CommaDiff = Comma - Num0;

        private const int Int32MinValueDiv10 = Int32.MinValue / 10;
        private const byte Int32MinValueMod10 = Num0 + -(Int32.MinValue % 10);
        private const long Int64MinValueDiv10 = Int64.MinValue / 10;
        private const byte Int64MinValueMod10 = (byte)(Num0 + -(Int64.MinValue % 10));
        private const ulong UInt64MaxValueDiv10 = UInt64.MaxValue / 10;
        private const ulong UInt64MaxValueMod10 = UInt64.MaxValue % 10;
        private const ulong UInt64MaxValueDiv10Minus1 = UInt64MaxValueDiv10 - 1;

        private const int NegativeBitFlag = unchecked((int)0x80000000);

        //--------------------------------------------------------------------------------
        // Integer
        //--------------------------------------------------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryParseInt16(byte[] bytes, int index, int length, byte filler, out short value)
        {
            var ret = TryParseInt64(bytes, index, length, filler, out var longValue);
            value = (short)longValue;
            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryParseInt32(byte[] bytes, int index, int length, byte filler, out int value)
        {
            var ret = TryParseInt64(bytes, index, length, filler, out var longValue);
            value = (int)longValue;
            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool TryParseInt64(byte[] bytes, int index, int length, byte filler, out long value)
        {
            fixed (byte* pBytes = &bytes[index])
            {
                value = 0L;

                var i = 0;
                while ((i < length) && (*(pBytes + i) == filler))
                {
                    i++;
                }

                if (i == length)
                {
                    return false;
                }

                var sign = *(pBytes + i) == Minus ? -1 : 1;
                i += sign == -1 ? 1 : 0;

                while (i < length)
                {
                    var num = *(pBytes + i) - Num0;
                    if ((num >= 0) && (num < 10))
                    {
                        value = (value * 10) + num;
                        i++;
                    }
                    else
                    {
                        while ((i < length) && (*(pBytes + i) == filler))
                        {
                            i++;
                        }

                        break;
                    }
                }

                value *= sign;
                return i == length;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FormatInt16(byte[] bytes, int index, int length, short value, Padding padding, bool zerofill, byte filler)
        {
            FormatInt32(bytes, index, length, value, padding, zerofill, filler);
        }

        public static unsafe void FormatInt32(byte[] bytes, int index, int length, int value, Padding padding, bool zerofill, byte filler)
        {
            fixed (byte* pBytes = &bytes[index])
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
                else if (padding == Padding.Left)
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
                else
                {
                    if (negative && (i >= 0))
                    {
                        *(pBytes + i--) = Minus;
                    }

                    if (i >= 0)
                    {
                        var size = length - (i + 1);
                        Buffer.MemoryCopy(pBytes + i + 1, pBytes, size, size);

                        for (var j = size; j < length; j++)
                        {
                            *(pBytes + j) = filler;
                        }
                    }
                }
            }
        }

        public static unsafe void FormatInt64(byte[] bytes, int index, int length, long value, Padding padding, bool zerofill, byte filler)
        {
            fixed (byte* pBytes = &bytes[index])
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
                else if (padding == Padding.Left)
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
                else
                {
                    if (negative && (i >= 0))
                    {
                        *(pBytes + i--) = Minus;
                    }

                    if (i >= 0)
                    {
                        var size = length - (i + 1);
                        Buffer.MemoryCopy(pBytes + i + 1, pBytes, size, size);

                        for (var j = size; j < length; j++)
                        {
                            *(pBytes + j) = filler;
                        }
                    }
                }
            }
        }

        //--------------------------------------------------------------------------------
        // Decimal
        //--------------------------------------------------------------------------------

        private struct DecimalMantissa
        {
            private ulong lomid;

            private long hi;

            public int Lo => (int)(lomid & 0xFFFFFFFF);

            public int Mid => (int)((lomid >> 32) & 0xFFFFFFFF);

            public int Hi => (int)hi;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Multiply10AndAdd(ulong value)
            {
                if (hi == 0)
                {
                    if (lomid < UInt64MaxValueDiv10Minus1)
                    {
                        lomid = (lomid * 10) + value;
                        return true;
                    }

                    if ((lomid == UInt64MaxValueDiv10) && (value <= UInt64MaxValueMod10))
                    {
                        lomid = (lomid * 10) + value;
                        return true;
                    }
                }

                var carry2 = (uint)((lomid >> 63) & 0x00000001);
                var carry8 = (uint)((lomid >> 61) & 0x00000007);

                var shift3 = lomid << 3;
                var shift1 = lomid << 1;

                var overflow = IsOverflow(shift3, shift1);
                lomid = shift1 + shift3;

                var addCarry = !overflow && IsOverflow(lomid, value);
                lomid += value;

                hi = (hi << 3) + (hi << 1) + carry2 + carry8;

                if (overflow)
                {
                    hi++;
                }
                else if (addCarry)
                {
                    hi++;
                }

                return hi <= UInt32.MaxValue;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private bool IsOverflow(ulong value1, ulong value2)
            {
                return UInt64.MaxValue - value1 < value2;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Increment()
            {
                if (lomid < UInt64.MaxValue)
                {
                    lomid++;
                }
                else
                {
                    lomid = 0;
                    hi++;
                }
            }
        }

        public static unsafe bool TryParseDecimal(byte[] bytes, int index, int length, byte filler, out decimal value)
        {
            value = 0m;

            var mantissa = default(DecimalMantissa);
            fixed (byte* pBytes = &bytes[index])
            {
                var i = 0;
                while ((i < length) && (*(pBytes + i) == filler))
                {
                    i++;
                }

                if (i == length)
                {
                    return false;
                }

                var negative = *(pBytes + i) == '-';
                i += negative ? 1 : 0;

                var count = 0;
                var dotPos = -1;
                while (i < length)
                {
                    var num = *(pBytes + i) - 0x30;
                    if ((num >= 0) && (num < 10))
                    {
                        if (!mantissa.Multiply10AndAdd((ulong)num))
                        {
                            return false;
                        }

                        count++;
                    }
                    else if ((num == DotDiff) && (dotPos == -1))
                    {
                        dotPos = count;
                        i++;
                        break;
                    }
                    else if (*(pBytes + i) == filler)
                    {
                        i++;
                        while ((i < length) && (*(pBytes + i) == filler))
                        {
                            i++;
                        }

                        if (i != length)
                        {
                            return false;
                        }

                        break;
                    }
                    else if (num != CommaDiff)
                    {
                        return false;
                    }

                    i++;
                }

                if (dotPos != -1)
                {
                    while (i < length)
                    {
                        var num = *(pBytes + i) - 0x30;
                        if ((num >= 0) && (num < 10))
                        {
                            if (count >= 29)
                            {
                                if (num > 4)
                                {
                                    mantissa.Increment();
                                }

                                break;
                            }

                            mantissa.Multiply10AndAdd((ulong)num);
                            count++;
                        }
                        else if (*(pBytes + i) == filler)
                        {
                            i++;
                            while ((i < length) && (*(pBytes + i) == filler))
                            {
                                i++;
                            }

                            if (i != length)
                            {
                                return false;
                            }

                            break;
                        }
                        else
                        {
                            return false;
                        }

                        i++;
                    }
                }

                value = new decimal(
                    mantissa.Lo,
                    mantissa.Mid,
                    mantissa.Hi,
                    negative,
                    (byte)(dotPos == -1 ? 0 : (count - dotPos)));
                return true;
            }
        }

        public static unsafe void FormatDecimal(
            byte[] bytes,
            int index,
            int length,
            decimal value,
            byte scale,
            int groupingSize,
            Padding padding,
            bool zerofill,
            byte filler)
        {
            var bits = Decimal.GetBits(value);
            var negative = (bits[3] & NegativeBitFlag) != 0;
            var decimalScale = (bits[3] >> 16) & 0x7F;

            var lo = 0L;
            var hi = 0L;

            var bit = bits[0];
            if (bit != 0)
            {
                AddBitBlockValue(ref lo, ref hi, 0, (bit >> 0) & 0b11111111);
                AddBitBlockValue(ref lo, ref hi, 1, (bit >> 8) & 0b11111111);
                AddBitBlockValue(ref lo, ref hi, 2, (bit >> 16) & 0b11111111);
                AddBitBlockValue(ref lo, ref hi, 3, (bit >> 24) & 0b11111111);
            }

            bit = bits[1];
            if (bit != 0)
            {
                AddBitBlockValue(ref lo, ref hi, 4, (bit >> 0) & 0b11111111);
                AddBitBlockValue(ref lo, ref hi, 5, (bit >> 8) & 0b11111111);
                AddBitBlockValue(ref lo, ref hi, 6, (bit >> 16) & 0b11111111);
                AddBitBlockValue(ref lo, ref hi, 7, (bit >> 24) & 0b11111111);
            }

            bit = bits[2];
            if (bit != 0)
            {
                AddBitBlockValue(ref lo, ref hi, 8, (bit >> 0) & 0b11111111);
                AddBitBlockValue(ref lo, ref hi, 9, (bit >> 8) & 0b11111111);
                AddBitBlockValue(ref lo, ref hi, 10, (bit >> 16) & 0b11111111);
                AddBitBlockValue(ref lo, ref hi, 11, (bit >> 24) & 0b11111111);
            }

            var work = stackalloc byte[30];
            var workSize = 0;
            var workPointer = 0;

            while (lo > 0)
            {
                work[workSize++] = (byte)(lo % 10);
                lo /= 10;
            }

            if (hi > 0)
            {
                workSize = 15;
                while (hi > 0)
                {
                    work[workSize++] = (byte)(hi % 10);
                    hi /= 10;
                }
            }

            // Fix Scale
            if (scale < decimalScale)
            {
                workPointer = decimalScale - scale;
                if (work[workPointer - 1] > 4)
                {
                    var i = workPointer;
                    var carry = true;
                    while (carry && (i < workSize))
                    {
                        if (work[i] == 9)
                        {
                            work[i++] = 0;
                        }
                        else
                        {
                            work[i] += 1;
                            carry = false;
                        }
                    }

                    if (carry)
                    {
                        workSize++;
                        work[i] = 1;
                    }
                }
            }

            fixed (byte* pBytes = &bytes[index])
            {
                var i = length - 1;

                if (scale > 0)
                {
                    var dotPos = length - scale - 1;

                    var limit = Min(scale - decimalScale, i + 1);
                    for (var j = 0; j < limit; j++)
                    {
                        *(pBytes + i--) = Num0;
                    }

                    limit = Min(i - dotPos, i + 1);
                    for (var j = 0; j < limit; j++)
                    {
                        if (workPointer < workSize)
                        {
                            *(pBytes + i--) = (byte)(Num0 + work[workPointer++]);
                        }
                        else
                        {
                            *(pBytes + i--) = Num0;
                        }
                    }

                    if ((i == dotPos) && (i >= 0))
                    {
                        *(pBytes + i--) = Dot;
                    }
                }

                var groupingCount = 0;

                if ((workPointer == workSize) && (i >= 0))
                {
                    *(pBytes + i--) = Num0;
                }
                else
                {
                    if (groupingSize <= 0)
                    {
                        var limit = Min(workSize - workPointer, i + 1);
                        for (var j = 0; j < limit; j++)
                        {
                            *(pBytes + i--) = (byte)(Num0 + work[workPointer++]);
                        }
                    }
                    else
                    {
                        while ((workPointer < workSize) && (i >= 0))
                        {
                            if (groupingCount == groupingSize)
                            {
                                *(pBytes + i--) = Comma;
                                groupingCount = 0;
                            }

                            if (i >= 0)
                            {
                                *(pBytes + i--) = (byte)(Num0 + work[workPointer++]);
                                groupingCount++;
                            }
                        }
                    }
                }

                if (zerofill)
                {
                    var end = negative ? 1 : 0;
                    if (groupingSize <= 0)
                    {
                        while (i >= end)
                        {
                            *(pBytes + i--) = Num0;
                        }
                    }
                    else
                    {
                        while (i >= end)
                        {
                            if (groupingCount == groupingSize)
                            {
                                *(pBytes + i--) = Comma;
                                groupingCount = 0;
                            }

                            if (i >= end)
                            {
                                *(pBytes + i--) = Num0;
                                groupingCount++;
                            }
                        }
                    }

                    if (negative && (i >= 0))
                    {
                        *pBytes = Minus;
                    }
                }
                else if (padding == Padding.Left)
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
                else
                {
                    if (negative && (i >= 0))
                    {
                        *(pBytes + i--) = Minus;
                    }

                    if (i >= 0)
                    {
                        var size = length - (i + 1);
                        Buffer.MemoryCopy(pBytes + i + 1, pBytes, size, size);

                        for (var j = size; j < length; j++)
                        {
                            *(pBytes + j) = filler;
                        }
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AddBitBlockValue(ref long lo, ref long hi, int block, int bits)
        {
            var baseIndex = (block << 9) + (bits << 1);

            int carry;
            var value = lo + Table[baseIndex];
            if (value >= 1000000000000000L)
            {
                lo = value - 1000000000000000L;
                carry = 1;
            }
            else
            {
                lo = value;
                carry = 0;
            }

            hi = hi + Table[baseIndex + 1] + carry;
        }

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Min(int x, int y)
        {
            return x < y ? x : y;
        }
    }
}
