namespace Smart.IO.ByteMapper.Converters
{
    using System;
    using System.Globalization;
    using System.Text;

    using Smart.IO.ByteMapper.Helpers;

    internal sealed class IntTextConverter : IMapConverter
    {
        private readonly int length;

        private readonly string format;

        private readonly Encoding encoding;

        private readonly bool trim;

        private readonly Padding padding;

        private readonly byte filler;

        private readonly NumberStyles style;

        private readonly IFormatProvider provider;

        private readonly Type convertEnumType;

        private readonly object defaultValue;

        public IntTextConverter(
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
            convertEnumType = BytesHelper.GetConvertEnumType(type);
            defaultValue = type.GetDefaultValue();
        }

        public object Read(byte[] buffer, int index)
        {
            var start = index;
            var size = length;
            if (trim)
            {
                BytesHelper.TrimRange(buffer, ref start, ref size, padding, filler);
            }

            var value = encoding.GetString(buffer, start, size);
            if ((value.Length > 0) && Int32.TryParse(value, style, provider, out var result))
            {
                return convertEnumType != null ? Enum.ToObject(convertEnumType, result) : result;
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
                BytesHelper.CopyBytes(encoding.GetBytes(((int)value).ToString(format, provider)), buffer, index, length, padding, filler);
            }
        }
    }

    internal sealed class LongTextConverter : IMapConverter
    {
        private readonly int length;

        private readonly string format;

        private readonly Encoding encoding;

        private readonly bool trim;

        private readonly Padding padding;

        private readonly byte filler;

        private readonly NumberStyles style;

        private readonly IFormatProvider provider;

        private readonly Type convertEnumType;

        private readonly object defaultValue;

        public LongTextConverter(
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
            convertEnumType = BytesHelper.GetConvertEnumType(type);
            defaultValue = type.GetDefaultValue();
        }

        public object Read(byte[] buffer, int index)
        {
            var start = index;
            var size = length;
            if (trim)
            {
                BytesHelper.TrimRange(buffer, ref start, ref size, padding, filler);
            }

            var value = encoding.GetString(buffer, start, size);
            if ((value.Length > 0) && Int64.TryParse(value, style, provider, out var result))
            {
                return convertEnumType != null ? Enum.ToObject(convertEnumType, result) : result;
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
                BytesHelper.CopyBytes(encoding.GetBytes(((long)value).ToString(format, provider)), buffer, index, length, padding, filler);
            }
        }
    }

    internal sealed class ShortTextConverter : IMapConverter
    {
        private readonly int length;

        private readonly string format;

        private readonly Encoding encoding;

        private readonly bool trim;

        private readonly Padding padding;

        private readonly byte filler;

        private readonly NumberStyles style;

        private readonly IFormatProvider provider;

        private readonly Type convertEnumType;

        private readonly object defaultValue;

        public ShortTextConverter(
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
            convertEnumType = BytesHelper.GetConvertEnumType(type);
            defaultValue = type.GetDefaultValue();
        }

        public object Read(byte[] buffer, int index)
        {
            var start = index;
            var size = length;
            if (trim)
            {
                BytesHelper.TrimRange(buffer, ref start, ref size, padding, filler);
            }

            var value = encoding.GetString(buffer, start, size);
            if ((value.Length > 0) && Int16.TryParse(value, style, provider, out var result))
            {
                return convertEnumType != null ? Enum.ToObject(convertEnumType, result) : result;
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
                BytesHelper.CopyBytes(encoding.GetBytes(((short)value).ToString(format, provider)), buffer, index, length, padding, filler);
            }
        }
    }

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
            var start = index;
            var size = length;
            if (trim)
            {
                BytesHelper.TrimRange(buffer, ref start, ref size, padding, filler);
            }

            var value = encoding.GetString(buffer, start, size);
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
                BytesHelper.CopyBytes(encoding.GetBytes(((decimal)value).ToString(format, provider)), buffer, index, length, padding, filler);
            }
        }
    }
}
