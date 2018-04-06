namespace Smart.IO.Mapper.Attributes
{
    using System;

    using Smart.ComponentModel;
    using Smart.IO.Mapper.Converters;
    using Smart.IO.Mapper.Helpers;

    public sealed class MapByteAttribute : AbstractMapMemberAttribute
    {
        private static readonly IMapConverter MapConverter = new MapConverter();

        public MapByteAttribute(int offset)
            : base(offset)
        {
        }

        public override int CalcSize(Type type)
        {
            return 1;
        }

        public override IMapConverter CreateConverter(IComponentContainer components, IMappingParameter parameters, Type type)
        {
            if (type == typeof(byte))
            {
                return MapConverter;
            }

            return null;
        }
    }
}
