namespace Smart.IO.ByteMapper.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    internal static partial class DateTimeByteHelper
    {
        private const byte Num0 = (byte)'0';
        private const byte Space = (byte)' ';
        private const int SpaceDiff = Space - Num0;

        private const char FormatYear = 'y';
        private const char FormatMonth = 'M';
        private const char FormatDay = 'd';
        private const char FormatHour = 'H';
        private const char FormatMinute = 'm';
        private const char FormatSecond = 's';
        private const char FormatMillisecond = 'f';

        private const long TicksPerMillisecond = 10000;
        private const long TicksPerSecond = TicksPerMillisecond * 1000;
        private const long TicksPerMinute = TicksPerSecond * 60;
        private const long TicksPerHour = TicksPerMinute * 60;
        private const long TicksPerDay = TicksPerHour * 24;

        private const int DaysPerYear = 365;
        private const int DaysPer4Years = (DaysPerYear * 4) + 1;
        private const int DaysPer100Years = (DaysPer4Years * 25) - 1;
        private const int DaysPer400Years = (DaysPer100Years * 4) + 1;

        private static readonly int[] DaysToMonth365 =
        {
            0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334, 365
        };

        private static readonly int[] DaysToMonth366 =
        {
            0, 31, 60, 91, 121, 152, 182, 213, 244, 274, 305, 335, 366
        };

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void GetDatePart(long ticks, out int year, out int month, out int day)
        {
            var n = (int)(ticks / TicksPerDay);
            var y400 = n / DaysPer400Years;

            n -= y400 * DaysPer400Years;

            var y100 = n / DaysPer100Years;
            if (y100 == 4)
            {
                y100 = 3;
            }

            n -= y100 * DaysPer100Years;

            var y4 = n / DaysPer4Years;

            n -= y4 * DaysPer4Years;

            var y1 = n / DaysPerYear;

            if (y1 == 4)
            {
                y1 = 3;
            }

            year = (y400 * 400) + (y100 * 100) + (y4 * 4) + y1 + 1;

            n -= y1 * DaysPerYear;

            var leapYear = y1 == 3 && (y4 != 24 || y100 == 3);
            var days = leapYear ? DaysToMonth366 : DaysToMonth365;
            var m = (n >> 5) + 1;
            while (n >= days[m])
            {
                m++;
            }

            month = m;

            day = n - days[m - 1] + 1;
        }

        private static bool IsDateTimeFormatChar(char c)
        {
            return c == FormatYear || c == FormatMonth || c == FormatDay ||
                   c == FormatHour || c == FormatMinute || c == FormatSecond || c == FormatMillisecond;
        }

        private static bool IsDatePartChar(char c)
        {
            return c == FormatYear || c == FormatMonth || c == FormatDay;
        }

        public static DateTimeFormatEntry[] ParseDateTimeFormat(string format, out bool hasDatePart)
        {
            hasDatePart = false;
            var list = new List<DateTimeFormatEntry>();

            var index = 0;
            while (index < format.Length)
            {
                var start = index;
                var c = format[index++];
                if (IsDateTimeFormatChar(c))
                {
                    while (index < format.Length && format[index] == c)
                    {
                        index++;
                    }

                    var length = index - start;
                    if ((c == FormatMillisecond) && (length > 3))
                    {
                        throw new FormatException($"Invalid format. format=[{format}]");
                    }

                    list.Add(new DateTimeFormatEntry(c, length, null));

                    if (IsDatePartChar(c))
                    {
                        hasDatePart = true;
                    }
                }
                else
                {
                    while (index < format.Length && !IsDateTimeFormatChar(format[index]))
                    {
                        index++;
                    }

                    var length = index - start;

                    var bytes = new byte[length];
                    for (var i = 0; i < length; i++)
                    {
                        bytes[i] = (byte)format[start + i];
                    }

                    list.Add(new DateTimeFormatEntry((char)0, length, bytes));
                }
            }

            return list.ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsValidNumberAndFix(ref int value)
        {
            if ((value >= 0) && (value < 10))
            {
                return true;
            }

            if (value == SpaceDiff)
            {
                value = 0;
                return true;
            }

            return false;
        }

        //--------------------------------------------------------------------------------
        // Parse
        //--------------------------------------------------------------------------------

        public static unsafe bool TryParseDateTime(byte[] bytes, int index, DateTimeFormatEntry[] entries, DateTimeKind kind, out DateTime value)
        {
            fixed (byte* pBytes = &bytes[index])
            {
                var year = 0;
                var month = 1;
                var day = 1;
                var hour = 0;
                var minute = 0;
                var second = 0;
                var millisecond = 0;

                var offset = 0;
                for (var i = 0; i < entries.Length; i++)
                {
                    var entry = entries[i];
                    var part = entry.Part;
                    var length = entry.Length;
                    var pBase = pBytes + offset;

                    if (part == FormatYear)
                    {
                        // Year
                        year = ParseDateTimePart(pBase, length);
                        if (length == 2)
                        {
                            year += 2000;
                        }

                        if ((year > 9999) || (year < 1))
                        {
                            value = default;
                            return false;
                        }
                    }
                    else if (part == FormatMonth)
                    {
                        // Month
                        month = ParseDateTimePart(pBase, length);
                        if ((month < 1) || (month > 12))
                        {
                            value = default;
                            return false;
                        }
                    }
                    else if (part == FormatDay)
                    {
                        // Day
                        day = ParseDateTimePart(pBase, length);
                        if (day < 1)
                        {
                            value = default;
                            return false;
                        }
                    }
                    else if (part == FormatHour)
                    {
                        // Hour
                        hour = ParseDateTimePart(pBase, length);
                        if ((hour < 0) || (hour > 23))
                        {
                            value = default;
                            return false;
                        }
                    }
                    else if (part == FormatMinute)
                    {
                        // Minute
                        minute = ParseDateTimePart(pBase, length);
                        if ((minute < 0) || (minute > 59))
                        {
                            value = default;
                            return false;
                        }
                    }
                    else if (part == FormatSecond)
                    {
                        // Second
                        second = ParseDateTimePart(pBase, length);
                        if ((second < 0) || (second > 59))
                        {
                            value = default;
                            return false;
                        }
                    }
                    else if (part == FormatMillisecond)
                    {
                        // Millisecond
                        millisecond = ParseDateTimeMillisecond(pBase, length);
                        if (millisecond < 0)
                        {
                            value = default;
                            return false;
                        }
                    }

                    offset += length;
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
                if (millisecond > 0)
                {
                    ticks += millisecond * TicksPerMillisecond;
                }

                value = new DateTime(ticks, kind);
                return true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe int ParseDateTimePart(byte* pBase, int length)
        {
            var value = *pBase - Num0;
            if (!IsValidNumberAndFix(ref value))
            {
                return -1;
            }

            for (var i = 1; i < length; i++)
            {
                var num = *(pBase + i) - Num0;
                if (!IsValidNumberAndFix(ref num))
                {
                    return -1;
                }

                value = (value * 10) + num;
            }

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe int ParseDateTimeMillisecond(byte* pBase, int length)
        {
            if (length == 3)
            {
                var num100 = *pBase - Num0;
                var num10 = *(pBase + 1) - Num0;
                var num1 = *(pBase + 2) - Num0;
                if (IsValidNumberAndFix(ref num100) && IsValidNumberAndFix(ref num10) && IsValidNumberAndFix(ref num1))
                {
                    return (num100 * 100) + (num10 * 10) + num1;
                }
            }
            else if (length == 2)
            {
                var num100 = *pBase - Num0;
                var num10 = *(pBase + 1) - Num0;
                if (IsValidNumberAndFix(ref num100) && IsValidNumberAndFix(ref num10))
                {
                    return (num100 * 100) + (num10 * 10);
                }
            }
            else if (length == 1)
            {
                var num100 = *pBase - Num0;
                if (IsValidNumberAndFix(ref num100))
                {
                    return num100 * 100;
                }
            }

            return -1;
        }

        //--------------------------------------------------------------------------------
        // Format
        //--------------------------------------------------------------------------------

        public static unsafe void FormatDateTime(byte[] bytes, int index, bool hasDatePart, DateTimeFormatEntry[] entries, DateTime dateTime)
        {
            var year = 0;
            var month = 0;
            var day = 0;
            if (hasDatePart)
            {
                GetDatePart(dateTime.Ticks, out year, out month, out day);
            }

            fixed (byte* pBytes = &bytes[index])
            {
                var offset = 0;
                for (var i = 0; i < entries.Length; i++)
                {
                    var entry = entries[i];
                    var part = entry.Part;
                    var length = entry.Length;
                    var pBase = pBytes + offset;

                    if (part == FormatYear)
                    {
                        FormatDateTimePart4(pBase, year, length);
                    }
                    else if (part == FormatMonth)
                    {
                        FormatDateTimePart2(pBase, month, length);
                    }
                    else if (part == FormatDay)
                    {
                        FormatDateTimePart2(pBase, day, length);
                    }
                    else if (part == FormatHour)
                    {
                        FormatDateTimePart2(pBase, dateTime.Hour, length);
                    }
                    else if (part == FormatMinute)
                    {
                        FormatDateTimePart2(pBase, dateTime.Minute, length);
                    }
                    else if (part == FormatSecond)
                    {
                        FormatDateTimePart2(pBase, dateTime.Second, length);
                    }
                    else if (part == FormatMillisecond)
                    {
                        FormatDateTimeMillisecond(pBase, dateTime.Millisecond, length);
                    }
                    else
                    {
                        var source = entry.Bytes;
                        for (var j = 0; j < source.Length; j++)
                        {
                            *(pBase + j) = source[j];
                        }
                    }

                    offset += length;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void FormatDateTimePart4(byte* pBytes, int value, int length)
        {
            if (length == 4)
            {
                var hi = Table[value / 100];
                var lo = Table[value % 100];
                *pBytes = hi[0];
                *(pBytes + 1) = hi[1];
                *(pBytes + 2) = lo[0];
                *(pBytes + 3) = lo[1];
            }
            else if (length == 2)
            {
                var lo = Table[value % 100];
                *pBytes = lo[0];
                *(pBytes + 1) = lo[1];
            }
            else if (length > 4)
            {
                var offset = 0;
                for (var i = 0; i < length - 4; i++)
                {
                    *(pBytes + offset++) = Num0;
                }

                var hi = Table[value / 100];
                var lo = Table[value % 100];
                *(pBytes + offset++) = hi[0];
                *(pBytes + offset++) = hi[1];
                *(pBytes + offset++) = lo[0];
                *(pBytes + offset) = lo[1];
            }
            else if (length == 3)
            {
                var hi = Table[value / 100];
                var lo = Table[value % 100];
                *pBytes = hi[1];
                *(pBytes + 1) = lo[0];
                *(pBytes + 2) = lo[1];
            }
            else
            {
                *pBytes = (byte)(Num0 + (value % 10));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void FormatDateTimePart2(byte* pBytes, int value, int length)
        {
            if (length == 2)
            {
                var bytes = Table[value];
                *pBytes = bytes[0];
                *(pBytes + 1) = bytes[1];
            }
            else if (length > 2)
            {
                var offset = 0;
                for (var i = 0; i < length - 2; i++)
                {
                    *(pBytes + offset++) = Num0;
                }

                var bytes = Table[value];
                *(pBytes + offset++) = bytes[0];
                *(pBytes + offset) = bytes[1];
            }
            else
            {
                var bytes = Table[value];
                *pBytes = bytes[1];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void FormatDateTimeMillisecond(byte* pBytes, int value, int length)
        {
            if (length == 3)
            {
                var lo = Table[value % 100];
                *pBytes = (byte)(Num0 + (value / 100));
                *(pBytes + 1) = lo[0];
                *(pBytes + 2) = lo[1];
            }
            else if (length == 2)
            {
                var lo = Table[value % 100];
                *pBytes = (byte)(Num0 + (value / 100));
                *(pBytes + 1) = lo[0];
            }
            else
            {
                *pBytes = (byte)(Num0 + (value / 100));
            }
        }
    }
}
