namespace Smart.IO.Mapper.Attributes
{
    using Smart.IO.Mapper.Mappings;

    public sealed class FillterArrtibute : AbstractTypeMappingAttribute
    {
        public int Length { get; set; }

        public byte? Filler { get; set; }

        public FillterArrtibute(int offset)
            : base(offset)
        {
        }

        public override IMapping CreateMapping(IMappingCreateContext context)
        {
            return new FillMapping(
                Offset,
                Length,
                Filler ?? context.GetParameterOr<byte>(DefaultParameter.Filler, 0x00));
        }
    }
}
