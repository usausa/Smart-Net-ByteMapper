namespace Smart.IO.ByteMapper.Builders
{
    using System;

    using Smart.IO.ByteMapper.Converters;

    public sealed class ByteConverterBuilder : IMapConverterBuilder
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
