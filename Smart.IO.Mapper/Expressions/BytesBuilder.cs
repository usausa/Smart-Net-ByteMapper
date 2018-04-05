namespace Smart.IO.Mapper.Expressions
{
    using System;

    using Smart.ComponentModel;
    using Smart.IO.Mapper.Converters;
    using Smart.IO.Mapper.Helpers;

    internal sealed class ByteMapBuilder : IPropertyMapFactory
    {
        private static readonly IByteConverter ByteConverter = new ByteConverter();

        public int CalcSize(Type type)
        {
            return 1;
        }

        public IByteConverter CreateConverter(IComponentContainer components, IMappingParameter parameters, Type type)
        {
            return ByteConverter;
        }
    }
}
