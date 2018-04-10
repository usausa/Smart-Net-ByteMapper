namespace Smart.IO.ByteMapper.Builders
{
    using System;

    using Smart.IO.ByteMapper.Mappers;

    public interface ITypeMapperBuilder
    {
        int Offset { get; set; }

        int CalcSize(Type type);

        IMapper CreateMapper(IBuilderContext context, Type type);
    }
}
