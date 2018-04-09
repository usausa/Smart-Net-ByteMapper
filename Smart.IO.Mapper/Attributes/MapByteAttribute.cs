namespace Smart.IO.Mapper.Attributes
{
    using Smart.IO.Mapper.Builders;

    public sealed class MapByteAttribute : AbstractMemberMapAttribute
    {
        public MapByteAttribute(int offset)
            : base(offset)
        {
        }

        public override IMapConverterBuilder GetConverterBuilder()
        {
            return ByteConverterBuilder.Default;
        }
    }
}
