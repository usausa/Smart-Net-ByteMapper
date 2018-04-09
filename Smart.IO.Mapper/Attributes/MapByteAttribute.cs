namespace Smart.IO.Mapper.Attributes
{
    using Smart.IO.Mapper.Builders;

    public sealed class MapByteAttribute : AbstractMapMemberAttribute
    {
        private readonly ByteConverterBuilder builder = new ByteConverterBuilder();

        public MapByteAttribute(int offset)
            : base(offset)
        {
        }

        public override IMapConverterBuilder GetConverterBuilder()
        {
            return builder;
        }
    }
}
