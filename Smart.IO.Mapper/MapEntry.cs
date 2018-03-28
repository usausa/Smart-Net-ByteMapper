namespace Smart.IO.Mapper
{
    using System;
    using System.Collections.Generic;

    using Smart.IO.Mapper.Mappings;

    public class MapEntry
    {
        public Type TargetType { get; }

        public int Size { get; }

        public IDictionary<string, object> Parameters { get; }

        public IMappingFactory[] Factories { get; }

        public MapEntry(Type targetType, int size, IDictionary<string, object> parameters, IMappingFactory[] factories)
        {
            TargetType = targetType;
            Size = size;
            Parameters = parameters;
            Factories = factories;
        }
    }
}
