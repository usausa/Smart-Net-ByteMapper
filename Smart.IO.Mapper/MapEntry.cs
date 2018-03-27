namespace Smart.IO.Mapper
{
    using System;
    using System.Collections.Generic;

    public class MapEntry
    {
        public Type TargetType { get; }

        public int Size { get; }

        public IDictionary<string, object> Parameters { get; }

        public IMemberMapperFactory[] Factories { get; }

        public MapEntry(Type targetType, int size, IDictionary<string, object> parameters, IMemberMapperFactory[] factories)
        {
            TargetType = targetType;
            Size = size;
            Parameters = parameters;
            Factories = factories;
        }
    }
}
