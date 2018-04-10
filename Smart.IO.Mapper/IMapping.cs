namespace Smart.IO.Mapper
{
    using System;
    using System.Collections.Generic;

    using Smart.ComponentModel;

    using Smart.IO.Mapper.Mappers;

    public interface IMapping
    {
        Type Type { get; }

        string Name { get; }

        int Size { get; }

        IMapper[] CreateMappers(IComponentContainer components, IDictionary<string, object> parameters);
    }
}
