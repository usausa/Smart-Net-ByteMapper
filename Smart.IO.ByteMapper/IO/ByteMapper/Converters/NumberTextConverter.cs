namespace Smart.IO.ByteMapper.Converters
{
    using System;
    using System.Globalization;
    using System.Text;

    using Smart.IO.ByteMapper.Helpers;

    internal sealed class Int32TextConverter : IMapConverter
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

        public Int32TextConverter(
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
            convertEnumType = EnumHelper.GetConvertEnumType(type);
            defaultValue = type.GetDefaultValue();
        }

        public object Read(byte[] buffer, int index)
        {
            var start = index;
            var count = length;
            if (trim)
            {
                BytesHelper.TrimRange(buffer, ref start, ref count, padding, filler);
            }

            var value = encoding.GetString(buffer, start, count);
            if ((value.Length > 0) && Int32.TryParse(value, style, provider, out var result))
            {
                return convertEnumType is null ? result : Enum.ToObject(convertEnumType, result);
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
                BytesHelper.CopyBytes(encoding.GetBytes(((int)value).ToString(format, provider)), buffer, index, length, padding, filler);
            }
        }
    }

    internal sealed class Int64TextConverter : IMapConverter
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

        public Int64TextConverter(
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
            convertEnumType = EnumHelper.GetConvertEnumType(type);
            defaultValue = type.GetDefaultValue();
        }

        public object Read(byte[] buffer, int index)
        {
            var start = index;
            var count = length;
            if (trim)
            {
                BytesHelper.TrimRange(buffer, ref start, ref count, padding, filler);
            }

            var value = encoding.GetString(buffer, start, count);
            if ((value.Length > 0) && Int64.TryParse(value, style, provider, out var result))
            {
                return convertEnumType is null ? result : Enum.ToObject(convertEnumType, result);
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
                BytesHelper.CopyBytes(encoding.GetBytes(((long)value).ToString(format, provider)), buffer, index, length, padding, filler);
            }
        }
    }

    internal sealed class Int16TextConverter : IMapConverter
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

        public Int16TextConverter(
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
            convertEnumType = EnumHelper.GetConvertEnumType(type);
            defaultValue = type.GetDefaultValue();
        }

        public object Read(byte[] buffer, int index)
        {
            var start = index;
            var count = length;
            if (trim)
            {
                BytesHelper.TrimRange(buffer, ref start, ref count, padding, filler);
            }

            var value = encoding.GetString(buffer, start, count);
            if ((value.Length > 0) && Int16.TryParse(value, style, provider, out var result))
            {
                return convertEnumType is null ? result : Enum.ToObject(convertEnumType, result);
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
            var count = length;
            if (trim)
            {
                BytesHelper.TrimRange(buffer, ref start, ref count, padding, filler);
            }

            var value = encoding.GetString(buffer, start, count);
            if ((value.Length > 0) && Decimal.TryParse(value, style, provider, out var result))
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
                BytesHelper.CopyBytes(encoding.GetBytes(((decimal)value).ToString(format, provider)), buffer, index, length, padding, filler);
            }
        }
    }
}
