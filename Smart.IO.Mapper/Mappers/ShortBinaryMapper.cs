namespace Smart.IO.Mapper.Mappers
{
    using System;

    public sealed class BigEndianShortBinaryMapper : IMemberMapper
    {
        private readonly int offset;

        private readonly Func<object, object> getter;

        private readonly Action<object, object> setter;

        public BigEndianShortBinaryMapper(
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
            setter(target, ByteOrder.GetShortBE(buffer, index + offset));
        }

        public void Write(byte[] buffer, int index, object target)
        {
            ByteOrder.PutShortBE(buffer, index + offset, (short)getter(target));
        }
    }

    public sealed class LittleEndianShortBinaryMapper : IMemberMapper
    {
        private readonly int offset;

        private readonly Func<object, object> getter;

        private readonly Action<object, object> setter;

        public LittleEndianShortBinaryMapper(
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
            setter(target, ByteOrder.GetShortLE(buffer, index + offset));
        }

        public void Write(byte[] buffer, int index, object target)
        {
            ByteOrder.PutShortLE(buffer, index + offset, (short)getter(target));
        }
    }
}
