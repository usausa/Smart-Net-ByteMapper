namespace Smart.IO.Mapper.Attributes
{
    using System;

    using Smart.IO.Mapper.Converters;

    public sealed class MapBytesAttribute : AbstractPropertyAttribute
    {
        private readonly int length;

        private byte? filler;

        public byte Filler
        {
            get => throw new NotSupportedException();
            set => filler = value;
        }

        public MapBytesAttribute(int offset, int length)
            : base(offset)
        {
            this.length = length;
        }

        public override int CalcSize(Type type)
        {
            return length;
        }

        public override IByteConverter CreateConverter(IMappingCreateContext context, Type type)
        {
            if (type == typeof(byte[]))
            {
                return new BytesConverter(
                    length,
                    filler ?? context.GetParameter<byte>(Parameter.Filler));
            }

            return null;
        }
    }
}
