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

                    var minus = value < 0;
                    if (minus)
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
                        while (i >= (minus ? 1 : 0))
                        {
                            *(pBytes + i) = 0x30;
                            i--;
                        }

                        if (minus && (i >= 0))
                        {
                            *pBytes = 0x2D;
                        }
                    }
                    else
                    {
                        if (minus && (i >= 0))
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

                    var minus = value < 0;
                    if (minus)
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

                    if (minus && (i < length))
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
        // Decimal
        //--------------------------------------------------------------------------------

        public static unsafe bool TryParseDecimal(byte[] bytes, int index, int length, out decimal value)
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
                var decimalPos = 0;
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
                    else if ((*(pBytes + i) == '.') && (decimalPos == 0))
                    {
                        decimalPos = count;
                    }
                    else
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

                value = new decimal(mantissa.Lo, mantissa.Mid, mantissa.Hi, negative, (byte)(count - decimalPos));
                return true;
            }
        }

        private struct DecimalMantissa
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

        //--------------------------------------------------------------------------------
        // DateTime
        //--------------------------------------------------------------------------------

        public static unsafe bool TryParseDateTime(byte[] bytes, int offset, string format, out DateTime value)
        {
            fixed (byte* pBytes = &bytes[offset])
            fixed (char* pFormat = format)
            {
                var year = 0;
                var month = 0;
                var day = 0;
                var hour = 0;
                var minute = 0;
                var second = 0;
                var milisecond = 0;
                var msPow = 100;

                var length = format.Length;
                for (var i = 0; i < length; i++)
                {
                    var num = *(pBytes + i) - 0x30;
                    if ((num >= 0) && (num < 10))
                    {
                        switch (*(pFormat + i))
                        {
                            case 'y':
                                year = (year * 10) + num;
                                break;
                            case 'M':
                                month = (month * 10) + num;
                                break;
                            case 'd':
                                day = (day * 10) + num;
                                break;
                            case 'H':
                                hour = (hour * 10) + num;
                                break;
                            case 'm':
                                minute = (minute * 10) + num;
                                break;
                            case 's':
                                second = (second * 10) + num;
                                break;
                            case 'f':
                                milisecond = milisecond + (num * msPow);
                                msPow /= 10;
                                break;
                            default:
                                value = default;
                                return false;
                        }
                    }
                    else if (*(pFormat + i) == 'f')
                    {
                        msPow /= 10;
                    }
                }

                try
                {
                    value = new DateTime(year, month, day, hour, minute, second, milisecond);
                    return true;
                }
                catch (ArgumentOutOfRangeException)
                {
                    value = default;
                    return false;
                }
            }
        }

        public static unsafe void FormatDateTime(byte[] bytes, int offset, string format, DateTime dateTime)
        {
            fixed (byte* pBytes = &bytes[offset])
            fixed (char* pFormat = format)
            {
                var length = format.Length;
                for (var i = 0; i < length; i++)
                {
                    var c = *(pFormat + i);

                    var pow = 0;
                    int value;
                    switch (c)
                    {
                        case 'y':
                            value = dateTime.Year;
                            break;
                        case 'M':
                            value = dateTime.Month;
                            break;
                        case 'd':
                            value = dateTime.Day;
                            break;
                        case 'H':
                            value = dateTime.Hour;
                            break;
                        case 'm':
                            value = dateTime.Minute;
                            break;
                        case 's':
                            value = dateTime.Second;
                            break;
                        case 'f':
                            value = dateTime.Millisecond;
                            pow = 100;
                            break;
                        default:
                            *(pBytes + i) = (byte)c;
                            continue;
                    }

                    if (pow == 0)
                    {
                        var append = 0;
                        for (var j = i + 1; j < length; j++)
                        {
                            if (*(pFormat + j) == c)
                            {
                                append++;
                            }
                            else
                            {
                                break;
                            }
                        }

                        for (var j = i + append; j >= i; j--)
                        {
                            *(pBytes + j) = (byte)(0x30 + (value % 10));
                            value /= 10;
                        }

                        i += append;
                    }
                    else
                    {
                        while (true)
                        {
                            var div = value / pow;
                            value = value % pow;
                            pow /= 10;

                            *(pBytes + i) = (byte)(0x30 + div);

                            var next = i + 1;
                            if ((next < length) && (*(pFormat + next) == c))
                            {
                                if (pow == 0)
                                {
                                    throw new FormatException("Invalid format.");
                                }

                                i = next;
                                continue;
                            }

                            break;
                        }
                    }
                }
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
