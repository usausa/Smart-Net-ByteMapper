namespace Smart.IO.ByteMapper.Converters
{
    using System;

    using Smart.IO.ByteMapper.Helpers;

    internal sealed class DateTimeConverter : IMapConverter
    {
        private readonly int length;

        private readonly bool hasDatePart;

        private readonly DateTimeFormatEntry[] entries;

        private readonly DateTimeKind kind;

        private readonly byte filler;

        private readonly object defaultValue;

        public DateTimeConverter(string format, DateTimeKind kind, byte filler, Type type)
        {
            length = format.Length;
            entries = DateTimeByteHelper.ParseDateTimeFormat(format, out hasDatePart);
            this.kind = kind;
            this.filler = filler;
            defaultValue = type.GetDefaultValue();
        }

        public object Read(byte[] buffer, int index)
        {
            return DateTimeByteHelper.TryParseDateTime(buffer, index, entries, kind, out var result)
                ? result
                : defaultValue;
        }

        public void Write(byte[] buffer, int index, object value)
        {
            if (value == null)
            {
                BytesHelper.Fill(buffer, index, length, filler);
            }
            else
            {
                DateTimeByteHelper.FormatDateTime(buffer, index, hasDatePart, entries, (DateTime)value);
            }
        }
    }

    internal sealed class DateTimeOffsetConverter : IMapConverter
    {
        private readonly int length;

        private readonly bool hasDatePart;

        private readonly DateTimeFormatEntry[] entries;

        private readonly DateTimeKind kind;

        private readonly byte filler;

        private readonly object defaultValue;

        public DateTimeOffsetConverter(string format, DateTimeKind kind, byte filler, Type type)
        {
            length = format.Length;
            entries = DateTimeByteHelper.ParseDateTimeFormat(format, out hasDatePart);
            this.kind = kind;
            this.filler = filler;
            defaultValue = type.GetDefaultValue();
        }

        public object Read(byte[] buffer, int index)
        {
            if (DateTimeByteHelper.TryParseDateTime(buffer, index, entries, kind, out var result))
            {
                if (kind == DateTimeKind.Unspecified)
                {
                    return new DateTimeOffset(result, TimeSpan.Zero);
                }

                if (kind == DateTimeKind.Utc)
                {
                    return new DateTimeOffset(result);
                }

                var offset = TimeZoneInfo.Local.GetUtcOffset(result);
                var utcTick = result.Ticks - offset.Ticks;
                if (DateTimeHelper.IsValidTicks(utcTick))
                {
                    return new DateTimeOffset(result, offset);
                }
            }

            return defaultValue;
        }

        public void Write(byte[] buffer, int index, object value)
        {
            if (value == null)
            {
                BytesHelper.Fill(buffer, index, length, filler);
            }
            else
            {
                DateTimeByteHelper.FormatDateTime(buffer, index, hasDatePart, entries, ((DateTimeOffset)value).UtcDateTime);
            }
        }
    }
}
