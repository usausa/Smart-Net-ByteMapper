namespace Smart.IO.Mapper.Attributes
{
    using System;

    using Smart.IO.Mapper.Converters;

    public sealed class ByteArrtibute : AbstractPropertyAttribute
    {
        private static readonly ByteConverter Converter = new ByteConverter();

        public ByteArrtibute(int offset)
            : base(offset)
        {
        }

        public override int CalcSize(Type type)
        {
            return 1;
        }

        public override IByteConverter CreateConverter(IMappingCreateContext context, Type type)
        {
            if (type == typeof(byte))
            {
                return Converter;
            }

            return null;
        }
    }
}
