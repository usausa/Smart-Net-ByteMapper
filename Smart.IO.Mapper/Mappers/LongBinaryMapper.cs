namespace Smart.IO.Mapper.Mappers
{
    using System;

    public sealed class BigEndianLongBinaryMapper : IMemberMapper
    {
        private readonly int offset;

        private readonly Func<object, object> getter;

        private readonly Action<object, object> setter;

        public int Length => 8;

        public bool CanRead => getter != null;

        public bool CanWrite => setter != null;

        public BigEndianLongBinaryMapper(
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
            setter(target, ByteOrder.GetLongBE(buffer, index + offset));
        }

        public void Write(byte[] buffer, int index, object target)
        {
            ByteOrder.PutLongBE(buffer, index + offset, (long)getter(target));
        }
    }

    public sealed class LittleEndianLongBinaryMapper : IMemberMapper
    {
        private readonly int offset;

        private readonly Func<object, object> getter;

        private readonly Action<object, object> setter;

        public int Length => 8;

        public bool CanRead => getter != null;

        public bool CanWrite => setter != null;

        public LittleEndianLongBinaryMapper(
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
            setter(target, ByteOrder.GetLongLE(buffer, index + offset));
        }

        public void Write(byte[] buffer, int index, object target)
        {
            ByteOrder.PutLongLE(buffer, index + offset, (long)getter(target));
        }
    }
}
