namespace Smart.IO.Mapper.Mappers
{
    using System;

    public sealed class BigEndianIntBinaryMapper : IMemberMapper
    {
        private readonly int offset;

        private readonly Func<object, object> getter;

        private readonly Action<object, object> setter;

        public BigEndianIntBinaryMapper(
            int offset,
            Func<object, object> getter,
            Action<object, object> setter)
        {
            this.offset = offset;
            this.getter = getter;
            this.setter = setter;
        }

        public void Read(byte[] buffer, object target)
        {
            setter(target, ByteOrder.GetIntBE(buffer, offset));
        }

        public void Write(byte[] buffer, object target)
        {
            ByteOrder.PutIntBE(buffer, offset, (int)getter(target));
        }
    }

    public sealed class LittleEndianIntBinaryMapper : IMemberMapper
    {
        private readonly int offset;

        private readonly Func<object, object> getter;

        private readonly Action<object, object> setter;

        public LittleEndianIntBinaryMapper(
            int offset,
            Func<object, object> getter,
            Action<object, object> setter)
        {
            this.offset = offset;
            this.getter = getter;
            this.setter = setter;
        }

        public void Read(byte[] buffer, object target)
        {
            setter(target, ByteOrder.GetIntLE(buffer, offset));
        }

        public void Write(byte[] buffer, object target)
        {
            ByteOrder.PutIntLE(buffer, offset, (int)getter(target));
        }
    }
}
