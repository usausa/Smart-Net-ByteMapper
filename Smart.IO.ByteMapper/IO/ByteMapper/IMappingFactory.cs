namespace Smart.IO.ByteMapper
{
    using System;
    using System.Collections.Generic;

    using Smart.ComponentModel;

    public interface IMappingFactory
    {
        Type Type { get; }

        string Name { get; }

        IMapping Create(ComponentContainer components, IDictionary<string, object> parameters);
    }
}
