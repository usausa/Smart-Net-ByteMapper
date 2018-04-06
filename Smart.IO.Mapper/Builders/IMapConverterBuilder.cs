namespace Smart.IO.Mapper.Builders
{
    using System;

    using Smart.IO.Mapper.Converters;

    public interface IMapConverterBuilder
    {
        int CalcSize(IBuilderContext context, Type type);

        IMapConverter CreateConverter(IBuilderContext context, Type type);
    }
}
