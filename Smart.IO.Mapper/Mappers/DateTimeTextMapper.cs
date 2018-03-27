namespace Smart.IO.Mapper.Mappers
{
    using System;
    using System.Globalization;
    using System.Text;

    using Smart.IO.Mapper.Helpers;

    public sealed class DateTimeTextMapper : IMemberMapper
    {
        private readonly int length;

        private readonly Encoding encoding;

        private readonly byte filler;

        private readonly string format;

        private readonly DateTimeStyles style;

        private readonly DateTimeFormatInfo info;

        private readonly object defaultValue;

        public DateTimeTextMapper(
            int length,
            Encoding encoding,
            byte filler,
            string format,
            DateTimeStyles style,
            DateTimeFormatInfo info,
            Type type)
        {
            this.length = length;
            this.encoding = encoding;
            this.filler = filler;
            this.format = format;
            this.style = style;
            this.info = info;
            defaultValue = type.GetDefaultValue();
        }

        public object Read(byte[] buffer, int index)
        {
            var value = encoding.GetString(buffer, index, length);
            if ((value.Length > 0) && DateTime.TryParseExact(value, format, info, style, out var result))
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
                var bytes = encoding.GetBytes(((DateTime)value).ToString(format, info));
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
