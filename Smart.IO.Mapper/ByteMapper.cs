namespace Smart.IO.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Smart.Collections.Concurrent;
    using Smart.ComponentModel;
    using Smart.IO.Mapper.Mappings;

    public class ByteMapper
    {
        private readonly IDictionary<string, object> parameters;

        private readonly IDictionary<MapKey, MapEntry> entries;

        private readonly ThreadsafeTypeHashArrayMap<object> cache = new ThreadsafeTypeHashArrayMap<object>();

        public IComponentContainer Components { get; }

        public ByteMapper(IByteMapperConfig config)
        {
            Components = config.ResolveComponents();
            parameters = config.ResolveParameters();
            entries = config.ResolveEntries();
        }

        public ITypeMapper<T> Create<T>()
        {
            return Create<T>(Profile.Default);
        }

        public ITypeMapper<T> Create<T>(string profile)
        {
            var targetType = typeof(T);

            if (cache.TryGetValue(targetType, out var mapper))
            {
                mapper = cache.AddIfNotExist(targetType, x => CreateMapper<T>(x, profile));
            }

            return (ITypeMapper<T>)mapper;
        }

        private ITypeMapper<T> CreateMapper<T>(Type type, string profile)
        {
            if (!entries.TryGetValue(new MapKey(type, profile), out var entry))
            {
                throw new InvalidOperationException($"Mapper entry is not exist. type=[{type.FullName}], profile=[{profile}]");
            }

            return new TypeMapper<T>(
                type,
                entry.Size,
                entry.Factory(new MappingCreateContext(parameters, entry.Parameters, Components)));
        }
    }
}
