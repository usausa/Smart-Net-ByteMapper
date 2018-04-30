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

    public class ByteMapperOutputFormatter : OutputFormatter
    {
        private static readonly Type SingleWriterType = typeof(SingleOutputWriter<>);

        private static readonly Type EnumerableWriterType = typeof(EnumerableOutputWriter<>);

        private readonly ThreadsafeHashArrayMap<MapperKey, IOutputWriter> writerCache = new ThreadsafeHashArrayMap<MapperKey, IOutputWriter>();

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
            if (context.Object == null)
            {
                return;
            }

            var profile = context.HttpContext.Items.TryGetValue(Consts.ProfileKey, out var value) ? value as string : Profile.Default;

            var writer = writerCache.AddIfNotExist(new MapperKey(context.ObjectType, profile), CreateWriter);

            var stream = context.HttpContext.Response.Body;

            writer.Write(stream, context.Object);

            await stream.FlushAsync();
        }

        private IOutputWriter CreateWriter(MapperKey key)
        {
            var writerType = ResolveWriterType(key.Type);

            return (IOutputWriter)Activator.CreateInstance(writerType, config, key.Profile);
        }

        private Type ResolveWriterType(Type type)
        {
            var elementType = TypeHelper.GetEnumerableElementType(type);
            if (elementType != null)
            {
                return EnumerableWriterType.MakeGenericType(elementType);
            }

            return SingleWriterType.MakeGenericType(type);
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
                mapper = config.MapperFactory.Create<T>(profile);
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
                mapper = config.MapperFactory.Create<T>(profile);
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