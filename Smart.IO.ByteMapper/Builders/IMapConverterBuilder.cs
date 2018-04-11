namespace Smart.IO.ByteMapper.Builders
{
    using System;

    using Smart.IO.ByteMapper.Converters;

    public interface IMapConverterBuilder
    {
        bool Match(Type type);

        int CalcSize(Type type);

        IMapConverter CreateConverter(IBuilderContext context, Type type);
    }
}
