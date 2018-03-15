namespace Smart.IO.Mapper
{
    public interface IMemberMapperFactory
    {
        IMemberMapper Create(IMapperCreateContext context);
    }
}
