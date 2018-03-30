namespace Smart.IO.Mapper.Attributes
{
    using System;

    using Smart.IO.Mapper.Mappings;

    public sealed class FillterArrtibute : AbstractTypeMappingAttribute
    {
        public int Length { get; }

        public byte? Filler { get; set; }

        public FillterArrtibute(int offset, int length)
            : base(offset)
        {
            Length = length;
        }

        public override int CalcSize(Type type)
        {
            return Length;
        }

        public override IMapping CreateMapping(IMappingCreateContext context, Type type)
        {
            return new FillMapping(
                Offset,
                Length,
                Filler ?? context.GetParameter<byte>(Parameter.Filler));
        }
    }
}
