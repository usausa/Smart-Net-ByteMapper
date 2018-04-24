namespace Smart.IO.ByteMapper.Helpers
{
    using System;

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

        //--------------------------------------------------------------------------------
        // DateTime
        //--------------------------------------------------------------------------------

        public static unsafe bool TryParseDateTime(byte[] bytes, int index, string format, DateTimeKind kind, out DateTime value)
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
                        ? new DateTime(year, month, day, hour, minute, second, kind)
                        : new DateTime(year, month, day, hour, minute, second, milisecond, kind);
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
    }
}
