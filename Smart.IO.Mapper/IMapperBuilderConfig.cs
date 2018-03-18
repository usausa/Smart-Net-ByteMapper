namespace Smart.IO.Mapper
{
    using System.Collections.Generic;

    using Smart.ComponentModel;

    public interface IMapperBuilderConfig
    {
        IComponentContainer ResolveComponents();

        Dictionary<string, object> ResolveParameters();
    }
}