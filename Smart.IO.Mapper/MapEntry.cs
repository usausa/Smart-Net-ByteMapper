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

        public Func<IMappingCreateContext, IMapping[]> Factory { get; }

        public MapEntry(Type targetType, int size, IDictionary<string, object> parameters, Func<IMappingCreateContext, IMapping[]> factory)
        {
            TargetType = targetType;
            Size = size;
            Parameters = parameters;
            Factory = factory;
        }
    }
}
