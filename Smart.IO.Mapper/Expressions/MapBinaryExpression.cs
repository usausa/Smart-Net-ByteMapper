namespace Smart.IO.Mapper.Expressions
{
    using System;

    using Smart.ComponentModel;
    using Smart.IO.Mapper.Converters;
    using Smart.IO.Mapper.Helpers;

    internal sealed class MapBinaryExpression : IMemberMapFactory
    {
        private readonly Endian? endian;

        public MapBinaryExpression(Endian? endian)
        {
            this.endian = endian;
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

        public IByteConverter CreateConverter(IComponentContainer components, IMappingParameter parameters, Type type)
        {
            throw new NotImplementedException();
        }

        // TODO
    }
}
