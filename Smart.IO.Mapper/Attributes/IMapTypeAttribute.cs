namespace Smart.IO.Mapper.Attributes
{
    using Smart.IO.Mapper.Builders;

    public interface IMapTypeAttribute
    {
        ITypeMapperBuilder GetTypeMapperBuilder();
    }
}
