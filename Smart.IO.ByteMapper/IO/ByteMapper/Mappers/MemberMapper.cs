namespace Smart.IO.ByteMapper.Mappers
{
    using System;

    using Smart.IO.ByteMapper.Converters;

    public sealed class MemberMapper : IMapper
    {
        private readonly int offset;

        private readonly IMapConverter converter;

        private readonly Func<object, object> getter;

        private readonly Action<object, object> setter;

        public bool CanRead => getter != null;

        public bool CanWrite => setter != null;

        public MemberMapper(
            int offset,
            IMapConverter converter,
            Func<object, object> getter,
            Action<object, object> setter)
        {
            this.offset = offset;
            this.converter = converter;
            this.getter = getter;
            this.setter = setter;
        }

        public void Read(byte[] buffer, int index, object target)
        {
            setter(target, converter.Read(buffer, index + offset));
        }

        public void Write(byte[] buffer, int index, object target)
        {
            converter.Write(buffer, index + offset, getter(target));
        }
    }
}
