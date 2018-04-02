namespace Smart.IO.Mapper.Attributes
{
    using System;

    using Smart.IO.Mapper.Mappings;

    public sealed class MapFillterArrtibute : AbstractTypeMappingAttribute
    {
        private readonly int length;

        public byte? Filler { get; set; }

        public MapFillterArrtibute(int offset, int length)
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
                Filler ?? context.GetParameter<byte>(Parameter.Filler));
        }
    }
}
