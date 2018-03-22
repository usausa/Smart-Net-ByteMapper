namespace Smart.IO.Mapper.Mappers
{
    using System;
    using System.Globalization;
    using System.Text;

    using Smart.IO.Mapper.Helpers;

    public sealed class IntTextMapper : IMemberMapper
    {
        private readonly int offset;

        private readonly int length;

        private readonly Func<object, object> getter;

        private readonly Action<object, object> setter;

        private readonly Encoding encoding;

        private readonly bool trim;

        private readonly Padding padding;

        private readonly byte filler;

        private readonly NumberStyles style;

        private readonly NumberFormatInfo info;

        private readonly Type convertEnumType;

        private readonly object defaultValue;

        public IntTextMapper(
            int offset,
            int length,
            Func<object, object> getter,
            Action<object, object> setter,
            Encoding encoding,
            bool trim,
            Padding padding,
            byte filler,
            NumberStyles style,
            NumberFormatInfo info,
            Type type)
        {
            this.offset = offset;
            this.length = length;
            this.getter = getter;
            this.setter = setter;
            this.encoding = encoding;
            this.trim = trim;
            this.padding = padding;
            this.filler = filler;
            this.style = style;
            this.info = info;
            convertEnumType = BytesHelper.GetConvertEnumType(type);
            defaultValue = type.GetDefaultValue();
        }

        public void Read(byte[] buffer, int index, object target)
        {
            var value = BytesHelper.ReadString(buffer, index + offset, length, encoding, trim, padding, filler);
            if ((value.Length > 0) && Int32.TryParse(value, style, info, out var result))
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
                buffer.Fill(offset, length, filler);
            }
            else
            {
                BytesHelper.WriteString(((int)value).ToString(info), buffer, index + offset, length, encoding, padding, filler);
            }
        }
    }
}
