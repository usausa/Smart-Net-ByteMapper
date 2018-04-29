namespace Smart.AspNetCore.Formatters
{
    using System;
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

        private readonly MapperFactory mapperFactory;

        public ByteMapperOutputFormatter(MapperFactory mapperFactory)
        {
            this.mapperFactory = mapperFactory;
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

            await writer.WriteAsync(stream, context.Object);

            await stream.FlushAsync();
        }

        private IOutputWriter CreateWriter(MapperKey key)
        {
            var writerType = ResolveWriterType(key.Type);

            return (IOutputWriter)Activator.CreateInstance(writerType, mapperFactory, key.Profile);
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
            Task WriteAsync(Stream stream, object model);
        }

        private sealed class SingleOutputWriter<T> : IOutputWriter
        {
            private readonly ITypeMapper<T> mapper;

            public SingleOutputWriter(MapperFactory mapperFactory, string profile)
            {
                mapper = mapperFactory.Create<T>(profile);
            }

            public async Task WriteAsync(Stream stream, object model)
            {
                var buffer = new byte[mapper.Size];
                mapper.ToByte(buffer, 0, (T)model);
                await stream.WriteAsync(buffer, 0, buffer.Length);
            }
        }

        private sealed class EnumerableOutputWriter<T> : IOutputWriter
        {
            private readonly ITypeMapper<T> mapper;

            public EnumerableOutputWriter(MapperFactory mapperFactory, string profile)
            {
                mapper = mapperFactory.Create<T>(profile);
            }

            public async Task WriteAsync(Stream stream, object model)
            {
                var buffer = new byte[mapper.Size];
                foreach (var target in (IEnumerable<T>)model)
                {
                    mapper.ToByte(buffer, 0, target);
                    await stream.WriteAsync(buffer, 0, buffer.Length);
                }
            }
        }
    }
}