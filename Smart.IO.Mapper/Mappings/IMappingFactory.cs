namespace Smart.IO.Mapper.Mappings
{
    public interface IMappingFactory
    {
        IMapping Create(IMappingCreateContext context);
    }
}
