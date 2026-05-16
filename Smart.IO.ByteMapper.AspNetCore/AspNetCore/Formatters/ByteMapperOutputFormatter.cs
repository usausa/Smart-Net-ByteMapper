namespace Smart.AspNetCore.Formatters;

using System.Buffers;

using Microsoft.AspNetCore.Mvc.Formatters;

using Smart.Collections.Concurrent;
using Smart.IO.ByteMapper;

#pragma warning disable CA1062
public sealed class ByteMapperOutputFormatter : OutputFormatter
{
    private readonly ThreadsafeTypeHashArrayMap<IOutputWriter> writerCache = new();

    private readonly TypeProfileKeyCache<IOutputWriter> profiledWriterCache = new();

    private readonly ByteMapperFormatterConfig config;

    public ByteMapperOutputFormatter(ByteMapperFormatterConfig config)
    {
        this.config = config;
        foreach (var mediaType in config.SupportedMediaTypes)
        {
            SupportedMediaTypes.Add(mediaType);
        }
    }

    public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
    {
        if (context.Object is null)
        {
            return;
        }

        var profile = context.HttpContext.Items.TryGetValue(Const.ProfileKey, out var value)
            ? value as string
            : null;
        var writer = String.IsNullOrEmpty(profile)
            ? GetWriter(context.ObjectType)
            : GetWriter(context.ObjectType, profile);

        var stream = context.HttpContext.Response.Body;
        var cancellationToken = context.HttpContext.RequestAborted;

        await writer.WriteAsync(stream, context.Object, cancellationToken).ConfigureAwait(false);

        await stream.FlushAsync(cancellationToken).ConfigureAwait(false);
    }

    private IOutputWriter GetWriter(Type type)
    {
        if (!writerCache.TryGetValue(type, out var writer))
        {
            writer = writerCache.AddIfNotExist(type, CreateWriter);
        }

        return writer;
    }

    private IOutputWriter GetWriter(Type type, string profile)
    {
        if (!profiledWriterCache.TryGetValue(type, profile, out var writer))
        {
            writer = profiledWriterCache.AddIfNotExist(type, profile, CreateWriter);
        }

        return writer;
    }

    private IOutputWriter CreateWriter(Type type)
    {
        return ResolveWriterFactory(type)(config, null);
    }

    private IOutputWriter CreateWriter(Type type, string profile)
    {
        return ResolveWriterFactory(type)(config, profile);
    }

    private static Func<ByteMapperFormatterConfig, string, IOutputWriter> ResolveWriterFactory(Type type)
    {
        var elementType = TypeHelper.GetEnumerableElementType(type);
        var writerType = elementType is null
            ? typeof(SingleOutputWriter<>).MakeGenericType(type)
            : typeof(EnumerableOutputWriter<>).MakeGenericType(elementType);
        return WriterFactoryCache.GetOrCreate(writerType);
    }

    private static class WriterFactoryCache
    {
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<Type, Func<ByteMapperFormatterConfig, string, IOutputWriter>> Cache = new();

        internal static Func<ByteMapperFormatterConfig, string, IOutputWriter> GetOrCreate(Type writerType)
        {
            return Cache.GetOrAdd(writerType, static t =>
            {
                var ctor = t.GetConstructor([typeof(ByteMapperFormatterConfig), typeof(string)])!;
                return (cfg, profile) => (IOutputWriter)ctor.Invoke([cfg, profile]);
            });
        }
    }

    private interface IOutputWriter
    {
        ValueTask WriteAsync(Stream stream, object model, CancellationToken cancellationToken);
    }

#pragma warning disable CA1812
    private sealed class SingleOutputWriter<T> : IOutputWriter
    {
        private readonly ITypeMapper<T> mapper;

        private readonly int bufferSize;

        public SingleOutputWriter(ByteMapperFormatterConfig config, string profile)
        {
            mapper = String.IsNullOrEmpty(profile) ? config.MapperFactory.Create<T>() : config.MapperFactory.Create<T>(profile);
            bufferSize = mapper.Size;
        }

#pragma warning disable CA1835
        public async ValueTask WriteAsync(Stream stream, object model, CancellationToken cancellationToken)
        {
            var buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
            try
            {
                mapper.ToByte(buffer.AsSpan(0, bufferSize), (T)model);
                await stream.WriteAsync(buffer.AsMemory(0, bufferSize), cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }
#pragma warning restore CA1835
    }
#pragma warning restore CA1812

#pragma warning disable CA1812
    private sealed class EnumerableOutputWriter<T> : IOutputWriter
    {
        private readonly ITypeMapper<T> mapper;

        private readonly int bufferSize;

        public EnumerableOutputWriter(ByteMapperFormatterConfig config, string profile)
        {
            mapper = String.IsNullOrEmpty(profile) ? config.MapperFactory.Create<T>() : config.MapperFactory.Create<T>(profile);
            bufferSize = Math.Max(config.BufferSize, mapper.Size);
        }

#pragma warning disable CA1835
        public async ValueTask WriteAsync(Stream stream, object model, CancellationToken cancellationToken)
        {
            var buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
            try
            {
                var pos = 0;
                var limit = buffer.Length - mapper.Size;
                foreach (var target in (IEnumerable<T>)model)
                {
                    mapper.ToByte(buffer.AsSpan(pos, mapper.Size), target);

                    pos += mapper.Size;
                    if (pos > limit)
                    {
                        await stream.WriteAsync(buffer.AsMemory(0, pos), cancellationToken).ConfigureAwait(false);
                        pos = 0;
                    }
                }

                if (pos > 0)
                {
                    await stream.WriteAsync(buffer.AsMemory(0, pos), cancellationToken).ConfigureAwait(false);
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }
#pragma warning restore CA1835
    }
#pragma warning restore CA1812

    // ------------------------------------------------------------
    // Diagnostics
    // ------------------------------------------------------------

    public DiagnosticsInfo Diagnostics
    {
        get
        {
            var cacheDiagnostics = writerCache.Diagnostics;
            var profiledCacheDiagnostics = profiledWriterCache.Diagnostics;

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
