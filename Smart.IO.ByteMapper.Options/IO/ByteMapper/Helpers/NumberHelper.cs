namespace Smart.IO.ByteMapper.Helpers
{
    using System;
    using System.Runtime.CompilerServices;

    internal static class NumberHelper
    {
        private const byte Minus = (byte)'-';
        private const byte Dot = (byte)'.';
        private const byte Comma = (byte)',';
        private const byte Num0 = (byte)'0';

        private const long Int64MinValueDiv10 = Int64.MinValue / 10;
        private const byte Int64MinValueMod10 = (byte)(Num0 + -(Int64.MinValue % 10));

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
                        value = (value << 3) + (value << 1) + num;
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
            FormatInt64(bytes, index, length, value, padding, zerofill, filler);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FormatInt32(byte[] bytes, int index, int length, int value, Padding padding, bool zerofill, byte filler)
        {
            FormatInt64(bytes, index, length, value, padding, zerofill, filler);
        }

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

                    ReverseBytes(pBytes, i - 1);

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

        public static bool IsDecimalLimited64Applicable(int length, byte scale, int groupSize)
        {
            return length <= 18 + (scale > 0 ? 1 : 0) + (groupSize > 0 ? (18 - scale - 1) / groupSize : 0);
        }

        public static unsafe bool TryParseDecimalLimited64(byte[] bytes, int index, int length, byte filler, out decimal value)
        {
            fixed (byte* pBytes = &bytes[index])
            {
                value = 0m;

                var i = 0;
                while ((i < length) && (*(pBytes + i) == filler))
                {
                    i++;
                }

                if (i == length)
                {
                    return false;
                }

                var negative = *(pBytes + i) == Minus;
                i += negative ? 1 : 0;

                var midlo = 0UL;
                var count = 0;
                var dotPos = -1;
                while (i < length)
                {
                    var num = *(pBytes + i) - Num0;
                    if ((num >= 0) && (num < 10))
                    {
                        midlo = (midlo << 3) + (midlo << 1) + (ulong)num;
                        count++;
                    }
                    else if ((*(pBytes + i) == Dot) && (dotPos == -1))
                    {
                        dotPos = count;
                    }
                    else if (*(pBytes + i) != Comma)
                    {
                        while ((i < length) && (*(pBytes + i) == filler))
                        {
                            i++;
                        }

                        break;
                    }

                    i++;
                }

                if (i != length)
                {
                    return false;
                }

                value = new decimal(
                    (int)(midlo & 0xFFFFFFFF),
                    (int)((midlo >> 32) & 0xFFFFFFFF),
                    0,
                    negative,
                    (byte)(dotPos < 0 ? 0 : (count - dotPos)));
                return true;
            }
        }

        public static unsafe void FormatDecimalLimited64(
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
            var decimalNum = ((ulong)(bits[1] & 0x00000000FFFFFFFF) << 32) + (ulong)(bits[0] & 0x00000000FFFFFFFF);

            fixed (byte* pBytes = &bytes[index])
            {
                if ((padding == Padding.Left) || zerofill)
                {
                    var i = length - 1;
                    var dotPos = scale > 0 ? length - scale - 1 : Int32.MaxValue;
                    var groupingCount = 0;

                    if (scale > decimalScale)
                    {
                        for (var j = 0; j < scale - decimalScale; j++)
                        {
                            *(pBytes + i--) = Num0;
                        }
                    }
                    else if (scale < decimalScale)
                    {
                        FixDecimalScale(ref decimalNum, decimalScale - scale);
                    }

                    if (decimalNum > UInt32.MaxValue)
                    {
                        while (i >= 0)
                        {
                            if (groupingCount == groupingSize)
                            {
                                *(pBytes + i--) = Comma;
                                groupingCount = 0;

                                if (i < 0)
                                {
                                    break;
                                }
                            }

                            *(pBytes + i--) = (byte)(Num0 + (decimalNum % 10));
                            decimalNum /= 10;

                            if (i < dotPos)
                            {
                                groupingCount++;
                            }
                            else if ((i == dotPos) && (i >= 0))
                            {
                                *(pBytes + i--) = Dot;
                            }

                            if (decimalNum <= UInt32.MaxValue)
                            {
                                break;
                            }
                        }
                    }

                    var decimalNum2 = (uint)decimalNum;
                    while (i >= 0)
                    {
                        if (groupingCount == groupingSize)
                        {
                            *(pBytes + i--) = Comma;
                            groupingCount = 0;

                            if (i < 0)
                            {
                                break;
                            }
                        }

                        *(pBytes + i--) = (byte)(Num0 + (decimalNum2 % 10));
                        decimalNum2 /= 10;

                        if (i < dotPos)
                        {
                            groupingCount++;
                        }
                        else if ((i == dotPos) && (i >= 0))
                        {
                            *(pBytes + i--) = Dot;

                            if ((decimalNum2 == 0) && (i >= 0))
                            {
                                *(pBytes + i--) = Num0;
                            }
                        }

                        if (decimalNum2 == 0)
                        {
                            break;
                        }
                    }

                    if (zerofill)
                    {
                        var end = negative ? 1 : 0;
                        while (i >= end)
                        {
                            if (groupingCount == groupingSize)
                            {
                                *(pBytes + i--) = Comma;
                                groupingCount = 0;

                                if (i < end)
                                {
                                    break;
                                }
                            }

                            *(pBytes + i--) = Num0;

                            groupingCount++;
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
                    var dotPos = scale > 0 ? scale : Int32.MinValue;
                    var groupingCount = 0;

                    if (scale > decimalScale)
                    {
                        for (var j = 0; j < scale - decimalScale; j++)
                        {
                            *(pBytes + i++) = Num0;
                        }
                    }
                    else if (scale < decimalScale)
                    {
                        FixDecimalScale(ref decimalNum, decimalScale - scale);
                    }

                    if (decimalNum > UInt32.MaxValue)
                    {
                        while (i < length)
                        {
                            if (groupingCount == groupingSize)
                            {
                                *(pBytes + i++) = Comma;
                                groupingCount = 0;

                                if (i >= length)
                                {
                                    break;
                                }
                            }

                            *(pBytes + i++) = (byte)(Num0 + (decimalNum % 10));
                            decimalNum /= 10;

                            if (i > dotPos)
                            {
                                groupingCount++;
                            }
                            else if ((i == dotPos) && (i < length))
                            {
                                *(pBytes + i++) = Dot;
                            }

                            if (decimalNum <= UInt32.MaxValue)
                            {
                                break;
                            }
                        }
                    }

                    var decimalNum2 = (uint)decimalNum;
                    while (i < length)
                    {
                        if (groupingCount == groupingSize)
                        {
                            *(pBytes + i++) = Comma;
                            groupingCount = 0;

                            if (i >= length)
                            {
                                break;
                            }
                        }

                        *(pBytes + i++) = (byte)(Num0 + (decimalNum2 % 10));
                        decimalNum2 /= 10;

                        if (i > dotPos)
                        {
                            groupingCount++;
                        }
                        else if ((i == dotPos) && (i < length))
                        {
                            *(pBytes + i++) = Dot;

                            if ((decimalNum2 == 0) && (i < length))
                            {
                                *(pBytes + i++) = Num0;
                            }
                        }

                        if (decimalNum2 == 0)
                        {
                            break;
                        }
                    }

                    if (negative && (i < length))
                    {
                        *(pBytes + i++) = Minus;
                    }

                    ReverseBytes(pBytes, i - 1);

                    while (i < length)
                    {
                        *(pBytes + i++) = filler;
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void FixDecimalScale(ref ulong value, int exponent)
        {
            if (value <= UInt32.MaxValue)
            {
                if (exponent >= 10)
                {
                    value = 0UL;
                    return;
                }

                var pow = 1U;
                var under = exponent == 1 ? value % 10 : 0UL;
                for (var i = 0; i < exponent; i++)
                {
                    pow *= 10;
                    if (i == exponent - 2)
                    {
                        under = ((uint)value / pow) % 10;
                    }
                }

                value = (uint)value / pow;
                value += under > 4 ? 1UL : 0;
            }
            else
            {
                if (exponent >= 20)
                {
                    value = 0UL;
                    return;
                }

                var pow = 1UL;
                var under = exponent == 1 ? value % 10 : 0UL;
                for (var i = 0; i < exponent; i++)
                {
                    pow *= 10;
                    if (i == exponent - 2)
                    {
                        under = (value / pow) % 10;
                    }
                }

                value = value / pow;
                value += under > 4 ? 1UL : 0;
            }
        }

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void ReverseBytes(byte* ptr, int length)
        {
            var start = ptr;
            var end = ptr + length;
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
