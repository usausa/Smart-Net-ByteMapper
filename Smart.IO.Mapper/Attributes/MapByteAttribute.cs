namespace Smart.IO.Mapper.Attributes
{
    using System;

    using Smart.ComponentModel;
    using Smart.IO.Mapper.Converters;
    using Smart.IO.Mapper.Helpers;

    public sealed class MapByteAttribute : AbstractMapPropertyAttribute
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

        public override IByteConverter CreateConverter(IComponentContainer components, IMappingParameter parameters, Type type)
        {
            if (type == typeof(byte))
            {
                return ByteConverter;
            }

            return null;
        }
    }
}
