namespace Smart.IO.ByteMapper.Builders
{
    using Smart.IO.ByteMapper.Mappers;

    public sealed class FillerTypeMapperBuilder : ITypeMapperBuilder
    {
        public int Offset { get; set; }

        public int Length { get; set; }

        public byte? Filler { get; set; }

        public int CalcSize()
        {
            return Length;
        }

        public IMapper CreateMapper(IBuilderContext context)
        {
            return new FillMapper(
                Offset,
                Length,
                Filler ?? context.GetParameter<byte>(Parameter.Filler));
        }
    }
}
