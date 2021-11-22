#pragma warning disable SA1203 // Constants must appear before fields
namespace ByteHelper
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static class ByteHelper
    {
        private const byte Minus = (byte)'-';
        //private const byte Dot = (byte)'.';
        //private const byte Comma = (byte)',';
        private const byte Num0 = (byte)'0';

        private const long Int64MinValueDiv10 = Int64.MinValue / 10;
        private const byte Int64MinValueMod10 = (byte)(Num0 + -(Int64.MinValue % 10));

        //--------------------------------------------------------------------------------
        // Fill
        //--------------------------------------------------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] Fill(this byte[] array, int offset, int length, byte value)
        {
            if ((length <= 0) || array is null)
            {
                return array;
            }

            for (var i = 0; i < length; i++)
            {
                array[offset + i] = value;
            }

            return array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe byte[] FillUnsafe(this byte[] array, int offset, int length, byte value)
        {
            if ((length <= 0) || array is null)
            {
                return array;
            }

            fixed (byte* pSrc = &array[offset])
            {
                var pDst = pSrc;
                for (var i = 0; i < length; i++)
                {
                    *pDst++ = value;
                }
            }

            return array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe byte[] FillMemoryCopy(this byte[] array, int offset, int length, byte value)
        {
            // few cost
            if ((length <= 0) || array is null)
            {
                return array;
            }

            //if (length <= 32)
            //{
            //    for (var i = 0; i < length; i++)
            //    {
            //        array[offset + i] = value;
            //    }

            //    return array;
            //}

            fixed (byte* pSrc = &array[offset])
            {
                *pSrc = value;
                byte* pDst;

                int copy;
                for (copy = 1; copy <= length >> 1; copy <<= 1)
                {
                    pDst = pSrc + copy;
                    Buffer.MemoryCopy(pSrc, pDst, length - copy, copy);
                }

                pDst = pSrc + copy;
                Buffer.MemoryCopy(pSrc, pDst, length - copy, length - copy);
            }

            return array;
        }

        //--------------------------------------------------------------------------------
        // Convert
        //--------------------------------------------------------------------------------

        public static unsafe uint FloatToUIntBit1(float value)
        {
            var result = default(uint);
            Buffer.MemoryCopy((byte*)&value, (byte*)&result, 4, 4);
            return result;
        }

        public static unsafe ulong DoubleToULongBit1(double value)
        {
            var result = default(ulong);
            Buffer.MemoryCopy((byte*)&value, (byte*)&result, 8, 8);
            return result;
        }

        public static unsafe float UIntToFloatBit1(uint value)
        {
            var result = default(float);
            Buffer.MemoryCopy((byte*)&value, (byte*)&result, 4, 4);
            return result;
        }

        public static unsafe double ULongToDoubleBit1(ulong value)
        {
            var result = default(double);
            Buffer.MemoryCopy((byte*)&value, (byte*)&result, 8, 8);
            return result;
        }

        // Faster

        public static uint FloatToUIntBit2(float value)
        {
            var bit = new FloatBit { FloatValue = value };
            return bit.UIntValue;
        }

        public static ulong DoubleToULongBit2(double value)
        {
            var bit = new DoubleBit { DoubleValue = value };
            return bit.ULongValue;
        }

        public static float UIntToFloatBit2(uint value)
        {
            var bit = new FloatBit { UIntValue = value };
            return bit.FloatValue;
        }

        public static double DoubleToULongBit2(ulong value)
        {
            var bit = new DoubleBit { ULongValue = value };
            return bit.ULongValue;
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        private struct FloatBit
        {
            [FieldOffset(0)]
            public float FloatValue;
            [FieldOffset(0)]
            public uint UIntValue;
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        private struct DoubleBit
        {
            [FieldOffset(0)]
            public double DoubleValue;
            [FieldOffset(0)]
            public ulong ULongValue;
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
        public static unsafe string GetAsciiString(byte[] bytes)
        {
            var length = bytes.Length;
            var str = new string('\0', length);

            fixed (byte* pSrc = &bytes[0])
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
        // Integer
        //--------------------------------------------------------------------------------

        public static unsafe bool TryParseInt32(byte[] bytes, int index, int length, out int value)
        {
            value = 0;

            fixed (byte* pBytes = &bytes[index])
            {
                var i = 0;
                while ((i < length) && (*(pBytes + i) == ' '))
                {
                    i++;
                }

                if (i == length)
                {
                    return true;
                }

                var sign = *(pBytes + i) == '-' ? -1 : 1;
                i += sign == -1 ? 1 : 0;

                while (i < length)
                {
                    var num = *(pBytes + i) - 0x30;
                    if ((num >= 0) && (num < 10))
                    {
                        value = (value * 10) + num;
                        i++;
                    }
                    else
                    {
                        while ((i < length) && (*(pBytes + i) == ' '))
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

        public static unsafe void FormatInt32(byte[] bytes, int offset, int length, int value, Padding padding, bool withZero)
        {
            fixed (byte* pBytes = &bytes[offset])
            {
                if ((padding == Padding.Left) || withZero)
                {
                    var i = length - 1;

                    if ((value == Int32.MinValue) && (i >= 0))
                    {
                        *(pBytes + i) = 0x38;
                        i--;

                        value = -214748364;
                    }

                    var negative = value < 0;
                    if (negative)
                    {
                        value = -value;
                    }

                    while (i >= 0)
                    {
                        *(pBytes + i) = (byte)(0x30 + (value % 10));
                        i--;

                        value /= 10;
                        if (value == 0)
                        {
                            break;
                        }
                    }

                    if (withZero)
                    {
                        while (i >= (negative ? 1 : 0))
                        {
                            *(pBytes + i) = 0x30;
                            i--;
                        }

                        if (negative && (i >= 0))
                        {
                            *pBytes = 0x2D;
                        }
                    }
                    else
                    {
                        if (negative && (i >= 0))
                        {
                            *(pBytes + i) = 0x2D;
                            i--;
                        }

                        while (i >= 0)
                        {
                            *(pBytes + i) = 0x20;
                            i--;
                        }
                    }
                }
                else
                {
                    var i = 0;

                    if ((value == Int32.MinValue) && (i < length))
                    {
                        *(pBytes + i) = 0x38;
                        i++;

                        value = -214748364;
                    }

                    var negative = value < 0;
                    if (negative)
                    {
                        value = -value;
                    }

                    while (i < length)
                    {
                        *(pBytes + i) = (byte)(0x30 + (value % 10));
                        i++;

                        value /= 10;
                        if (value == 0)
                        {
                            break;
                        }
                    }

                    if (negative && (i < length))
                    {
                        *(pBytes + i) = 0x2D;
                        i++;
                    }

                    var start = pBytes;
                    var end = pBytes + i - 1;
                    while (start < end)
                    {
                        var tmp = *start;
                        *start = *end;
                        *end = tmp;
                        start++;
                        end--;
                    }

                    while (i < length)
                    {
                        *(pBytes + i) = 0x20;
                        i++;
                    }
                }
            }
        }

        //--------------------------------------------------------------------------------
        // Long
        //--------------------------------------------------------------------------------

        public static unsafe bool TryParseInt64(byte[] bytes, int index, int length, out long value)
        {
            value = 0;

            fixed (byte* pBytes = &bytes[index])
            {
                var i = 0;
                while ((i < length) && (*(pBytes + i) == ' '))
                {
                    i++;
                }

                if (i == length)
                {
                    return true;
                }

                var sign = *(pBytes + i) == '-' ? -1 : 1;
                i += sign == -1 ? 1 : 0;

                while (i < length)
                {
                    var num = *(pBytes + i) - 0x30;
                    if ((num >= 0) && (num < 10))
                    {
                        value = (value * 10) + num;
                        i++;
                    }
                    else
                    {
                        while ((i < length) && (*(pBytes + i) == ' '))
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

        //public static unsafe bool TryParseDecimal(byte[] bytes, int index, int length, out decimal value)
        //{
        //    value = 0m;

        //    var mantissa = default(DecimalMantissaInt3);
        //    fixed (byte* pBytes = &bytes[index])
        //    {
        //        var i = 0;
        //        while ((i < length) && (*(pBytes + i) == ' '))
        //        {
        //            i++;
        //        }

        //        if (i == length)
        //        {
        //            return true;
        //        }

        //        var negative = *(pBytes + i) == '-';
        //        i += negative ? 1 : 0;

        //        var count = 0;
        //        var dotPos = -1;
        //        while (i < length)
        //        {
        //            var num = *(pBytes + i) - 0x30;
        //            if ((num >= 0) && (num < 10))
        //            {
        //                if (mantissa.Multiply10AndAdd(num))
        //                {
        //                    count++;
        //                }
        //                else
        //                {
        //                    return false;
        //                }
        //            }
        //            else if ((*(pBytes + i) == '.') && (dotPos == -1))
        //            {
        //                dotPos = count;
        //            }
        //            else if (*(pBytes + i) != ',')
        //            {
        //                while ((i < length) && (*(pBytes + i) == ' '))
        //                {
        //                    i++;
        //                }

        //                break;
        //            }

        //            i++;
        //        }

        //        if (i != length)
        //        {
        //            return false;
        //        }

        //        value = new decimal(
        //            mantissa.Lo,
        //            mantissa.Mid,
        //            mantissa.Hi,
        //            negative,
        //            (byte)(dotPos == -1 ? 0 : (count - dotPos)));
        //        return true;
        //    }
        //}

        //private struct DecimalMantissaInt3
        //{
        //    public int Lo { get; private set; }

        //    public int Mid { get; private set; }

        //    public int Hi { get; private set; }

        //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //    public bool Multiply10AndAdd(int value)
        //    {
        //        var lo = Lo;
        //        var mid = Mid;
        //        var hi = Hi;

        //        var lo8 = Lo;
        //        var mid8 = Mid;
        //        var hi8 = Hi;

        //        var carryLo2 = ShiftWithCarry(ref lo, 1);
        //        var carryMid2 = ShiftWithCarry(ref mid, 1);
        //        if (ShiftWithCarry(ref hi, 1) > 0)
        //        {
        //            return false;
        //        }

        //        mid = mid | carryLo2;
        //        hi = hi | carryMid2;

        //        var carryLo8 = ShiftWithCarry(ref lo8, 3);
        //        var carryMid8 = ShiftWithCarry(ref mid8, 3);
        //        if (ShiftWithCarry(ref hi8, 3) > 0)
        //        {
        //            return false;
        //        }

        //        mid8 = mid8 | carryLo8;
        //        hi8 = hi8 | carryMid8;

        //        var carryLo = AddWithCarry(ref lo, lo8, 0);
        //        var carryMid = AddWithCarry(ref mid, mid8, carryLo);
        //        if (AddWithCarry(ref hi, hi8, carryMid) > 0)
        //        {
        //            return false;
        //        }

        //        var carry = AddWithCarry(ref lo, value);
        //        carry = AddWithCarry(ref mid, carry);
        //        if (AddWithCarry(ref hi, carry) > 0)
        //        {
        //            return false;
        //        }

        //        Lo = lo;
        //        Mid = mid;
        //        Hi = hi;

        //        return true;
        //    }

        //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //    private static int AddWithCarry(ref int value, int add)
        //    {
        //        var l = (ulong)(uint)value;
        //        l += (ulong)add;
        //        value = (int)(l & 0xFFFFFFFF);
        //        return (int)((l >> 32) & 0xFFFFFFFF);
        //    }

        //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //    private static int AddWithCarry(ref int value, int add1, int add2)
        //    {
        //        var l = (ulong)(uint)value;
        //        l += (uint)add1;
        //        l += (uint)add2;
        //        value = (int)(l & 0xFFFFFFFF);
        //        return (int)((l >> 32) & 0xFFFFFFFF);
        //    }

        //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //    private static int ShiftWithCarry(ref int value, int bit)
        //    {
        //        var l = (ulong)(uint)value;
        //        l = l << bit;
        //        value = (int)(l & 0xFFFFFFFF);
        //        return (int)((l >> 32) & 0xFFFFFFFF);
        //    }
        //}

        // TypeB

        // TODO Slow
        //public static unsafe bool TryParseDecimal3(byte[] bytes, int index, int length, out decimal value)
        //{
        //    value = 0m;

        //    fixed (byte* pBytes = &bytes[index])
        //    {
        //        var i = 0;
        //        while ((i < length) && (*(pBytes + i) == ' '))
        //        {
        //            i++;
        //        }

        //        if (i == length)
        //        {
        //            return true;
        //        }

        //        var negative = *(pBytes + i) == '-';
        //        i += negative ? 1 : 0;

        //        var nantissa = BigInteger.Zero;
        //        var count = 0;
        //        var dotPos = -1;
        //        while (i < length)
        //        {
        //            var num = *(pBytes + i) - 0x30;
        //            if ((num >= 0) && (num < 10))
        //            {
        //                nantissa = (nantissa * 10) + num;
        //                count++;
        //            }
        //            else if ((*(pBytes + i) == '.') && (dotPos == -1))
        //            {
        //                dotPos = count;
        //            }
        //            else if (*(pBytes + i) != ',')
        //            {
        //                while ((i < length) && (*(pBytes + i) == ' '))
        //                {
        //                    i++;
        //                }

        //                break;
        //            }

        //            i++;
        //        }

        //        if (i != length)
        //        {
        //            return false;
        //        }

        //        value = new decimal(
        //            (int)(uint)(nantissa & 0xFFFFFFFF),
        //            (int)(uint)((nantissa >> 32) & 0xFFFFFFFF),
        //            (int)(uint)((nantissa >> 64) & 0xFFFFFFFF),
        //            negative,
        //            (byte)(dotPos == -1 ? 0 : (count - dotPos)));
        //        return true;
        //    }
        //}

        // TODO Slow
        //public static unsafe void FormatDecimal(byte[] bytes, int offset, int length, decimal value, Padding padding, bool withZero)
        //{
        //    fixed (byte* pBytes = &bytes[offset])
        //    {
        //        if ((padding == Padding.Left) || withZero)
        //        {
        //            var i = length - 1;

        //            var negative = value < 0;
        //            if (negative)
        //            {
        //                value = -value;
        //            }

        //            while (i >= 0)
        //            {
        //                *(pBytes + i) = (byte)(0x30 + (value % 10));
        //                i--;

        //                value = decimal.Floor(value / 10);
        //                if (value == 0m)
        //                {
        //                    break;
        //                }
        //            }

        //            if (withZero)
        //            {
        //                while (i >= (negative ? 1 : 0))
        //                {
        //                    *(pBytes + i) = 0x30;
        //                    i--;
        //                }

        //                if (negative && (i >= 0))
        //                {
        //                    *pBytes = 0x2D;
        //                }
        //            }
        //            else
        //            {
        //                if (negative && (i >= 0))
        //                {
        //                    *(pBytes + i) = 0x2D;
        //                    i--;
        //                }

        //                while (i >= 0)
        //                {
        //                    *(pBytes + i) = 0x20;
        //                    i--;
        //                }
        //            }

        //        }

        //        // ...
        //    }
        //}

        //--------------------------------------------------------------------------------
        // Reverse
        //--------------------------------------------------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Reverse(byte[] bytes, int index, int length)
        {
            var start = index;
            var end = index + length - 1;
            while (start < end)
            {
                var tmp = bytes[start];
                bytes[start] = bytes[end];
                bytes[end] = tmp;
                start++;
                end--;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void ReverseUnsafe(byte[] bytes, int index, int length)
        {
            fixed (byte* ptr = &bytes[index])
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
#pragma warning restore SA1203 // Constants must appear before fields
