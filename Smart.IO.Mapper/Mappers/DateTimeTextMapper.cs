namespace Smart.IO.Mapper.Mappers
{
    using System;
    using System.Globalization;
    using System.Text;

    public sealed class DateTimeTextMapper : IMemberMapper
    {
        private readonly int offset;

        private readonly Func<object, object> getter;

        private readonly Action<object, object> setter;

        private readonly Encoding encoding;

        private readonly byte filler;

        private readonly string format;

        private readonly DateTimeStyles style;

        private readonly DateTimeFormatInfo info;

        private readonly object defaultValue;

        public int Length { get; }

        public bool CanRead => getter != null;

        public bool CanWrite => setter != null;

        public DateTimeTextMapper(
            int offset,
            Func<object, object> getter,
            Action<object, object> setter,
            Encoding encoding,
            byte filler,
            string format,
            DateTimeStyles style,
            DateTimeFormatInfo info,
            Type type)
        {
            this.offset = offset;
            Length = format.Length;
            this.getter = getter;
            this.setter = setter;
            this.encoding = encoding;
            this.filler = filler;
            this.format = format;
            this.style = style;
            this.info = info;
            defaultValue = type.GetDefaultValue();
        }

        public void Read(byte[] buffer, int index, object target)
        {
            var value = encoding.GetString(buffer, index + offset, Length);
            if ((value.Length > 0) && DateTime.TryParseExact(value, format, info, style, out var result))
            {
                setter(target, result);
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
                var bytes = encoding.GetBytes(((DateTime)value).ToString(format, info));
                Buffer.BlockCopy(bytes, 0, buffer, offset + Length - bytes.Length, bytes.Length);
            }
        }
    }
}
