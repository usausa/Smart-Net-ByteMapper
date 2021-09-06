namespace Smart.IO.ByteMapper
{
    using System.Collections.Generic;

    using Smart.ComponentModel;

    public interface IMapperFactoryConfig
    {
        ComponentContainer ResolveComponents();

        IDictionary<string, object> ResolveParameters();

        IEnumerable<IMappingFactory> ResolveMappingFactories();
    }
}
