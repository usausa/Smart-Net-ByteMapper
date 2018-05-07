#pragma warning disable SA1203 // Constants must appear before fields
namespace ByteHelperTest
{
    using System;
    //using System.Numerics;
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
            if ((length <= 0) || (array == null))
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
            if ((length <= 0) || (array == null))
            {
                return array;
            }

            fixed (byte* pSrc = &array[offset])
            {
                for (var i = 0; i < length; i++)
                {
                    var pDst = pSrc + i;
                    *pDst = value;
                }
            }

            return array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe byte[] FillMemoryCopy(this byte[] array, int offset, int length, byte value)
        {
            // few cost
            if ((length <= 0) || (array == null))
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

        public static unsafe bool TryParseDecimal(byte[] bytes, int index, int length, out decimal value)
        {
            value = 0m;

            var mantissa = default(DecimalMantissaInt3);
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

                var negative = *(pBytes + i) == '-';
                i += negative ? 1 : 0;

                var count = 0;
                var dotPos = -1;
                while (i < length)
                {
                    var num = *(pBytes + i) - 0x30;
                    if ((num >= 0) && (num < 10))
                    {
                        if (mantissa.Multiply10AndAdd(num))
                        {
                            count++;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if ((*(pBytes + i) == '.') && (dotPos == -1))
                    {
                        dotPos = count;
                    }
                    else if (*(pBytes + i) != ',')
                    {
                        while ((i < length) && (*(pBytes + i) == ' '))
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
                    mantissa.Lo,
                    mantissa.Mid,
                    mantissa.Hi,
                    negative,
                    (byte)(dotPos == -1 ? 0 : (count - dotPos)));
                return true;
            }
        }

        private struct DecimalMantissaInt3
        {
            public int Lo { get; private set; }

            public int Mid { get; private set; }

            public int Hi { get; private set; }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Multiply10AndAdd(int value)
            {
                var lo = Lo;
                var mid = Mid;
                var hi = Hi;

                var lo8 = Lo;
                var mid8 = Mid;
                var hi8 = Hi;

                var carryLo2 = ShiftWithCarry(ref lo, 1);
                var carryMid2 = ShiftWithCarry(ref mid, 1);
                if (ShiftWithCarry(ref hi, 1) > 0)
                {
                    return false;
                }

                mid = mid | carryLo2;
                hi = hi | carryMid2;

                var carryLo8 = ShiftWithCarry(ref lo8, 3);
                var carryMid8 = ShiftWithCarry(ref mid8, 3);
                if (ShiftWithCarry(ref hi8, 3) > 0)
                {
                    return false;
                }

                mid8 = mid8 | carryLo8;
                hi8 = hi8 | carryMid8;

                var carryLo = AddWithCarry(ref lo, lo8, 0);
                var carryMid = AddWithCarry(ref mid, mid8, carryLo);
                if (AddWithCarry(ref hi, hi8, carryMid) > 0)
                {
                    return false;
                }

                var carry = AddWithCarry(ref lo, value);
                carry = AddWithCarry(ref mid, carry);
                if (AddWithCarry(ref hi, carry) > 0)
                {
                    return false;
                }

                Lo = lo;
                Mid = mid;
                Hi = hi;

                return true;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static int AddWithCarry(ref int value, int add)
            {
                var l = (ulong)(uint)value;
                l += (ulong)add;
                value = (int)(l & 0xFFFFFFFF);
                return (int)((l >> 32) & 0xFFFFFFFF);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static int AddWithCarry(ref int value, int add1, int add2)
            {
                var l = (ulong)(uint)value;
                l += (uint)add1;
                l += (uint)add2;
                value = (int)(l & 0xFFFFFFFF);
                return (int)((l >> 32) & 0xFFFFFFFF);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static int ShiftWithCarry(ref int value, int bit)
            {
                var l = (ulong)(uint)value;
                l = l << bit;
                value = (int)(l & 0xFFFFFFFF);
                return (int)((l >> 32) & 0xFFFFFFFF);
            }
        }

        public static unsafe bool TryParseDecimalX(byte[] bytes, int index, int length, out decimal value)
        {
            value = 0m;

            var mantissa = default(DecimalMantissa);
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

                var negative = *(pBytes + i) == '-';
                i += negative ? 1 : 0;

                var count = 0;
                var dotPos = -1;
                while (i < length)
                {
                    var num = *(pBytes + i) - 0x30;
                    if ((num >= 0) && (num < 10))
                    {
                        mantissa.Multiply10AndAdd((ulong)num);
                        count++;
                    }
                    else if ((*(pBytes + i) == '.') && (dotPos == -1))
                    {
                        dotPos = count;
                    }
                    else if (*(pBytes + i) != ',')
                    {
                        while ((i < length) && (*(pBytes + i) == ' '))
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
                    mantissa.Lo,
                    mantissa.Mid,
                    mantissa.Hi,
                    negative,
                    (byte)(dotPos == -1 ? 0 : (count - dotPos)));
                return true;
            }
        }

        public struct DecimalMantissa
        {
            private const ulong Threshold = 1_000_000_000_000_000_000UL;

            private ulong lomid;

            private uint hi;

            public int Lo => (int)(lomid & 0xFFFFFFFF);

            public int Mid => (int)((lomid >> 32) & 0xFFFFFFFF);

            public int Hi => (int)hi;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Multiply10AndAdd(ulong value)
            {
                if ((hi == 0) && (lomid <= Threshold))
                {
                    lomid = (lomid * 10) + value;
                    return;
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
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private bool IsOverflow(ulong value1, ulong value2)
            {
                return UInt64.MaxValue - value1 <= value2;
            }
        }

        // TypeB

        public static unsafe bool TryParseDecimal2(byte[] bytes, int index, int length, out decimal value)
        {
            value = 0m;

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

                var negative = *(pBytes + i) == '-';
                i += negative ? 1 : 0;

                var midlo = 0UL;
                var count = 0;
                var dotPos = -1;
                while (i < length)
                {
                    var num = *(pBytes + i) - 0x30;
                    if ((num >= 0) && (num < 10))
                    {
                        midlo = (midlo * 10) + (ulong)num;
                        count++;
                    }
                    else if ((*(pBytes + i) == '.') && (dotPos == -1))
                    {
                        dotPos = count;
                    }
                    else if (*(pBytes + i) != ',')
                    {
                        while ((i < length) && (*(pBytes + i) == ' '))
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
                    (byte)(dotPos == -1 ? 0 : (count - dotPos)));
                return true;
            }
        }

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

        private const int NegativeBitFlag = unchecked((int)0x80000000);

        public static unsafe void FormatDecimal2(
            byte[] bytes,
            int offset,
            int length,
            decimal value,
            byte scale,
            Padding padding,
            bool withZero,
            int groupingSize)
        {
            var bits = Decimal.GetBits(value);
            var negative = (bits[3] & NegativeBitFlag) != 0;
            var decimalScale = (bits[3] >> 16) & 0x7F;
            var decimalNum = ((ulong)(bits[1] & 0x00000000FFFFFFFF) << 32) + (ulong)(bits[0] & 0x00000000FFFFFFFF);

            fixed (byte* pBytes = &bytes[offset])
            {
                if ((padding == Padding.Left) || withZero)
                {
                    var i = length - 1;
                    var dotPos = scale > 0 ? length - scale - 1 : Int32.MinValue;
                    var groupingCount = 0;

                    if (scale > decimalScale)
                    {
                        for (var j = 0; j < scale - decimalScale; j++)
                        {
                            *(pBytes + i--) = 0x30;
                        }
                    }
                    else if (scale < decimalScale)
                    {
                        //for (var j = 0; j < decimalScale - scale; j++)
                        //{
                        //    decimalNum /= 10;
                        //}
                        FixDecimalScale(ref decimalNum, decimalScale - scale);
                    }

                    if (decimalNum > UInt32.MaxValue)
                    {
                        while (i >= 0)
                        {
                            if (groupingCount == groupingSize)
                            {
                                *(pBytes + i--) = 0x2C;

                                groupingCount = 0;
                            }

                            *(pBytes + i--) = (byte)(0x30 + (decimalNum % 10));

                            decimalNum /= 10;

                            if (i > dotPos)
                            {
                                groupingCount++;
                            }

                            if (i == dotPos)
                            {
                                *(pBytes + i--) = 0x2E;

                                if (decimalNum == 0)
                                {
                                    *(pBytes + i--) = 0x30;
                                }
                            }

                            // MEMO tune
                            if (decimalNum <= UInt32.MaxValue)
                            {
                                break;
                            }

                            if (decimalNum == 0)
                            {
                                break;
                            }
                        }
                    }

                    // MEMO tune
                    if (decimalNum > 0)
                    {
                        var decimalNum2 = (uint)decimalNum;
                        while (i >= 0)
                        {
                            if (groupingCount == groupingSize)
                            {
                                *(pBytes + i--) = 0x2C;

                                groupingCount = 0;
                            }

                            *(pBytes + i--) = (byte)(0x30 + (decimalNum2 % 10));

                            decimalNum2 /= 10;

                            if (i > dotPos)
                            {
                                groupingCount++;
                            }

                            if (i == dotPos)
                            {
                                *(pBytes + i--) = 0x2E;

                                if (decimalNum2 == 0)
                                {
                                    *(pBytes + i--) = 0x30;
                                }
                            }

                            if (decimalNum2 == 0)
                            {
                                break;
                            }
                        }
                    }

                    if (withZero)
                    {
                        var end = negative ? 1 : 0;
                        while (i >= end)
                        {
                            if (groupingCount == groupingSize)
                            {
                                *(pBytes + i--) = 0x2C;

                                groupingCount = 0;
                            }

                            if (i >= end)
                            {
                                *(pBytes + i--) = 0x30;
                            }

                            groupingCount++;
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
                            *(pBytes + i--) = 0x2D;
                        }

                        while (i >= 0)
                        {
                            *(pBytes + i--) = 0x20;
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
                            *(pBytes + i++) = 0x30;
                        }
                    }
                    else if (scale < decimalScale)
                    {
                        //for (var j = 0; j < decimalScale - scale; j++)
                        //{
                        //    decimalNum /= 10;
                        //}
                        FixDecimalScale(ref decimalNum, decimalScale - scale);
                    }

                    if (decimalNum > UInt32.MaxValue)
                    {
                        while (i < length)
                        {
                            if (groupingCount == groupingSize)
                            {
                                *(pBytes + i++) = 0x2C;

                                groupingCount = 0;
                            }

                            *(pBytes + i++) = (byte)(0x30 + (decimalNum % 10));

                            decimalNum /= 10;

                            if (i > dotPos)
                            {
                                groupingCount++;
                            }

                            if (i == dotPos)
                            {
                                *(pBytes + i++) = 0x2E;

                                if (decimalNum == 0)
                                {
                                    *(pBytes + i++) = 0x30;
                                }
                            }

                            // MEMO tune
                            if (decimalNum <= UInt32.MaxValue)
                            {
                                break;
                            }

                            if (decimalNum == 0)
                            {
                                break;
                            }
                        }
                    }

                    // MEMO tune
                    if (decimalNum > 0)
                    {
                        var decimalNum2 = (uint)decimalNum;
                        while (i < length)
                        {
                            if (groupingCount == groupingSize)
                            {
                                *(pBytes + i++) = 0x2C;

                                groupingCount = 0;
                            }

                            *(pBytes + i++) = (byte)(0x30 + (decimalNum2 % 10));

                            decimalNum2 /= 10;

                            if (i > dotPos)
                            {
                                groupingCount++;
                            }

                            if (i == dotPos)
                            {
                                *(pBytes + i++) = 0x2E;

                                if (decimalNum2 == 0)
                                {
                                    *(pBytes + i++) = 0x30;
                                }
                            }

                            if (decimalNum2 == 0)
                            {
                                break;
                            }
                        }
                    }

                    if (negative && (i < length))
                    {
                        *(pBytes + i++) = 0x2D;
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
                        *(pBytes + i++) = 0x20;
                    }
                }
            }
        }

        // private
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FixDecimalScale(ref ulong value, int diff)
        {
            if ((value <= UInt32.MaxValue) && (diff <= 9))
            {
                var pow = 10U;
                for (var i = 0; i < diff - 1; i++)
                {
                    pow *= 10;
                }

                value = (uint)value / pow;
            }
            else if (diff <= 19)
            {
                var pow = 10UL;
                for (var i = 0; i < diff - 1; i++)
                {
                    pow *= 10;
                }

                value = value / pow;
            }
            else
            {
                for (var i = 0; i < diff; i++)
                {
                    value /= 10;
                }
            }
        }

        public static unsafe void FormatDecimal3(byte[] bytes, int offset, int length, decimal value, byte scale, Padding padding, bool withZero)
        {
            var bits = Decimal.GetBits(value);
            var negative = (bits[3] & NegativeBitFlag) != 0;
            var decimalScale = (bits[3] >> 16) & 0x7F;
            var decimalNum = (uint)bits[0];

            fixed (byte* pBytes = &bytes[offset])
            {
                if ((padding == Padding.Left) || withZero)
                {
                    var i = length - 1;
                    var dotPos = scale > 0 ? length - scale - 1 : Int32.MinValue;

                    if (scale > decimalScale)
                    {
                        for (var j = 0; j < scale - decimalScale; j++)
                        {
                            *(pBytes + i) = 0x30;
                            i--;
                        }
                    }
                    else if (scale < decimalScale)
                    {
                        for (var j = 0; j < decimalScale - scale; j++)
                        {
                            decimalNum /= 10;
                        }
                    }

                    while (i >= 0)
                    {
                        *(pBytes + i) = (byte)(0x30 + (decimalNum % 10));
                        i--;

                        decimalNum /= 10;

                        if (i == dotPos)
                        {
                            *(pBytes + i) = 0x2E;
                            i--;

                            if (decimalNum == 0)
                            {
                                *(pBytes + i) = 0x30;
                                i--;
                            }
                        }

                        if (decimalNum == 0)
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
                    var dotPos = scale > 0 ? scale : Int32.MinValue;

                    if (scale > decimalScale)
                    {
                        for (var j = 0; j < scale - decimalScale; j++)
                        {
                            *(pBytes + i) = 0x30;
                            i++;
                        }
                    }
                    else if (scale < decimalScale)
                    {
                        for (var j = 0; j < decimalScale - scale; j++)
                        {
                            decimalNum /= 10;
                        }
                    }

                    while (i < length)
                    {
                        *(pBytes + i) = (byte)(0x30 + (decimalNum % 10));
                        i++;

                        decimalNum /= 10;

                        if (i == dotPos)
                        {
                            *(pBytes + i) = 0x2E;
                            i--;

                            if (decimalNum == 0)
                            {
                                *(pBytes + i) = 0x30;
                                i--;
                            }
                        }

                        if (decimalNum == 0)
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
        // DateTime
        //--------------------------------------------------------------------------------

        private const char FormatYear = 'y';
        private const char FormatMonth = 'M';
        private const char FormatDay = 'd';
        private const char FormatHour = 'H';
        private const char FormatMinute = 'm';
        private const char FormatSecond = 's';
        private const char FormatMilisecond = 'f';

        private const long TicksPerMillisecond = 10000;
        private const long TicksPerSecond = TicksPerMillisecond * 1000;
        private const long TicksPerMinute = TicksPerSecond * 60;
        private const long TicksPerHour = TicksPerMinute * 60;
        private const long TicksPerDay = TicksPerHour * 24;

        private static readonly int[] DaysToMonth365 =
        {
            0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334, 365
        };

        private static readonly int[] DaysToMonth366 =
        {
            0, 31, 60, 91, 121, 152, 182, 213, 244, 274, 305, 335, 366
        };

        public static unsafe bool TryParseDateTime(byte[] bytes, int index, string format, DateTimeKind kind, out DateTime value)
        {
            fixed (byte* pBytes = &bytes[index])
            fixed (char* pFormat = format)
            {
                var year = 1;
                var month = 1;
                var day = 1;
                var hour = 0;
                var minute = 0;
                var second = 0;
                var milisecond = 0;

                var length = format.Length;
                var i = 0;
                while (i < length)
                {
                    var c = *(pFormat + i);
                    if (c == FormatYear)
                    {
                        // Year
                        var prev = i;
                        year = ParseDateTimePart(pBytes, pFormat, c, length, ref i);
                        if (i - prev == 2)
                        {
                            year += 2000;
                        }

                        if ((year > 9999) || (year < 1))
                        {
                            value = default;
                            return false;
                        }
                    }
                    else if (c == FormatMonth)
                    {
                        // Month
                        month = ParseDateTimePart(pBytes, pFormat, c, length, ref i);
                        if ((month < 1) || (month > 12))
                        {
                            value = default;
                            return false;
                        }
                    }
                    else if (c == FormatDay)
                    {
                        // Day
                        day = ParseDateTimePart(pBytes, pFormat, c, length, ref i);
                        if (day < 1)
                        {
                            value = default;
                            return false;
                        }
                    }
                    else if (c == FormatHour)
                    {
                        // Hour
                        hour = ParseDateTimePart(pBytes, pFormat, c, length, ref i);
                        if ((hour < 0) || (hour > 23))
                        {
                            value = default;
                            return false;
                        }
                    }
                    else if (c == FormatMinute)
                    {
                        // Minute
                        minute = ParseDateTimePart(pBytes, pFormat, c, length, ref i);
                        if ((minute < 0) || (minute > 59))
                        {
                            value = default;
                            return false;
                        }
                    }
                    else if (c == FormatSecond)
                    {
                        // Second
                        second = ParseDateTimePart(pBytes, pFormat, c, length, ref i);
                        if ((second < 0) || (second > 59))
                        {
                            value = default;
                            return false;
                        }
                    }
                    else if (c == FormatMilisecond)
                    {
                        // Milisecond
                        milisecond = ParseDateTimeMilisecond(pBytes, pFormat, length, ref i);
                        if (milisecond < 0)
                        {
                            value = default;
                            return false;
                        }
                    }
                    else if (c == *(pBytes + i))
                    {
                        i++;
                    }
                    else
                    {
                        value = default;
                        return false;
                    }
                }

                if (year == 0)
                {
                    year = DateTime.Now.Year;
                }

                var days = DateTime.IsLeapYear(year) ? DaysToMonth366 : DaysToMonth365;

                if (day > days[month] - days[month - 1])
                {
                    value = default;
                    return false;
                }

                var y = year - 1;
                var ticks = (((y * 365) + (y / 4) - (y / 100) + (y / 400)) + days[month - 1] + (day - 1)) * TicksPerDay;
                ticks += (((long)hour * 3600) + ((long)minute * 60) + second) * TimeSpan.TicksPerSecond;
                if (milisecond > 0)
                {
                    ticks += milisecond * TicksPerMillisecond;
                }

                value = new DateTime(ticks, kind);
                return true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe int ParseDateTimePart(byte* pBytes, char* pFormat, char c, int limit, ref int i)
        {
            var value = 0;

            do
            {
                var num = *(pBytes + i++) - Num0;
                if ((num >= 0) && (num < 10))
                {
                    value = (value * 10) + num;
                }
                else if (num != -16)
                {
                    return -1;
                }
            }
            while ((i < limit) && (*(pFormat + i) == c));

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe int ParseDateTimeMilisecond(byte* pBytes, char* pFormat, int limit, ref int i)
        {
            var value = 0;
            var index = 0;

            do
            {
                var num = *(pBytes + i++) - Num0;
                if ((num >= 0) && (num < 10))
                {
                    if (index == 0)
                    {
                        value = num * 100;
                    }
                    else if (index == 1)
                    {
                        value += num * 10;
                    }
                    else if (index == 2)
                    {
                        value += num;
                    }
                }
                else if (num != -16)
                {
                    return -1;
                }

                index++;
            }
            while ((i < limit) && (*(pFormat + i) == FormatMilisecond));

            return value;
        }

        public static unsafe void FormatDateTime(byte[] bytes, int index, string format, DateTime dateTime)
        {
            fixed (byte* pBytes = &bytes[index])
            fixed (char* pFormat = format)
            {
                var length = format.Length;
                var i = 0;
                while (i < length)
                {
                    var c = *(pFormat + i);
                    if (c == FormatYear)
                    {
                        FormatDateTimePart(pBytes, pFormat, c, dateTime.Year, length, ref i);
                    }
                    else if (c == FormatMonth)
                    {
                        FormatDateTimePart(pBytes, pFormat, c, dateTime.Month, length, ref i);
                    }
                    else if (c == FormatDay)
                    {
                        FormatDateTimePart(pBytes, pFormat, c, dateTime.Day, length, ref i);
                    }
                    else if (c == FormatHour)
                    {
                        FormatDateTimePart(pBytes, pFormat, c, dateTime.Hour, length, ref i);
                    }
                    else if (c == FormatMinute)
                    {
                        FormatDateTimePart(pBytes, pFormat, c, dateTime.Minute, length, ref i);
                    }
                    else if (c == FormatSecond)
                    {
                        FormatDateTimePart(pBytes, pFormat, c, dateTime.Second, length, ref i);
                    }
                    else if (c == FormatMilisecond)
                    {
                        FormatDateTimeMilisecond(pBytes, pFormat, dateTime.Millisecond, length, ref i);
                    }
                    else
                    {
                        *(pBytes + i++) = (byte)c;
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void FormatDateTimePart(byte* pBytes, char* pFormat, char c, int value, int limit, ref int i)
        {
            var length = 1;
            for (var j = i + 1; (j < limit) && (*(pFormat + j) == c); j++)
            {
                length++;
            }

            var offset = i + length - 1;
            for (var j = 0; j < length - 1; j++)
            {
                DivMod10(value, out var div, out var mod);
                *(pBytes + offset--) = (byte)(Num0 + mod);
                value = div;
            }

            *(pBytes + offset) = (byte)(Num0 + value);

            i += length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void FormatDateTimeMilisecond(byte* pBytes, char* pFormat, int value, int limit, ref int i)
        {
            var length = 1;
            for (var j = i + 1; (j < limit) && (*(pFormat + j) == FormatMilisecond); j++)
            {
                length++;
            }

            if (length > 3)
            {
                throw new FormatException("Invalid format.");
            }

            *(pBytes + i++) = (byte)(Num0 + (value / 100));
            value = value % 100;

            if (length > 1)
            {
                DivMod10(value, out var div, out var mod);
                *(pBytes + i++) = (byte)(Num0 + div);
                value = mod;
            }

            if (length > 2)
            {
                *(pBytes + i++) = (byte)(Num0 + value);
            }
        }

        private class Entry
        {
            public int Div { get; }

            public int Mod { get; }

            public Entry(int div, int mod)
            {
                Div = div;
                Mod = mod;
            }
        }

        private const int ModEntryMax = 100;

        private static readonly Entry[] DivMods10Entry = new Entry[ModEntryMax];

        static ByteHelper()
        {
            for (var i = 0; i < ModEntryMax; i++)
            {
                DivMods10Entry[i] = new Entry(Div10Signed(i), i % 10);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DivMod10(int value, out int div, out int mod)
        {
            if (value < ModEntryMax)
            {
                var entry = DivMods10Entry[value];
                div = entry.Div;
                mod = entry.Mod;
            }
            else
            {
                div = Div10Signed(value);
                mod = value % 10;
            }
        }

        //--------------------------------------------------------------------------------
        // Rverse
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
