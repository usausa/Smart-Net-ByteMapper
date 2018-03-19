namespace Smart.IO.Mapper.Mappers
{
    using System;

    public sealed class BigEndianLongBinaryMapper : IMemberMapper
    {
        private readonly int offset;

        private readonly Func<object, object> getter;

        private readonly Action<object, object> setter;

        public BigEndianLongBinaryMapper(
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
            setter(target, ByteOrder.GetLongBE(buffer, offset));
        }

        public void Write(byte[] buffer, object target)
        {
            ByteOrder.PutLongBE(buffer, offset, (long)getter(target));
        }
    }

    public sealed class LittleEndianLongBinaryMapper : IMemberMapper
    {
        private readonly int offset;

        private readonly Func<object, object> getter;

        private readonly Action<object, object> setter;

        public LittleEndianLongBinaryMapper(
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
            setter(target, ByteOrder.GetLongLE(buffer, offset));
        }

        public void Write(byte[] buffer, object target)
        {
            ByteOrder.PutLongLE(buffer, offset, (long)getter(target));
        }
    }
}
