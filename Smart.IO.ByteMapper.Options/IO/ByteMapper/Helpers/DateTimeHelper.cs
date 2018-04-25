namespace Smart.IO.ByteMapper.Helpers
{
    using System;
    using System.Runtime.CompilerServices;

    internal static class DateTimeHelper
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

        private static readonly int[] Miliseconds = { 100, 10, 1 };

        //--------------------------------------------------------------------------------
        // DateTime
        //--------------------------------------------------------------------------------

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
                var num = *(pBytes + i) - Num0;
                if ((num >= 0) && (num < 10))
                {
                    value = (value * 10) + num;
                }
                else if (num != -16)
                {
                    return -1;
                }

                i++;
            }
            while ((i < limit) && (*(pFormat + i) == c));

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe int ParseDateTimeMilisecond(byte* pBytes, char* pFormat, int limit, ref int i)
        {
            var index = 0;
            var value = 0;
            do
            {
                var num = *(pBytes + i) - Num0;
                if ((num >= 0) && (num < 10) && (index < Miliseconds.Length))
                {
                    value += Miliseconds[index] * num;
                }
                else if (num != -16)
                {
                    return -1;
                }

                index++;
                i++;
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

                        // TODO optimize
                        for (var j = i + append; j >= i; j--)
                        {
                            *(pBytes + j) = (byte)(Num0 + (value % 10));
                            value /= 10;
                        }

                        i += append;
                    }
                    else
                    {
                        // TODO optimize & max3
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
    }
}
