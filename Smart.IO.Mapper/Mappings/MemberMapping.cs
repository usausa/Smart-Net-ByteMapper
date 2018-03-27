namespace Smart.IO.Mapper.Mappings
{
    using System;

    using Smart.IO.Mapper.Converters;

    public sealed class MemberMapping : IMapping
    {
        private readonly int offset;

        private readonly IByteConverter converter;

        private readonly Func<object, object> getter;

        private readonly Action<object, object> setter;

        public bool CanRead => getter != null;

        public bool CanWrite => setter != null;

        public MemberMapping(
            int offset,
            IByteConverter converter,
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