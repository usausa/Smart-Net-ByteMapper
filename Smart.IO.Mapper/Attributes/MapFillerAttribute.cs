namespace Smart.IO.Mapper.Attributes
{
    using System;

    using Smart.IO.Mapper.Mappings;

    public sealed class MapFillerAttribute : AbstractTypeMappingAttribute
    {
        private readonly int length;

        private byte? filler;

        public byte Filler
        {
            get => throw new NotSupportedException();
            set => filler = value;
        }

        public MapFillerAttribute(int offset, int length)
            : base(offset)
        {
            this.length = length;
        }

        public override int CalcSize(Type type)
        {
            return length;
        }

        public override IMapping CreateMapping(IMappingCreateContext context, Type type)
        {
            return new FillMapping(
                Offset,
                length,
                filler ?? context.GetParameter<byte>(Parameter.Filler));
        }
    }
}
