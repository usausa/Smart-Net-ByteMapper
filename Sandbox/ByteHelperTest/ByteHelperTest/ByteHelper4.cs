namespace ByteHelperTest
{
    using System;
    using System.Runtime.CompilerServices;

    internal static class ByteHelper4
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

        //--------------------------------------------------------------------------------
        // DateTime3
        //--------------------------------------------------------------------------------

        public enum DateTimePart
        {
            Year, Month, Day, Hour, Minute, Second, Millisecond, Other
        }

        public sealed class DateTimeEntry
        {
            public DateTimePart Part { get; }

            public int Length { get; }

            public byte[] Bytes { get; }

            public DateTimeEntry(DateTimePart part, int length, byte[] bytes)
            {
                Part = part;
                Length = length;
                Bytes = bytes;
            }
        }

        public static unsafe void FormatDateTime2(byte[] bytes, int index, DateTimeEntry[] entries, DateTime dateTime)
        {
            fixed (byte* pBytes = &bytes[index])
            {
                var offset = 0;
                for (var i = 0; i < entries.Length; i++)
                {
                    var entry = entries[i];
                    var part = entry.Part;
                    var length = entry.Length;
                    var pBase = pBytes + offset;

                    if (part == DateTimePart.Year)
                    {
                        FormatDateTimePart(pBase, dateTime.Year, length);
                    }
                    else if (part == DateTimePart.Month)
                    {
                        FormatDateTimePart(pBase, dateTime.Month, length);
                    }
                    else if (part == DateTimePart.Day)
                    {
                        FormatDateTimePart(pBase, dateTime.Day, length);
                    }
                    else if (part == DateTimePart.Hour)
                    {
                        FormatDateTimePart(pBase, dateTime.Hour, length);
                    }
                    else if (part == DateTimePart.Minute)
                    {
                        FormatDateTimePart(pBase, dateTime.Minute, length);
                    }
                    else if (part == DateTimePart.Second)
                    {
                        FormatDateTimePart(pBase, dateTime.Second, length);
                    }
                    else if (part == DateTimePart.Millisecond)
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
        private static unsafe void FormatDateTimePart(byte* pBytes, int value, int length)
        {
            var offset = length - 1;
            for (var j = 0; j < length - 1; j++)
            {
                OperationHelper.DivMod10Signed(value, out var div, out var mod);
                *(pBytes + offset--) = (byte)(Num0 + mod);
                value = div;
            }

            *(pBytes + offset) = (byte)(Num0 + value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void FormatDateTimeMillisecond(byte* pBytes, int value, int length)
        {
            *pBytes = (byte)(Num0 + (value / 100));
            value = value % 100;

            if (length > 1)
            {
                OperationHelper.DivMod10Signed(value, out var div, out var mod);
                *(pBytes + 1) = (byte)(Num0 + div);
                value = mod;
            }

            if (length > 2)
            {
                *(pBytes + 2) = (byte)(Num0 + value);
            }
        }

        //--------------------------------------------------------------------------------
        // DateTime2(slow)
        //--------------------------------------------------------------------------------

        //// Number of days in a non-leap year
        //private const int DaysPerYear = 365;
        //// Number of days in 4 years
        //private const int DaysPer4Years = DaysPerYear * 4 + 1;       // 1461
        //// Number of days in 100 years
        //private const int DaysPer100Years = DaysPer4Years * 25 - 1;  // 36524
        //// Number of days in 400 years
        //private const int DaysPer400Years = DaysPer100Years * 4 + 1; // 146097

        //public static void GetDatePart(long ticks, out int year, out int month, out int day)
        //{
        //    // n = number of days since 1/1/0001
        //    int n = (int)(ticks / TicksPerDay);
        //    // y400 = number of whole 400-year periods since 1/1/0001
        //    int y400 = n / DaysPer400Years;
        //    // n = day number within 400-year period
        //    n -= y400 * DaysPer400Years;
        //    // y100 = number of whole 100-year periods within 400-year period
        //    int y100 = n / DaysPer100Years;
        //    // Last 100-year period has an extra day, so decrement result if 4
        //    if (y100 == 4) y100 = 3;
        //    // n = day number within 100-year period
        //    n -= y100 * DaysPer100Years;
        //    // y4 = number of whole 4-year periods within 100-year period
        //    int y4 = n / DaysPer4Years;
        //    // n = day number within 4-year period
        //    n -= y4 * DaysPer4Years;
        //    // y1 = number of whole years within 4-year period
        //    int y1 = n / DaysPerYear;
        //    // Last year has an extra day, so decrement result if 4
        //    if (y1 == 4) y1 = 3;
        //    // If year was requested, compute and return it
        //    //if (part == DatePartYear)
        //    //{
        //    //    return y400 * 400 + y100 * 100 + y4 * 4 + y1 + 1;
        //    //}
        //    year = y400 * 400 + y100 * 100 + y4 * 4 + y1 + 1;
        //    // n = day number within year
        //    n -= y1 * DaysPerYear;
        //    // If day-of-year was requested, return it
        //    //if (part == DatePartDayOfYear) return n + 1;
        //    // Leap year calculation looks different from IsLeapYear since y1, y4,
        //    // and y100 are relative to year 1, not year 0
        //    bool leapYear = y1 == 3 && (y4 != 24 || y100 == 3);
        //    int[] days = leapYear ? DaysToMonth366 : DaysToMonth365;
        //    // All months have less than 32 days, so n >> 5 is a good conservative
        //    // estimate for the month
        //    int m = (n >> 5) + 1;
        //    // m = 1-based month number
        //    while (n >= days[m]) m++;
        //    // If month was requested, return it
        //    //if (part == DatePartMonth) return m;
        //    month = m;
        //    // Return 1-based day-of-month
        //    //return n - days[m - 1] + 1;
        //    day = n - days[m - 1] + 1;
        //}

        ////--------------------------------------------------------------------------------

        //public struct DateTimeContext
        //{
        //    public int Year;
        //    public int Month;
        //    public int Day;
        //    public long Ticks;
        //}

        //public unsafe interface IDateTimeBlockConverter
        //{
        //    // bool CanRead { get; }
        //    //int Read(byte* pBytes);

        //    void Write(byte* pBytes, ref DateTimeContext context);
        //}

        ////--------------------------------------------------------------------------------

        //public sealed class Year4BlockConverter : IDateTimeBlockConverter
        //{
        //    private readonly int offset;

        //    public Year4BlockConverter(int offset)
        //    {
        //        this.offset = offset;
        //    }

        //    public unsafe void Write(byte* pBytes, ref DateTimeContext context)
        //    {
        //        var ptr = pBytes + offset;

        //        OperationHelper.DivMod10Signed(context.Year, out var div, out var mod);
        //        *(ptr + 3) = (byte)(Num0 + mod);
        //        OperationHelper.DivMod10Signed(div, out div, out mod);
        //        *(ptr + 2) = (byte)(Num0 + mod);
        //        OperationHelper.DivMod10Signed(div, out div, out mod);
        //        *(ptr + 1) = (byte)(Num0 + mod);
        //        *pBytes = (byte)(Num0 + div);
        //    }
        //}

        //public sealed class Year2BlockConverter : IDateTimeBlockConverter
        //{
        //    private readonly int offset;

        //    public Year2BlockConverter(int offset)
        //    {
        //        this.offset = offset;
        //    }

        //    public unsafe void Write(byte* pBytes, ref DateTimeContext context)
        //    {
        //        var ptr = pBytes + offset;

        //        OperationHelper.DivMod10Signed(context.Year, out var div, out var mod);
        //        *(ptr + 1) = (byte)(Num0 + mod);
        //        OperationHelper.DivMod10Signed(div, out div, out mod);
        //        *ptr = (byte)(Num0 + mod);
        //    }
        //}

        //public sealed class Month2BlockConverter : IDateTimeBlockConverter
        //{
        //    private readonly int offset;

        //    public Month2BlockConverter(int offset)
        //    {
        //        this.offset = offset;
        //    }

        //    public unsafe void Write(byte* pBytes, ref DateTimeContext context)
        //    {
        //        var ptr = pBytes + offset;

        //        OperationHelper.DivMod10Signed(context.Month, out var div, out var mod);
        //        *(ptr + 1) = (byte)(Num0 + mod);
        //        OperationHelper.DivMod10Signed(div, out div, out mod);
        //        *ptr = (byte)(Num0 + mod);
        //    }
        //}

        //public sealed class Day2BlockConverter : IDateTimeBlockConverter
        //{
        //    private readonly int offset;

        //    public Day2BlockConverter(int offset)
        //    {
        //        this.offset = offset;
        //    }

        //    public unsafe void Write(byte* pBytes, ref DateTimeContext context)
        //    {
        //        var ptr = pBytes + offset;

        //        OperationHelper.DivMod10Signed(context.Day, out var div, out var mod);
        //        *(ptr + 1) = (byte)(Num0 + mod);
        //        OperationHelper.DivMod10Signed(div, out div, out mod);
        //        *ptr = (byte)(Num0 + mod);
        //    }
        //}

        //public sealed class Hour2BlockConverter : IDateTimeBlockConverter
        //{
        //    private readonly int offset;

        //    public Hour2BlockConverter(int offset)
        //    {
        //        this.offset = offset;
        //    }

        //    public unsafe void Write(byte* pBytes, ref DateTimeContext context)
        //    {
        //        var ptr = pBytes + offset;

        //        OperationHelper.DivMod10Signed((int)((context.Ticks / TicksPerHour) % 24), out var div, out var mod);
        //        *(ptr + 1) = (byte)(Num0 + mod);
        //        OperationHelper.DivMod10Signed(div, out div, out mod);
        //        *ptr = (byte)(Num0 + mod);
        //    }
        //}

        //public sealed class Minute2BlockConverter : IDateTimeBlockConverter
        //{
        //    private readonly int offset;

        //    public Minute2BlockConverter(int offset)
        //    {
        //        this.offset = offset;
        //    }

        //    public unsafe void Write(byte* pBytes, ref DateTimeContext context)
        //    {
        //        var ptr = pBytes + offset;

        //        OperationHelper.DivMod10Signed((int)((context.Ticks / TicksPerMinute) % 60), out var div, out var mod);
        //        *(ptr + 1) = (byte)(Num0 + mod);
        //        OperationHelper.DivMod10Signed(div, out div, out mod);
        //        *ptr = (byte)(Num0 + mod);
        //    }
        //}

        //public sealed class Second2BlockConverter : IDateTimeBlockConverter
        //{
        //    private readonly int offset;

        //    public Second2BlockConverter(int offset)
        //    {
        //        this.offset = offset;
        //    }

        //    public unsafe void Write(byte* pBytes, ref DateTimeContext context)
        //    {
        //        var ptr = pBytes + offset;

        //        OperationHelper.DivMod10Signed((int)((context.Ticks / TicksPerSecond) % 60), out var div, out var mod);
        //        *(ptr + 1) = (byte)(Num0 + mod);
        //        OperationHelper.DivMod10Signed(div, out div, out mod);
        //        *ptr = (byte)(Num0 + mod);
        //    }
        //}

        //public sealed class Millisecond3BlockConverter : IDateTimeBlockConverter
        //{
        //    private readonly int offset;

        //    public Millisecond3BlockConverter(int offset)
        //    {
        //        this.offset = offset;
        //    }

        //    public unsafe void Write(byte* pBytes, ref DateTimeContext context)
        //    {
        //        var ptr = pBytes + offset;

        //        OperationHelper.DivMod10Signed((int)((context.Ticks / TicksPerMillisecond) % 1000), out var div, out var mod);
        //        *(ptr + 2) = (byte)(Num0 + mod);
        //        OperationHelper.DivMod10Signed(div, out div, out mod);
        //        *(ptr + 1) = (byte)(Num0 + mod);
        //        *ptr = (byte)(Num0 + div);
        //    }
        //}

        //// Number args(Func, length)
        //// Decimal
    }
}
