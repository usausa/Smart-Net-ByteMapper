namespace Smart.IO.Mapper.Attributes
{
    using System;

    using Smart.IO.Mapper.Converters;

    public sealed class MapByteAttribute : AbstractPropertyAttribute
    {
        private static readonly IByteConverter ByteConverter = new ByteConverter();

        public MapByteAttribute(int offset)
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
                return ByteConverter;
            }

            return null;
        }
    }
}
