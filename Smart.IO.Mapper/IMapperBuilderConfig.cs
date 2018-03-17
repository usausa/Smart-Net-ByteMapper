namespace Smart.IO.Mapper
{
    using System;
    using System.Collections.Generic;

    using Smart.ComponentModel;

    public interface IMapperBuilderConfig
    {
        IComponentContainer ResolveComponents();

        Dictionary<Type, object> ResolveParameters();
    }
}