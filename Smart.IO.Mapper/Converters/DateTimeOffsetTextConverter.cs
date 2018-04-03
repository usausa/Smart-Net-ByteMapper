namespace Smart.IO.Mapper.Converters
{
    using System;
    using System.Globalization;
    using System.Text;

    using Smart.IO.Mapper.Helpers;

    public sealed class DateTimeOffsetTextConverter : IByteConverter
    {
        private readonly int length;

        private readonly Encoding encoding;

        private readonly byte filler;

        private readonly string format;

        private readonly DateTimeStyles style;

        private readonly IFormatProvider provider;

        private readonly object defaultValue;

        public DateTimeOffsetTextConverter(
            int length,
            Encoding encoding,
            byte filler,
            string format,
            DateTimeStyles style,
            IFormatProvider provider,
            Type type)
        {
            this.length = length;
            this.encoding = encoding;
            this.filler = filler;
            this.format = format;
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
            if (value == null)
            {
                buffer.Fill(index, length, filler);
            }
            else
            {
                var bytes = encoding.GetBytes(((DateTimeOffset)value).ToString(format, provider));
                if (bytes.Length >= length)
                {
                    Buffer.BlockCopy(bytes, 0, buffer, index, length);
                }
                else
                {
                    BytesHelper.CopyPadRight(bytes, buffer, index, length, filler);
                }
            }
        }
    }
}
