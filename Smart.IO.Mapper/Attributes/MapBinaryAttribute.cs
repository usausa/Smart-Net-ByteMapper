namespace Smart.IO.Mapper.Attributes
{
    using System;

    using Smart.IO.Mapper.Builders;

    public sealed class MapBinaryAttribute : AbstractMemberMapAttribute
    {
        private readonly BinaryConverterBuilder builder = new BinaryConverterBuilder();

        public Endian Endian
        {
            get => throw new NotSupportedException();
            set => builder.Endian = value;
        }

        public MapBinaryAttribute(int offset)
            : base(offset)
        {
        }

        public override IMapConverterBuilder GetConverterBuilder()
        {
            return builder;
        }
    }
}
