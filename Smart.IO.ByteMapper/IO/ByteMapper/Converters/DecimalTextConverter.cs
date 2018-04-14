namespace Smart.IO.ByteMapper.Converters
{
    using System;
    using System.Globalization;
    using System.Text;

    using Smart.IO.ByteMapper.Helpers;

    internal sealed class DecimalTextConverter : IMapConverter
    {
        private readonly int length;

        private readonly string format;

        private readonly Encoding encoding;

        private readonly bool trim;

        private readonly Padding padding;

        private readonly byte filler;

        private readonly NumberStyles style;

        private readonly IFormatProvider provider;

        private readonly object defaultValue;

        public DecimalTextConverter(
            int length,
            string format,
            Encoding encoding,
            bool trim,
            Padding padding,
            byte filler,
            NumberStyles style,
            IFormatProvider provider,
            Type type)
        {
            this.length = length;
            this.format = format;
            this.encoding = encoding;
            this.trim = trim;
            this.padding = padding;
            this.filler = filler;
            this.style = style;
            this.provider = provider;
            defaultValue = type.GetDefaultValue();
        }

        public object Read(byte[] buffer, int index)
        {
            var value = BytesHelper.ReadString(buffer, index, length, encoding, trim, padding, filler);
            if ((value.Length > 0) && Decimal.TryParse(value, style, provider, out var result))
            {
                return result;
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
                BytesHelper.WriteString(((decimal)value).ToString(format, provider), buffer, index, length, encoding, padding, filler);
            }
        }
    }
}
