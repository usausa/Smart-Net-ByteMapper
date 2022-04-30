namespace Smart.IO.ByteMapper;

public interface IMapperProfile
{
    IEnumerable<IMappingFactory> ResolveMappingFactories();
}
