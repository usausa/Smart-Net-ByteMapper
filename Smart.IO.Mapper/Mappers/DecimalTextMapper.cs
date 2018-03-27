namespace Smart.IO.Mapper.Mappers
{
    using System;
    using System.Globalization;
    using System.Text;

    using Smart.IO.Mapper.Helpers;

    public sealed class DecimalTextMapper : IMemberMapper
    {
        private readonly int length;

        private readonly Encoding encoding;

        private readonly bool trim;

        private readonly Padding padding;

        private readonly byte filler;

        private readonly NumberStyles style;

        private readonly NumberFormatInfo info;

        private readonly object defaultValue;

        public DecimalTextMapper(
            int length,
            Encoding encoding,
            bool trim,
            Padding padding,
            byte filler,
            NumberStyles style,
            NumberFormatInfo info,
            Type type)
        {
            this.length = length;
            this.encoding = encoding;
            this.trim = trim;
            this.padding = padding;
            this.filler = filler;
            this.style = style;
            this.info = info;
            defaultValue = type.GetDefaultValue();
        }

        public object Read(byte[] buffer, int index)
        {
            var value = BytesHelper.ReadString(buffer, index, length, encoding, trim, padding, filler);
            if ((value.Length > 0) && Decimal.TryParse(value, style, info, out var result))
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
                BytesHelper.WriteString(((decimal)value).ToString(info), buffer, index, length, encoding, padding, filler);
            }
        }
    }
}
