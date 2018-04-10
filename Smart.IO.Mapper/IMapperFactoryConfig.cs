namespace Smart.IO.Mapper
{
    using System.Collections.Generic;

    using Smart.ComponentModel;

    public interface IMapperFactoryConfig
    {
        IComponentContainer ResolveComponents();

        IDictionary<string, object> ResolveParameters();

        IEnumerable<IMapping> ResolveMappings();
    }
}