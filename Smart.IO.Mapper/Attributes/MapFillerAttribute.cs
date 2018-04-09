namespace Smart.IO.Mapper.Attributes
{
    using System;

    using Smart.IO.Mapper.Builders;

    public sealed class MapFillerAttribute : AbstractTypeMapAttribute
    {
        private readonly FillerTypeMapperBuilder builder = new FillerTypeMapperBuilder();

        public byte Filler
        {
            get => throw new NotSupportedException();
            set => builder.Filler = value;
        }

        public MapFillerAttribute(int offset, int length)
        {
            builder.Offset = offset;
            builder.Length = length;
        }

        public override ITypeMapperBuilder GetTypeMapperBuilder()
        {
            return builder;
        }
    }
}
