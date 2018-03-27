﻿namespace Smart.IO.Mapper.Mappers
{
    using System;
    using System.Globalization;
    using System.Text;

    using Smart.IO.Mapper.Helpers;

    public sealed class IntTextMapper : IMemberMapper
    {
        private readonly int length;

        private readonly Encoding encoding;

        private readonly bool trim;

        private readonly Padding padding;

        private readonly byte filler;

        private readonly NumberStyles style;

        private readonly IFormatProvider provider;

        private readonly Type convertEnumType;

        private readonly object defaultValue;

        public IntTextMapper(
            int length,
            Encoding encoding,
            bool trim,
            Padding padding,
            byte filler,
            NumberStyles style,
            IFormatProvider provider,
            Type type)
        {
            this.length = length;
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
            var value = BytesHelper.ReadString(buffer, index, length, encoding, trim, padding, filler);
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
                buffer.Fill(index, length, filler);
            }
            else
            {
                BytesHelper.WriteString(((int)value).ToString(provider), buffer, index, length, encoding, padding, filler);
            }
        }
    }
}
