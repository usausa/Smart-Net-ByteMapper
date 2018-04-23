namespace Smart.IO.ByteMapper.Helpers
{
    using System;
    using System.Runtime.CompilerServices;

    public static class BytesHelper
    {
        private const byte Minus = (byte)'-';
        private const byte Dot = (byte)'.';
        private const byte Comma = (byte)',';
        private const byte Num0 = (byte)'0';

        private const char FormatYear = 'y';
        private const char FormatMonth = 'M';
        private const char FormatDay = 'd';
        private const char FormatHour = 'H';
        private const char FormatMinute = 'm';
        private const char FormatSecond = 's';
        private const char FormatMilisecond = 'f';

        private const long Int64MinValueDiv10 = Int64.MinValue / 10;
        private const byte Int64MinValueMod10 = (byte)(Num0 + -(Int64.MinValue % 10));

        private const int NegativeBitFlag = unchecked((int)0x80000000);

        //--------------------------------------------------------------------------------
        // Enum
        //--------------------------------------------------------------------------------

        public static Type GetConvertEnumType(Type type)
        {
            var targetType = type.IsNullableType() ? Nullable.GetUnderlyingType(type) : type;
            return (targetType?.IsEnum ?? false) ? targetType : null;
        }

        //--------------------------------------------------------------------------------
        // Fill
        //--------------------------------------------------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Fill(byte[] byets, int index, int length, byte value)
        {
            for (var i = 0; i < length; i++)
            {
                byets[index + i] = value;
            }
        }

        //--------------------------------------------------------------------------------
        // Copy
        //--------------------------------------------------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyPadRight(byte[] bytes, byte[] buffer, int index, int length, byte filler)
        {
            Buffer.BlockCopy(bytes, 0, buffer, index, bytes.Length);
            Fill(buffer, index + bytes.Length, length - bytes.Length, filler);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyPadLeft(byte[] bytes, byte[] buffer, int index, int length, byte filler)
        {
            Buffer.BlockCopy(bytes, 0, buffer, index + length - bytes.Length, bytes.Length);
            Fill(buffer, index, length - bytes.Length, filler);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TrimRange(byte[] buffer, ref int start, ref int size, Padding padding, byte filler)
        {
            if (padding == Padding.Left)
            {
                var end = start + size;
                while ((start < end) && (buffer[start] == filler))
                {
                    start++;
                    size--;
                }
            }
            else
            {
                while ((size > 0) && (buffer[start + size - 1] == filler))
                {
                    size--;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBytes(byte[] bytes, byte[] buffer, int index, int length, Padding padding, byte filler)
        {
            if (bytes.Length >= length)
            {
                Buffer.BlockCopy(bytes, 0, buffer, index, length);
            }
            else if (padding == Padding.Right)
            {
                CopyPadRight(bytes, buffer, index, length, filler);
            }
            else
            {
                CopyPadLeft(bytes, buffer, index, length, filler);
            }
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
        public static unsafe string GetAsciiString(byte[] bytes, int index, int length)
        {
            var str = new string('\0', length);

            fixed (byte* pSrc = &bytes[index])
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
        // Number
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
                        midlo = (midlo * 10) + (ulong)num;
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
        // DateTime
        //--------------------------------------------------------------------------------

        public static unsafe bool TryParseDateTime(byte[] bytes, int index, string format, out DateTime value)
        {
            fixed (byte* pBytes = &bytes[index])
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
                    var num = *(pBytes + i) - Num0;
                    if ((num >= 0) && (num < 10))
                    {
                        var c = *(pFormat + i);
                        if (c == FormatYear)
                        {
                            year = (year * 10) + num;
                        }
                        else if (c == FormatMonth)
                        {
                            month = (month * 10) + num;
                        }
                        else if (c == FormatDay)
                        {
                            day = (day * 10) + num;
                        }
                        else if (c == FormatHour)
                        {
                            hour = (hour * 10) + num;
                        }
                        else if (c == FormatMinute)
                        {
                            minute = (minute * 10) + num;
                        }
                        else if (c == FormatSecond)
                        {
                            second = (second * 10) + num;
                        }
                        else if (c == FormatMilisecond)
                        {
                            milisecond = milisecond + (num * msPow);
                            msPow /= 10;
                        }
                        else
                        {
                            value = default;
                            return false;
                        }
                    }
                    else if (*(pFormat + i) == FormatMilisecond)
                    {
                        msPow /= 10;
                    }
                }

                try
                {
                    value = milisecond == 0
                        ? new DateTime(year, month, day, hour, minute, second)
                        : new DateTime(year, month, day, hour, minute, second, milisecond);
                    return true;
                }
                catch (ArgumentOutOfRangeException)
                {
                    value = default;
                    return false;
                }
            }
        }

        public static unsafe void FormatDateTime(byte[] bytes, int index, string format, DateTime dateTime)
        {
            fixed (byte* pBytes = &bytes[index])
            fixed (char* pFormat = format)
            {
                var length = format.Length;
                for (var i = 0; i < length; i++)
                {
                    var pow = 0;
                    int value;

                    var c = *(pFormat + i);
                    if (c == FormatYear)
                    {
                        value = dateTime.Year;
                    }
                    else if (c == FormatMonth)
                    {
                        value = dateTime.Month;
                    }
                    else if (c == FormatDay)
                    {
                        value = dateTime.Day;
                    }
                    else if (c == FormatHour)
                    {
                        value = dateTime.Hour;
                    }
                    else if (c == FormatMinute)
                    {
                        value = dateTime.Minute;
                    }
                    else if (c == FormatSecond)
                    {
                        value = dateTime.Second;
                    }
                    else if (c == FormatMilisecond)
                    {
                        value = dateTime.Millisecond;
                        pow = 100;
                    }
                    else
                    {
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
                            *(pBytes + j) = (byte)(Num0 + (value % 10));
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

                            *(pBytes + i) = (byte)(Num0 + div);

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
