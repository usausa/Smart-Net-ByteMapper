namespace Smart.IO.Mapper.Expressions
{
    using System;

    using Smart.ComponentModel;
    using Smart.IO.Mapper.Converters;
    using Smart.IO.Mapper.Helpers;

    public interface IMapBinarySyntax
    {
        IMapBinarySyntax Endian(Endian value);
    }

    internal sealed class MapBinaryExpression : IMemberMapFactory, IMapBinarySyntax
    {
        private Endian? endian;

        // TODO
        public IMapBinarySyntax Endian(Endian value)
        {
            endian = value;
            return this;
        }

        public int CalcSize(Type type)
        {
            if (type == typeof(int))
            {
                return 4;
            }

            if (type == typeof(long))
            {
                return 8;
            }

            if (type == typeof(short))
            {
                return 2;
            }

            return 0;
        }

        public IMapConverter CreateConverter(IComponentContainer components, IMappingParameter parameters, Type type)
        {
            throw new NotImplementedException();
        }
    }
}
