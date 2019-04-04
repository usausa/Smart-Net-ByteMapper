namespace Smart.IO.ByteMapper.Builders
{
    using Smart.IO.ByteMapper.Mappers;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Ignore")]
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
