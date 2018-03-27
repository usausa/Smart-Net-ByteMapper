namespace Smart.IO.Mapper
{
    // TODO MappingのFactory？
    public interface IMemberMapperFactory
    {
        IMemberMapper Create(IMapperCreateContext context);
    }
}
