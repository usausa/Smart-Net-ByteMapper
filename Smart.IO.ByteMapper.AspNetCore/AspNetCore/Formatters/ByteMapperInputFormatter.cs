namespace Smart.AspNetCore.Formatters
{
    using System;
    using System.Buffers;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc.Formatters;

    using Smart.Collections.Concurrent;
    using Smart.IO.ByteMapper;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Ignore")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Ignore")]
    public class ByteMapperInputFormatter : InputFormatter
    {
        private static readonly Type SingleReaderType = typeof(SingleInputReader<>);

        private static readonly Type ArrayReaderType = typeof(ArrayInputReader<>);

        private static readonly Type ListReaderType = typeof(ListInputReader<>);

        private readonly ThreadsafeTypeHashArrayMap<IInputReader> readerCache = new();

        private readonly TypeProfileKeyCache<IInputReader> profiledReaderCache = new();

        private readonly ByteMapperFormatterConfig config;

        public ByteMapperInputFormatter(ByteMapperFormatterConfig config)
        {
            this.config = config;
            foreach (var mediaType in config.SupportedMediaTypes)
            {
                SupportedMediaTypes.Add(mediaType);
            }
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            var profile = context.HttpContext.Items.TryGetValue(Const.ProfileKey, out var value)
                ? value as string
                : null;
            var reader = String.IsNullOrEmpty(profile)
                ? GetReader(context.ModelType)
                : GetReader(context.ModelType, profile);

            var request = context.HttpContext.Request;

            var model = await reader.ReadAsync(request.Body, request.ContentLength).ConfigureAwait(false);

            return await InputFormatterResult.SuccessAsync(model).ConfigureAwait(false);
        }

        private IInputReader GetReader(Type type)
        {
            if (!readerCache.TryGetValue(type, out var reader))
            {
                reader = readerCache.AddIfNotExist(type, CreateReader);
            }

            return reader;
        }

        private IInputReader GetReader(Type type, string profile)
        {
            if (!profiledReaderCache.TryGetValue(type, profile, out var reader))
            {
                reader = profiledReaderCache.AddIfNotExist(type, profile, CreateReader);
            }

            return reader;
        }

        private IInputReader CreateReader(Type type)
        {
            var readerType = ResolveReaderType(type);
            return (IInputReader)Activator.CreateInstance(readerType, config, null);
        }

        private IInputReader CreateReader(Type type, string profile)
        {
            var readerType = ResolveReaderType(type);
            return (IInputReader)Activator.CreateInstance(readerType, config, profile);
        }

        private static Type ResolveReaderType(Type type)
        {
            if (type.IsArray)
            {
                return ArrayReaderType.MakeGenericType(type.GetElementType()!);
            }

            if (TypeHelper.IsEnumerableType(type))
            {
                return ListReaderType.MakeGenericType(type.GenericTypeArguments[0]);
            }

            return SingleReaderType.MakeGenericType(type);
        }

        private interface IInputReader
        {
            ValueTask<object> ReadAsync(Stream stream, long? length);
        }

        private sealed class SingleInputReader<T> : IInputReader
        {
            private readonly ITypeMapper<T> mapper;

            private readonly Func<T> factory;

            private readonly int bufferSize;

            public SingleInputReader(ByteMapperFormatterConfig config, string profile)
            {
                mapper = String.IsNullOrEmpty(profile) ? config.MapperFactory.Create<T>() : config.MapperFactory.Create<T>(profile);
                factory = config.DelegateFactory.CreateFactory<T>();
                bufferSize = mapper.Size;
            }

            public async ValueTask<object> ReadAsync(Stream stream, long? length)
            {
                var buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
                try
                {
                    if (await stream.ReadAsync(buffer, 0, bufferSize).ConfigureAwait(false) != bufferSize)
                    {
                        return default;
                    }

                    var target = factory();
                    mapper.FromByte(buffer, 0, target);
                    return target;
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }
            }
        }

        private sealed class ArrayInputReader<T> : IInputReader
        {
            private readonly ITypeMapper<T> mapper;

            private readonly Func<T> factory;

            private readonly int bufferSize;

            private readonly int readSize;

            public ArrayInputReader(ByteMapperFormatterConfig config, string profile)
            {
                mapper = String.IsNullOrEmpty(profile) ? config.MapperFactory.Create<T>() : config.MapperFactory.Create<T>(profile);
                factory = config.DelegateFactory.CreateFactory<T>();
                bufferSize = Math.Max(config.BufferSize, mapper.Size);
                readSize = (bufferSize / mapper.Size) * mapper.Size;
            }

            public async ValueTask<object> ReadAsync(Stream stream, long? length)
            {
                var buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
                try
                {
                    if (length.HasValue)
                    {
                        var array = new T[length.Value / mapper.Size];

                        var index = 0;
                        int read;
                        while ((read = await stream.ReadAsync(buffer, 0, readSize).ConfigureAwait(false)) > 0)
                        {
                            var limit = read - mapper.Size;
                            for (var pos = 0; pos <= limit; pos += mapper.Size)
                            {
                                var target = factory();
                                mapper.FromByte(buffer, pos, target);
                                array[index] = target;
                                index++;
                            }
                        }

                        return array;
                    }
                    else
                    {
                        var list = new List<T>();

                        int read;
                        while ((read = await stream.ReadAsync(buffer, 0, readSize).ConfigureAwait(false)) > 0)
                        {
                            var limit = read - mapper.Size;
                            for (var pos = 0; pos <= limit; pos += mapper.Size)
                            {
                                var target = factory();
                                mapper.FromByte(buffer, pos, target);
                                list.Add(target);
                            }
                        }

                        return list.ToArray();
                    }
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }
            }
        }

        private sealed class ListInputReader<T> : IInputReader
        {
            private readonly ITypeMapper<T> mapper;

            private readonly Func<T> factory;

            private readonly int bufferSize;

            private readonly int readSize;

            public ListInputReader(ByteMapperFormatterConfig config, string profile)
            {
                mapper = String.IsNullOrEmpty(profile) ? config.MapperFactory.Create<T>() : config.MapperFactory.Create<T>(profile);
                factory = config.DelegateFactory.CreateFactory<T>();
                bufferSize = Math.Max(config.BufferSize, mapper.Size);
                readSize = (bufferSize / mapper.Size) * mapper.Size;
            }

            public async ValueTask<object> ReadAsync(Stream stream, long? length)
            {
                var buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
                try
                {
                    var list = length.HasValue ? new List<T>((int)(length.Value / mapper.Size)) : new List<T>();

                    int read;
                    while ((read = await stream.ReadAsync(buffer, 0, readSize).ConfigureAwait(false)) > 0)
                    {
                        var limit = read - mapper.Size;
                        for (var pos = 0; pos <= limit; pos += mapper.Size)
                        {
                            var target = factory();
                            mapper.FromByte(buffer, pos, target);
                            list.Add(target);
                        }
                    }

                    return list;
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }
            }
        }

        // ------------------------------------------------------------
        // Diagnostics
        // ------------------------------------------------------------

        public DiagnosticsInfo Diagnostics
        {
            get
            {
                var cacheDiagnostics = readerCache.Diagnostics;
                var profiledCacheDiagnostics = profiledReaderCache.Diagnostics;

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
}