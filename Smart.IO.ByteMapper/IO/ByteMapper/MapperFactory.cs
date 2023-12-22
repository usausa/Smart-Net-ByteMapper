namespace Smart.IO.ByteMapper;

using System.Reflection;

using Smart.Collections.Concurrent;
using Smart.ComponentModel;

public sealed class MapperFactory
{
    private readonly IDictionary<string, object> parameters;

    private readonly Dictionary<Type, IMappingFactory> mappingFactories;

    private readonly Dictionary<TypeProfile, IMappingFactory> profiledMappingFactories;

    private readonly ThreadsafeTypeHashArrayMap<object> cache = new();

    private readonly TypeProfileKeyCache<object> profiledCache = new();

    public ComponentContainer Components { get; }

    public MapperFactory(IMapperFactoryConfig config)
    {
        if (config is null)
        {
            throw new ArgumentNullException(nameof(config));
        }

        Components = config.ResolveComponents();
        parameters = config.ResolveParameters();
        mappingFactories = config.ResolveMappingFactories()
            .Where(static x => String.IsNullOrEmpty(x.Name))
            .ToDictionary(static x => x.Type, static x => x);
        profiledMappingFactories = config.ResolveMappingFactories()
            .Where(static x => !String.IsNullOrEmpty(x.Name))
            .ToDictionary(static x => new TypeProfile(x.Type, x.Name), static x => x);
    }

    // ------------------------------------------------------------
    // Create
    // ------------------------------------------------------------

    public ITypeMapper Create(Type type)
    {
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        var method = GetType().GetMethod(nameof(CreateInternal), BindingFlags.Instance | BindingFlags.NonPublic);
        var genericMethod = method.MakeGenericMethod(type);
        return (ITypeMapper)genericMethod.Invoke(this, null);
    }

    public ITypeMapper Create(Type type, string profile)
    {
        if (profile is null)
        {
            return Create(type);
        }

        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        var method = GetType().GetMethod(nameof(CreateInternalWithProfile), BindingFlags.Instance | BindingFlags.NonPublic);
        var genericMethod = method.MakeGenericMethod(type);
        return (ITypeMapper)genericMethod.Invoke(this, [profile]);
    }

    public ITypeMapper<T> Create<T>()
    {
        return CreateInternal<T>();
    }

    public ITypeMapper<T> Create<T>(string profile)
    {
        if (profile is null)
        {
            return CreateInternal<T>();
        }

        return CreateInternalWithProfile<T>(profile);
    }

    private ITypeMapper<T> CreateInternal<T>()
    {
        var key = typeof(T);
        if (!cache.TryGetValue(key, out var mapper))
        {
            mapper = cache.AddIfNotExist(key, CreateMapper<T>);
        }

        return (ITypeMapper<T>)mapper;
    }

    private ITypeMapper<T> CreateInternalWithProfile<T>(string profile)
    {
        var type = typeof(T);
        if (!profiledCache.TryGetValue(type, profile, out var mapper))
        {
            mapper = profiledCache.AddIfNotExist(type, profile, CreateMapper<T>);
        }

        return (ITypeMapper<T>)mapper;
    }

    private TypeMapper<T> CreateMapper<T>(Type type)
    {
        if (!mappingFactories.TryGetValue(type, out var mappingFactory))
        {
            throw new ByteMapperException($"Mapper entry is not exist. type=[{type.FullName}]");
        }

        var mapping = mappingFactory.Create(Components, parameters);
        return new TypeMapper<T>(mapping.Type, mapping.Size, mapping.Filler, mapping.Mappers);
    }

    private TypeMapper<T> CreateMapper<T>(Type type, string profile)
    {
        var key = new TypeProfile(type, profile);
        if (!profiledMappingFactories.TryGetValue(key, out var mappingFactory))
        {
            throw new ByteMapperException($"Mapper entry is not exist. type=[{key.Type.FullName}], profile=[{key.Profile}]");
        }

        var mapping = mappingFactory.Create(Components, parameters);
        return new TypeMapper<T>(mapping.Type, mapping.Size, mapping.Filler, mapping.Mappers);
    }

    // ------------------------------------------------------------
    // Diagnostics
    // ------------------------------------------------------------

    public DiagnosticsInfo Diagnostics
    {
        get
        {
            var cacheDiagnostics = cache.Diagnostics;
            var profiledCacheDiagnostics = profiledCache.Diagnostics;

            return new DiagnosticsInfo(
                cacheDiagnostics.Count,
                cacheDiagnostics.Width,
                cacheDiagnostics.Depth,
                profiledCacheDiagnostics.Count,
                profiledCacheDiagnostics.Width,
                profiledCacheDiagnostics.Depth);
        }
    }

    public sealed class DiagnosticsInfo
    {
        public int CacheCount { get; }

        public int CacheWidth { get; }

        public int CacheDepth { get; }

        public int ProfiledCacheCount { get; }

        public int ProfiledCacheWidth { get; }

        public int ProfiledCacheDepth { get; }

        public DiagnosticsInfo(
            int cacheCount,
            int cacheWidth,
            int cacheDepth,
            int profiledCacheCount,
            int profiledCacheWidth,
            int profiledCacheDepth)
        {
            CacheCount = cacheCount;
            CacheWidth = cacheWidth;
            CacheDepth = cacheDepth;
            ProfiledCacheCount = profiledCacheCount;
            ProfiledCacheWidth = profiledCacheWidth;
            ProfiledCacheDepth = profiledCacheDepth;
        }
    }
}
