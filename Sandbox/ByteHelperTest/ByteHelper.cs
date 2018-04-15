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
                for (copy = 1; copy <= length / 2; copy <<= 1)
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

        // TODO parse 1, 2, 3

        // TODO format

        //// TODO check, minus, trim ?
        //public static int ParseInteger(byte[] bytes, int index, int count)
        //{
        //    var end = index + count;

        //    // No check, simple version
        //    var value = 0;
        //    for (var i = index; i < end; i++)
        //    {
        //        value *= 10;
        //        value += bytes[i] - '0';
        //    }

        //    return value;
        //}

        //--------------------------------------------------------------------------------
        // Decimal
        //--------------------------------------------------------------------------------

        // TODO parse 1, 2, 3

        // TODO format -000123 ?

        //--------------------------------------------------------------------------------
        // DateTime
        //--------------------------------------------------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryParseDateTime(byte[] bytes, int index, string format, out DateTime dateTime)
        {
            var year = 0;
            var month = 0;
            var day = 0;
            var hour = 0;
            var minute = 0;
            var second = 0;
            var milisecond = 0;
            var msPow = 100;

            for (var i = 0; i < format.Length; i++)
            {
                var value = bytes[index + i] - '0';
                if ((value >= 0) && (value < 10))
                {
                    switch (format[i])
                    {
                        case 'y':
                            year = (year * 10) + value;
                            break;
                        case 'M':
                            month = (month * 10) + value;
                            break;
                        case 'd':
                            day = (day * 10) + value;
                            break;
                        case 'H':
                            hour = (hour * 10) + value;
                            break;
                        case 'm':
                            minute = (minute * 10) + value;
                            break;
                        case 's':
                            second = (second * 10) + value;
                            break;
                        case 'f':
                            milisecond = milisecond + (value * msPow);
                            msPow /= 10;
                            break;
                        default:
                            dateTime = default;
                            return false;
                    }
                }
                else if (format[i] == 'f')
                {
                    msPow /= 10;
                }
            }

            try
            {
                dateTime = new DateTime(year, month, day, hour, minute, second, milisecond);
                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                dateTime = default;
                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FormatDateTime(byte[] bytes, int index, string format, DateTime dateTime)
        {
            // TODO length
            for (var i = 0; i < format.Length; i++)
            {
                var c = format[i];

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
                        bytes[index + i] = (byte)c;
                        continue;
                }

                if (pow == 0)
                {
                    var append = 0;
                    for (var j = i + 1; j < format.Length; j++)
                    {
                        if (format[j] == c)
                        {
                            append++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    var start = index + i;
                    for (var j = start + append; j >= start; j--)
                    {
                        bytes[j] = (byte)('0' + (value % 10));
                        value = value / 10;
                    }

                    i += append;
                }
                else
                {
                    while (true)
                    {
                        var div = value / pow;
                        value = value % pow;
                        pow = pow / 10;

                        bytes[index + i] = (byte)('0' + div);

                        var next = i + 1;
                        if ((next < format.Length) && (format[next] == c))
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
