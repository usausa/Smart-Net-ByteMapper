namespace Smart.IO.ByteMapper.Builders
{
    using Smart.IO.ByteMapper.Mappers;

    public sealed class ConstantTypeMapperBuilder : ITypeMapperBuilder
    {
        public int Offset { get; set; }

        public byte[] Content { get; set; }

        public int CalcSize()
        {
            return Content.Length;
        }

        public IMapper CreateMapper(IBuilderContext context)
        {
            return new ConstantMapper(Offset, Content);
        }
    }
}
