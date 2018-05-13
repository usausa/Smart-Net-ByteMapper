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
                        midlo = (midlo * 10) + (ulong)num;
                        count++;
                    }
                    else if ((num == DotDiff) && (dotPos == -1))
                    {
                        dotPos = count;
                    }
                    else if (num != CommaDiff)
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

            var work = stackalloc byte[30];
            var workSize = 0;
            var workPointer = 0;

            while (decimalNum > Int64.MaxValue)
            {
                work[workSize++] = (byte)(decimalNum % 10);
                decimalNum /= 10;
            }

            var decimalNum2 = (long)decimalNum;
            while (decimalNum2 > 0)
            {
                work[workSize++] = (byte)(decimalNum2 % 10);
                decimalNum2 /= 10;
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
                if ((padding == Padding.Left) || zerofill)
                {
                    var i = length - 1;

                    if (scale > 0)
                    {
                        var dotPos = length - scale - 1;

                        var completion = scale - decimalScale;
                        while ((completion > 0) && (i >= 0))
                        {
                            *(pBytes + i--) = Num0;
                            completion--;
                        }

                        while ((i > dotPos) && (i >= 0))
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
                        while ((workPointer < workSize) && (i >= 0))
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

                            *(pBytes + i--) = (byte)(Num0 + work[workPointer++]);

                            groupingCount++;
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

                    if (scale > 0)
                    {
                        var dotPos = scale;

                        var completion = scale - decimalScale;
                        while ((completion > 0) && (i < length))
                        {
                            *(pBytes + i++) = Num0;
                            completion--;
                        }

                        while ((i < dotPos) && (i < length))
                        {
                            if (workPointer < workSize)
                            {
                                *(pBytes + i++) = (byte)(Num0 + work[workPointer++]);
                            }
                            else
                            {
                                *(pBytes + i++) = Num0;
                            }
                        }

                        if ((i == dotPos) && (i < length))
                        {
                            *(pBytes + i++) = Dot;
                        }
                    }

                    var groupingCount = 0;

                    if ((workPointer == workSize) && (i < length))
                    {
                        *(pBytes + i++) = Num0;
                    }
                    else
                    {
                        while ((workPointer < workSize) && (i < length))
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

                            *(pBytes + i++) = (byte)(Num0 + work[workPointer++]);

                            groupingCount++;
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

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

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
