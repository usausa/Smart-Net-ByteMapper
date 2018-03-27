namespace Smart.IO.Mapper.Mappers
{
    using System;
    using System.Globalization;
    using System.Text;

    using Smart.IO.Mapper.Helpers;

    public sealed class ShortTextMapper : IMemberMapper
    {
        private readonly int offset;

        private readonly Func<object, object> getter;

        private readonly Action<object, object> setter;

        private readonly Encoding encoding;

        private readonly bool trim;

        private readonly Padding padding;

        private readonly byte filler;

        private readonly NumberStyles style;

        private readonly IFormatProvider provider;

        private readonly Type convertEnumType;

        private readonly object defaultValue;

        public int Length { get; }

        public bool CanRead => getter != null;

        public bool CanWrite => setter != null;

        public ShortTextMapper(
            int offset,
            int length,
            Func<object, object> getter,
            Action<object, object> setter,
            Encoding encoding,
            bool trim,
            Padding padding,
            byte filler,
            NumberStyles style,
            NumberFormatInfo provider,
            Type type)
        {
            this.offset = offset;
            Length = length;
            this.getter = getter;
            this.setter = setter;
            this.encoding = encoding;
            this.trim = trim;
            this.padding = padding;
            this.filler = filler;
            this.style = style;
            this.provider = provider;
            convertEnumType = BytesHelper.GetConvertEnumType(type);
            defaultValue = type.GetDefaultValue();
        }

        public void Read(byte[] buffer, int index, object target)
        {
            var value = BytesHelper.ReadString(buffer, index + offset, Length, encoding, trim, padding, filler);
            if ((value.Length > 0) && Int16.TryParse(value, style, provider, out var result))
            {
                setter(target, convertEnumType != null ? Enum.ToObject(convertEnumType, result) : result);
            }
            else
            {
                setter(target, defaultValue);
            }
        }

        public void Write(byte[] buffer, int index, object target)
        {
            var value = getter(target);
            if (value == null)
            {
                buffer.Fill(offset, Length, filler);
            }
            else
            {
                BytesHelper.WriteString(((short)value).ToString(provider), buffer, index + offset, Length, encoding, padding, filler);
            }
        }
    }
}
