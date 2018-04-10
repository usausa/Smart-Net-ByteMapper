namespace Smart.IO.ByteMapper
{
    using System;
    using System.Collections.Generic;

    using Smart.ComponentModel;

    using Smart.IO.ByteMapper.Mappers;

    public interface IMapping
    {
        Type Type { get; }

        string Name { get; }

        int Size { get; }

        IMapper[] CreateMappers(IComponentContainer components, IDictionary<string, object> parameters);
    }
}
