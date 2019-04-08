namespace Smart.IO.ByteMapper.Attributes
{
    using Smart.IO.ByteMapper.Builders;

    public sealed class MapByteAttribute : AbstractMemberMapAttribute
    {
        public MapByteAttribute(int offset)
            : base(offset)
        {
        }

        public override IMapConverterBuilder GetConverterBuilder() => ByteConverterBuilder.Default;
    }
}
