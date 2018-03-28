namespace Smart.IO.Mapper.Converters
{
    using System;
    using System.Globalization;
    using System.Text;

    using Smart.IO.Mapper.Helpers;

    public sealed class DateTimeOffsetTextConverter : IByteConverter
    {
        private readonly Encoding encoding;

        private readonly byte filler;

        private readonly string format;

        private readonly DateTimeStyles style;

        private readonly IFormatProvider provider;

        private readonly object defaultValue;

        public int Length { get; }

        public DateTimeOffsetTextConverter(
            int length,
            Encoding encoding,
            byte filler,
            string format,
            DateTimeStyles style,
            IFormatProvider provider,
            Type type)
        {
            Length = length;
            this.encoding = encoding;
            this.filler = filler;
            this.format = format;
            this.style = style;
            this.provider = provider;
            defaultValue = type.GetDefaultValue();
        }

        public object Read(byte[] buffer, int index)
        {
            var value = encoding.GetString(buffer, index, Length);
            if ((value.Length > 0) && DateTimeOffset.TryParseExact(value, format, provider, style, out var result))
            {
                return result;
            }

            return defaultValue;
        }

        public void Write(byte[] buffer, int index, object value)
        {
            if (value == null)
            {
                buffer.Fill(index, Length, filler);
            }
            else
            {
                var bytes = encoding.GetBytes(((DateTimeOffset)value).ToString(format, provider));
                if (bytes.Length >= Length)
                {
                    Buffer.BlockCopy(bytes, 0, buffer, index, Length);
                }
                else
                {
                    BytesHelper.CopyPadRight(bytes, buffer, index, Length, filler);
                }
            }
        }
    }
}
