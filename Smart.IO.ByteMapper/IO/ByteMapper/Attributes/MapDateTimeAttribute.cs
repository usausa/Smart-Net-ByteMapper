namespace Smart.IO.ByteMapper.Attributes
{
    using System;

    using Smart.IO.ByteMapper.Builders;

    public sealed class MapDateTimeAttribute : AbstractMemberMapAttribute
    {
        private readonly DateTimeConverterBuilder builder = new DateTimeConverterBuilder();

        public byte Filler
        {
            get => throw new NotSupportedException();
            set => builder.Filler = value;
        }

        public MapDateTimeAttribute(int offset, string format)
            : base(offset)
        {
            builder.Format = format;
        }

        public override IMapConverterBuilder GetConverterBuilder()
        {
            return builder;
        }
    }
}
