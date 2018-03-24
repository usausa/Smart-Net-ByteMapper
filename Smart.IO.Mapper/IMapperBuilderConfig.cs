namespace Smart.IO.Mapper
{
    using System.Collections.Generic;

    using Smart.ComponentModel;

    public interface IMapperBuilderConfig
    {
        IComponentContainer ResolveComponents();

        IDictionary<string, object> ResolveParameters();

        IDictionary<MappingKey, MappingEntry> ResolveMappings();
    }
}