namespace Smart.IO.Mapper.Expressions
{
    using System;
    using System.Collections.Generic;

    using Smart.ComponentModel;
    using Smart.IO.Mapper.Mappers;

    public class MapBuilder<T> : IMapTypeSyntax<T>, IMapping
    {
        public Type Type { get; }

        public string Profile { get; set; } // TODO

        public int Size { get; set; } // TODO

        public MapBuilder(Type type)
        {
            Type = type;
        }

        public IMapper[] CreateMappers(IComponentContainer components, IDictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }
    }
}
