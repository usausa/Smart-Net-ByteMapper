namespace Smart.IO.ByteMapper.Converters
{
    using System;

    using Smart.IO.ByteMapper.Helpers;

    internal sealed class DateTimeConverter : IMapConverter
    {
        private readonly string format;

        private readonly DateTimeKind kind;

        private readonly byte filler;

        private readonly object defaultValue;

        public DateTimeConverter(string format, DateTimeKind kind, byte filler, Type type)
        {
            this.format = format;
            this.kind = kind;
            this.filler = filler;
            defaultValue = type.GetDefaultValue();
        }

        public object Read(byte[] buffer, int index)
        {
            return DateTimeByteHelper.TryParseDateTime(buffer, index, format, kind, out var result)
                ? result
                : defaultValue;
        }

        public void Write(byte[] buffer, int index, object value)
        {
            if (value == null)
            {
                BytesHelper.Fill(buffer, index, format.Length, filler);
            }
            else
            {
                DateTimeByteHelper.FormatDateTime(buffer, index, format, (DateTime)value);
            }
        }
    }

    internal sealed class DateTimeOffsetConverter : IMapConverter
    {
        private static readonly long MaxTick = DateTime.MaxValue.Ticks;

        private readonly string format;

        private readonly DateTimeKind kind;

        private readonly byte filler;

        private readonly object defaultValue;

        public DateTimeOffsetConverter(string format, DateTimeKind kind, byte filler, Type type)
        {
            this.format = format;
            this.kind = kind;
            this.filler = filler;
            defaultValue = type.GetDefaultValue();
        }

        public object Read(byte[] buffer, int index)
        {
            if (DateTimeByteHelper.TryParseDateTime(buffer, index, format, kind, out var result))
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
                if ((utcTick >= 0) && (utcTick <= MaxTick))
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
                BytesHelper.Fill(buffer, index, format.Length, filler);
            }
            else
            {
                DateTimeByteHelper.FormatDateTime(buffer, index, format, ((DateTimeOffset)value).UtcDateTime);
            }
        }
    }
}
