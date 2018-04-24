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
            return DateTimeHelper.TryParseDateTime(buffer, index, format, kind, out var result)
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
                DateTimeHelper.FormatDateTime(buffer, index, format, (DateTime)value);
            }
        }
    }

    internal sealed class DateTimeOffsetConverter : IMapConverter
    {
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
            return DateTimeHelper.TryParseDateTime(buffer, index, format, kind, out var result)
                ? (DateTimeOffset)result
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
                DateTimeHelper.FormatDateTime(buffer, index, format, ((DateTimeOffset)value).DateTime);
            }
        }
    }
}
