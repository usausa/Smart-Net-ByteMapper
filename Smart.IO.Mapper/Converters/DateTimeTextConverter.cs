namespace Smart.IO.Mapper.Converters
{
    using System;
    using System.Globalization;
    using System.Text;

    using Smart.IO.Mapper.Helpers;

    public sealed class DateTimeTextConverter : IByteConverter
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
            if (value == null)
            {
                buffer.Fill(index, length, filler);
            }
            else
            {
                var bytes = encoding.GetBytes(((DateTime)value).ToString(format, provider));
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
