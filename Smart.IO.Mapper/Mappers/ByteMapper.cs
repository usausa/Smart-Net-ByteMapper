namespace Smart.IO.Mapper.Mappers
{
    using System;

    public sealed class ByteMapper : IMemberMapper
    {
        private readonly int offset;

        private readonly Func<object, object> getter;

        private readonly Action<object, object> setter;

        public ByteMapper(
            int offset,
            Func<object, object> getter,
            Action<object, object> setter)
        {
            this.offset = offset;
            this.getter = getter;
            this.setter = setter;
        }

        public void Read(byte[] buffer, int index, object target)
        {
            setter(target, buffer[index + offset]);
        }

        public void Write(byte[] buffer, int index, object target)
        {
            buffer[index + offset] = (byte)getter(target);
        }
    }
}
