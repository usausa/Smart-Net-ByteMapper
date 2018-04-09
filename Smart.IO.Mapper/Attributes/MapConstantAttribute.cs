namespace Smart.IO.Mapper.Attributes
{
    using Smart.IO.Mapper.Builders;

    public sealed class MapConstantAttribute : AbstractMapTypeAttribute
    {
        private readonly ConstantTypeMapperBuilder builder = new ConstantTypeMapperBuilder();

        public MapConstantAttribute(int offset, byte[] content)
        {
            builder.Offset = offset;
            builder.Content = content;
        }

        public override ITypeMapperBuilder GetTypeMapperBuilder()
        {
            return builder;
        }
    }
}
