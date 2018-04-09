namespace Smart.IO.Mapper.Builders
{
    using System;

    using Smart.IO.Mapper.Converters;

    public class ByteConverterBuilder : IMapConverterBuilder
    {
        public int CalcSize(IBuilderContext context, Type type)
        {
            return 1;
        }

        public IMapConverter CreateConverter(IBuilderContext context, Type type)
        {
            if (type == typeof(byte))
            {
                return ByteConverter.Default;
            }

            return null;
        }
    }
}
