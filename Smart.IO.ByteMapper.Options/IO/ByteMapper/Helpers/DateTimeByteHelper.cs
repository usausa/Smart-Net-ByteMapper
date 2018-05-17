namespace Smart.IO.ByteMapper.Helpers
{
    using System;
    using System.Runtime.CompilerServices;

    internal static class DateTimeByteHelper
    {
        private const byte Num0 = (byte)'0';

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

        //--------------------------------------------------------------------------------
        // DateTime
        //--------------------------------------------------------------------------------

        public static unsafe bool TryParseDateTime(byte[] bytes, int index, string format, DateTimeKind kind, out DateTime value)
        {
            fixed (byte* pBytes = &bytes[index])
            fixed (char* pFormat = format)
            {
                var year = 0;
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
                OperationHelper.DivMod10Signed(value, out var div, out var mod);
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
                OperationHelper.DivMod10Signed(value, out var div, out var mod);
                *(pBytes + i++) = (byte)(Num0 + div);
                value = mod;
            }

            if (length > 2)
            {
                *(pBytes + i++) = (byte)(Num0 + value);
            }
        }
    }
}
