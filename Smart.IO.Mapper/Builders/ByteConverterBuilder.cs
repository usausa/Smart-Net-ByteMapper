namespace Smart.IO.Mapper.Builders
{
    using System;

    using Smart.IO.Mapper.Converters;

    public class ByteConverterBuilder : IMapConverterBuilder
    {
        public static ByteConverterBuilder Default { get; } = new ByteConverterBuilder();

        public int CalcSize(Type type)
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
