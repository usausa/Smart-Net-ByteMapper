namespace Smart.IO.Mapper.Builders
{
    using System;

    using Smart.IO.Mapper.Converters;

    public interface IMapConverterBuilder
    {
        int CalcSize(Type type);

        IMapConverter CreateConverter(IBuilderContext context, Type type);
    }
}
