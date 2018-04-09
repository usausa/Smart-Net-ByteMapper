namespace Smart.IO.Mapper.Builders
{
    using System;

    using Smart.IO.Mapper.Mappers;

    public interface ITypeMapperBuilder
    {
        int Offset { get; set; }

        int CalcSize(Type type);

        IMapper CreateMapper(IBuilderContext context, Type type);
    }
}
