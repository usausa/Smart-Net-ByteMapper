namespace Smart.IO.Mapper
{
    using Smart.IO.Mapper.Converters;

    // TODO MappingのFactory？
    public interface IMemberMapperFactory
    {
        IByteConverter Create(IMapperCreateContext context);
    }
}
