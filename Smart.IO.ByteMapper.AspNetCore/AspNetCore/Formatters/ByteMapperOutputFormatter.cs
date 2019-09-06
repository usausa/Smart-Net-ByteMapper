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
    public class ByteMapperOutputFormatter : OutputFormatter
    {
        private static readonly Type SingleWriterType = typeof(SingleOutputWriter<>);

        private static readonly Type EnumerableWriterType = typeof(EnumerableOutputWriter<>);

        private readonly ThreadsafeTypeHashArrayMap<IOutputWriter> writerCache = new ThreadsafeTypeHashArrayMap<IOutputWriter>();

        private readonly TypeProfileKeyCache<IOutputWriter> profiledWriterCache = new TypeProfileKeyCache<IOutputWriter>();

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

            writer.Write(stream, context.Object);

            await stream.FlushAsync().ConfigureAwait(false);
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
            var writerType = ResolveWriterType(type);
            return (IOutputWriter)Activator.CreateInstance(writerType, config, null);
        }

        private IOutputWriter CreateWriter(Type type, string profile)
        {
            var writerType = ResolveWriterType(type);
            return (IOutputWriter)Activator.CreateInstance(writerType, config, profile);
        }

        private static Type ResolveWriterType(Type type)
        {
            var elementType = TypeHelper.GetEnumerableElementType(type);
            if (elementType is null)
            {
                return SingleWriterType.MakeGenericType(type);
            }

            return EnumerableWriterType.MakeGenericType(elementType);
        }

        private interface IOutputWriter
        {
            void Write(Stream stream, object model);
        }

        private sealed class SingleOutputWriter<T> : IOutputWriter
        {
            private readonly ITypeMapper<T> mapper;

            private readonly int bufferSize;

            public SingleOutputWriter(ByteMapperFormatterConfig config, string profile)
            {
                mapper = String.IsNullOrEmpty(profile) ? config.MapperFactory.Create<T>() : config.MapperFactory.Create<T>(profile);
                bufferSize = mapper.Size;
            }

            public void Write(Stream stream, object model)
            {
                var buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
                try
                {
                    mapper.ToByte(buffer, 0, (T)model);
                    stream.Write(buffer, 0, bufferSize);
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }
            }
        }

        private sealed class EnumerableOutputWriter<T> : IOutputWriter
        {
            private readonly ITypeMapper<T> mapper;

            private readonly int bufferSize;

            public EnumerableOutputWriter(ByteMapperFormatterConfig config, string profile)
            {
                mapper = String.IsNullOrEmpty(profile) ? config.MapperFactory.Create<T>() : config.MapperFactory.Create<T>(profile);
                bufferSize = Math.Max(config.BufferSize, mapper.Size);
            }

            public void Write(Stream stream, object model)
            {
                var buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
                try
                {
                    var pos = 0;
                    var limit = buffer.Length - mapper.Size;
                    foreach (var target in (IEnumerable<T>)model)
                    {
                        mapper.ToByte(buffer, pos, target);

                        pos += mapper.Size;
                        if (pos > limit)
                        {
                            stream.Write(buffer, 0, pos);
                            pos = 0;
                        }
                    }

                    if (pos > 0)
                    {
                        stream.Write(buffer, 0, pos);
                    }
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }
            }
        }
    }
}