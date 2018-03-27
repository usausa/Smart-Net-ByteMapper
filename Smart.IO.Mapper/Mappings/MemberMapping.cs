namespace Smart.IO.Mapper.Mappings
{
    using System;

    using Smart.IO.Mapper.Mappers;

    public sealed class MemberMapping : IMapping
    {
        private readonly int offset;

        private readonly IMemberMapper mapper;

        private readonly Func<object, object> getter;

        private readonly Action<object, object> setter;

        public bool CanRead => getter != null;

        public bool CanWrite => setter != null;

        public MemberMapping(
            int offset,
            IMemberMapper mapper,
            Func<object, object> getter,
            Action<object, object> setter)
        {
            this.offset = offset;
            this.mapper = mapper;
            this.getter = getter;
            this.setter = setter;
        }

        public void Read(byte[] buffer, int index, object target)
        {
            setter(target, mapper.Read(buffer, index + offset));
        }

        public void Write(byte[] buffer, int index, object target)
        {
            mapper.Write(buffer, index + offset, getter(target));
        }
    }
}