namespace Smart.IO.ByteMapper
{
    using System.Collections.Generic;

    using Smart.ComponentModel;

    public interface IMapperFactoryConfig
    {
        IComponentContainer ResolveComponents();

        IDictionary<string, object> ResolveParameters();

        IEnumerable<IMappingFactory> ResolveMappingFactories();
    }
}