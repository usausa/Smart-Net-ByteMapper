namespace Smart.IO.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Smart.Collections.Concurrent;
    using Smart.ComponentModel;

    public sealed class ByteMapper
    {
        private readonly IDictionary<string, object> parameters;

        private readonly IDictionary<MapKey, IMapping> mappings;

        private readonly ThreadsafeHashArrayMap<MapKey, object> cache = new ThreadsafeHashArrayMap<MapKey, object>();

        public IComponentContainer Components { get; }

        public ByteMapper(IByteMapperConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            Components = config.ResolveComponents();
            parameters = config.ResolveParameters();
            mappings = config.ResolveMappings()
                .ToDictionary(x => new MapKey(x.Type, x.Name ?? Names.Default), x => x);
        }

        public ITypeMapper<T> Create<T>()
        {
            return Create<T>(Names.Default);
        }

        public ITypeMapper<T> Create<T>(string profile)
        {
            var type = typeof(T);
            var key = new MapKey(type, profile ?? Names.Default);
            if (!cache.TryGetValue(key, out var mapper))
            {
                mapper = cache.AddIfNotExist(key, CreateMapper<T>);
            }

            return (ITypeMapper<T>)mapper;
        }

        private ITypeMapper<T> CreateMapper<T>(MapKey key)
        {
            if (!mappings.TryGetValue(key, out var mapping))
            {
                throw new ByteMapperException($"Mapper entry is not exist. type=[{key.Type.FullName}], name=[{key.Name}]");
            }

            return new TypeMapper<T>(
                mapping.Type,
                mapping.Size,
                mapping.CreateMappers(Components, parameters));
        }
    }
}
