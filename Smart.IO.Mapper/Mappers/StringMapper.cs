namespace Smart.IO.Mapper.Mappers
{
    using System;
    using System.Text;

    using Smart.IO.Mapper.Helpers;

    public sealed class StringMapper : IMemberMapper
    {
        private readonly int offset;

        private readonly int length;

        private readonly Func<object, object> getter;

        private readonly Action<object, object> setter;

        private readonly Encoding encoding;

        private readonly bool trim;

        private readonly Padding padding;

        private readonly byte filler;

        public StringMapper(
            int offset,
            int length,
            Func<object, object> getter,
            Action<object, object> setter,
            Encoding encoding,
            bool trim,
            Padding padding,
            byte filler)
        {
            this.offset = offset;
            this.length = length;
            this.getter = getter;
            this.setter = setter;
            this.encoding = encoding;
            this.trim = trim;
            this.padding = padding;
            this.filler = filler;
        }

        public void Read(byte[] buffer, int index, object target)
        {
            var value = BytesHelper.ReadString(buffer, index + offset, length, encoding, trim, padding, filler);
            setter(target, value);
        }

        public void Write(byte[] buffer, int index, object target)
        {
            var value = (string)getter(target);
            if (value == null)
            {
                buffer.Fill(offset, length, filler);
            }
            else
            {
                BytesHelper.WriteString(value, buffer, index + offset, length, encoding, padding, filler);
            }
        }
    }
}
