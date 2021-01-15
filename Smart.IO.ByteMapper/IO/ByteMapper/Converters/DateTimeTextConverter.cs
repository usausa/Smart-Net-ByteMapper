namespace Smart.IO.ByteMapper.Converters
{
    using System;
    using System.Globalization;
    using System.Text;

    using Smart.IO.ByteMapper.Helpers;

    internal sealed class DateTimeTextConverter : IMapConverter
    {
        private readonly int length;

        private readonly string format;

        private readonly Encoding encoding;

        private readonly byte filler;

        private readonly DateTimeStyles style;

        private readonly IFormatProvider provider;

        private readonly object defaultValue;

        public DateTimeTextConverter(
            int length,
            string format,
            Encoding encoding,
            byte filler,
            DateTimeStyles style,
            IFormatProvider provider,
            Type type)
        {
            this.length = length;
            this.format = format;
            this.encoding = encoding;
            this.filler = filler;
            this.style = style;
            this.provider = provider;
            defaultValue = type.GetDefaultValue();
        }

        public object Read(byte[] buffer, int index)
        {
            var value = encoding.GetString(buffer, index, length);
            if (DateTime.TryParseExact(value, format, provider, style, out var result))
            {
                return result;
            }

            return defaultValue;
        }

        public void Write(byte[] buffer, int index, object value)
        {
            if (value is null)
            {
                BytesHelper.Fill(buffer, index, length, filler);
            }
            else
            {
                var bytes = encoding.GetBytes(((DateTime)value).ToString(format, provider));
                BytesHelper.CopyBytes(bytes, buffer, index, length, Padding.Right, filler);
            }
        }
    }

    internal sealed class DateTimeOffsetTextConverter : IMapConverter
    {
        private readonly int length;

        private readonly string format;

        private readonly Encoding encoding;

        private readonly byte filler;

        private readonly DateTimeStyles style;

        private readonly IFormatProvider provider;

        private readonly object defaultValue;

        public DateTimeOffsetTextConverter(
            int length,
            string format,
            Encoding encoding,
            byte filler,
            DateTimeStyles style,
            IFormatProvider provider,
            Type type)
        {
            this.length = length;
            this.format = format;
            this.encoding = encoding;
            this.filler = filler;
            this.style = style;
            this.provider = provider;
            defaultValue = type.GetDefaultValue();
        }

        public object Read(byte[] buffer, int index)
        {
            var value = encoding.GetString(buffer, index, length);
            if (DateTimeOffset.TryParseExact(value, format, provider, style, out var result))
            {
                return result;
            }

            return defaultValue;
        }

        public void Write(byte[] buffer, int index, object value)
        {
            if (value is null)
            {
                BytesHelper.Fill(buffer, index, length, filler);
            }
            else
            {
                var bytes = encoding.GetBytes(((DateTimeOffset)value).ToString(format, provider));
                BytesHelper.CopyBytes(bytes, buffer, index, length, Padding.Right, filler);
            }
        }
    }
}
