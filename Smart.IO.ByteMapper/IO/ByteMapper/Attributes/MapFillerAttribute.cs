namespace Smart.IO.ByteMapper.Attributes
{
    using System;

    using Smart.IO.ByteMapper.Builders;

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
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            builder.Offset = offset;
            builder.Length = length;
        }

        public override ITypeMapperBuilder GetTypeMapperBuilder() => builder;
    }
}
