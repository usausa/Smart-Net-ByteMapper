namespace ByteHelperTest;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

internal static partial class ByteHelper4
{
    private const byte Num0 = (byte)'0';

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
            var millisecond = 0;

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
                else if (c == FormatMillisecond)
                {
                    // Millisecond
                    millisecond = ParseDateTimeMillisecond(pBytes, pFormat, length, ref i);
                    if (millisecond < 0)
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
            if (millisecond > 0)
            {
                ticks += millisecond * TicksPerMillisecond;
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
    private static unsafe int ParseDateTimeMillisecond(byte* pBytes, char* pFormat, int limit, ref int i)
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
        while ((i < limit) && (*(pFormat + i) == FormatMillisecond));

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
                else if (c == FormatMillisecond)
                {
                    FormatDateTimeMillisecond(pBytes, pFormat, dateTime.Millisecond, length, ref i);
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
    private static unsafe void FormatDateTimeMillisecond(byte* pBytes, char* pFormat, int value, int limit, ref int i)
    {
        var length = 1;
        for (var j = i + 1; (j < limit) && (*(pFormat + j) == FormatMillisecond); j++)
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

    //--------------------------------------------------------------------------------
    // DateTime3
    //--------------------------------------------------------------------------------

    public sealed class DateTimeEntry
    {
        public char Part { get; }

        public int Length { get; }

        public byte[] Bytes { get; }

        public DateTimeEntry(char part, int length, byte[] bytes)
        {
            Part = part;
            Length = length;
            Bytes = bytes;
        }
    }

    //--------------------------------------------------------------------------------

    private static bool IsDateTimeFormatChar(char c)
    {
        return c == FormatYear || c == FormatMonth || c == FormatDay ||
               c == FormatHour || c == FormatMinute || c == FormatSecond || c == FormatMillisecond;
    }

    private static bool IsDatePartChar(char c)
    {
        return c == FormatYear || c == FormatMonth || c == FormatDay;
    }

    public static DateTimeEntry[] ParseDateTimeFormat(string format, out bool hasDatePart)
    {
        hasDatePart = false;
        var list = new List<DateTimeEntry>();

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

                list.Add(new DateTimeEntry(c, index - start, null));

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

                list.Add(new DateTimeEntry((char)0, index - start, null));
            }
        }

        return list.ToArray();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsValidNumber(int value)
    {
        return (value >= 0) && (value < 10);
    }

    public static unsafe bool TryParseDateTime2(byte[] bytes, int index, DateTimeEntry[] entries, DateTimeKind kind, out DateTime value)
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
        if (!IsValidNumber(value))
        {
            return -1;
        }

        for (var i = 1; i < length; i++)
        {
            var num = *(pBase + i) - Num0;
            if (!IsValidNumber(num))
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
            if (IsValidNumber(num100) && IsValidNumber(num10) && IsValidNumber(num1))
            {
                return (num100 * 100) + (num10 * 10) + num1;
            }
        }
        else if (length == 2)
        {
            var num100 = *pBase - Num0;
            var num10 = *(pBase + 1) - Num0;
            if (IsValidNumber(num100) && IsValidNumber(num10))
            {
                return (num100 * 100) + (num10 * 10);
            }
        }
        else if (length == 1)
        {
            var num = *pBase - Num0;
            if (IsValidNumber(num))
            {
                return num;
            }
        }

        return -1;
    }

    //--------------------------------------------------------------------------------

    public static unsafe void FormatDateTime2(byte[] bytes, int index, bool hasDatePart, DateTimeEntry[] entries, DateTime dateTime)
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

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //private static unsafe void FormatDateTimePart(byte* pBytes, int value, int length)
    //{
    //    var offset = length - 1;
    //    for (var j = 0; j < length - 1; j++)
    //    {
    //        OperationHelper.DivMod10Signed(value, out var div, out var mod);
    //        *(pBytes + offset--) = (byte)(Num0 + mod);
    //        value = div;
    //    }

    //    *(pBytes + offset) = (byte)(Num0 + value);
    //}

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

    //--------------------------------------------------------------------------------
    // DateTime2(slow)
    //--------------------------------------------------------------------------------

#pragma warning disable SA1203 // Constants must appear before fields
    // Number of days in a non-leap year
    private const int DaysPerYear = 365;
                          // Number of days in 4 years
    private const int DaysPer4Years = (DaysPerYear * 4) + 1;       // 1461
    // Number of days in 100 years
    private const int DaysPer100Years = (DaysPer4Years * 25) - 1;  // 36524
    // Number of days in 400 years
    private const int DaysPer400Years = (DaysPer100Years * 4) + 1; // 146097
#pragma warning restore SA1203 // Constants must appear before fields

    public static void GetDatePart(long ticks, out int year, out int month, out int day)
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
}
