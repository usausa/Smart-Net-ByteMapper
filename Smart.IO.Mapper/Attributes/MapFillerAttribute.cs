namespace Smart.IO.Mapper.Attributes
{
    using System;

    using Smart.ComponentModel;
    using Smart.IO.Mapper.Helpers;
    using Smart.IO.Mapper.Mappers;

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

        public override IMapper CreateMapper(IComponentContainer components, IMappingParameter parameters, Type type)
        {
            return new FillMapper(
                Offset,
                length,
                filler ?? parameters.GetParameter<byte>(Parameter.Filler));
        }
    }
}
