namespace Smart.IO.ByteMapper.Builders
{
    using Smart.IO.ByteMapper.Mappers;

    public interface ITypeMapperBuilder
    {
        int Offset { get; set; }

        int CalcSize();

        IMapper CreateMapper(IBuilderContext context);
    }
}
