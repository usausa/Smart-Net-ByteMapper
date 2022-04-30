namespace Smart.IO.ByteMapper;

using Smart.ComponentModel;

public interface IMapperFactoryConfig
{
    ComponentContainer ResolveComponents();

    IDictionary<string, object> ResolveParameters();

    IEnumerable<IMappingFactory> ResolveMappingFactories();
}
