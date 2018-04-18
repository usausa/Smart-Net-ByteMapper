namespace Smart.IO.ByteMapper.Helpers
{
    using System;
    using System.Runtime.CompilerServices;

    public static class BytesHelper
    {
        private const byte Space = (byte)' ';
        private const byte Minus = (byte)'-';
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
        public static bool TryParseInt16(byte[] bytes, int index, int length, out short value)
        {
            var ret = TryParseInt64(bytes, index, length, out var longValue);
            value = (short)longValue;
            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryParseInt32(byte[] bytes, int index, int length, out int value)
        {
            var ret = TryParseInt64(bytes, index, length, out var longValue);
            value = (int)longValue;
            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool TryParseInt64(byte[] bytes, int index, int length, out long value)
        {
            value = 0L;

            fixed (byte* pBytes = &bytes[index])
            {
                var i = 0;
                while ((i < length) && (*(pBytes + i) == Space))
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
                        while ((i < length) && (*(pBytes + i) == Space))
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
        public static void FormatInt16(byte[] bytes, int index, int length, short value, Padding padding, bool zeroPadding)
        {
            FormatInt64(bytes, index, length, value, padding, zeroPadding);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FormatInt32(byte[] bytes, int index, int length, int value, Padding padding, bool zeroPadding)
        {
            FormatInt64(bytes, index, length, value, padding, zeroPadding);
        }

        public static unsafe void FormatInt64(byte[] bytes, int index, int length, long value, Padding padding, bool zeroPadding)
        {
            fixed (byte* pBytes = &bytes[index])
            {
                if ((padding == Padding.Left) || zeroPadding)
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

                    if (zeroPadding)
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
                            *(pBytes + i--) = Space;
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
                        *(pBytes + i++) = Space;
                    }
                }
            }
        }

        //--------------------------------------------------------------------------------
        // Decimal
        //--------------------------------------------------------------------------------

        // TODO

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
