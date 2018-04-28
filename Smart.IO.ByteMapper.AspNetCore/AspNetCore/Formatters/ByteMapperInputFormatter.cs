namespace Smart.AspNetCore.Formatters
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc.Formatters;

    using Smart.Collections.Concurrent;
    using Smart.IO.ByteMapper;
    using Smart.Reflection;

    public class ByteMapperInputFormatter : InputFormatter
    {
        private static readonly Type SingleReaderType = typeof(SingleInputReader<>);

        private static readonly Type ArrayReaderType = typeof(ArrayInputReader<>);

        private static readonly Type ListReaderType = typeof(ListInputReader<>);

        private static readonly Type[] RederConstructorTypes = { typeof(MapperFactory), typeof(string), typeof(Func<object>) };

        private readonly ThreadsafeHashArrayMap<MapperKey, IInputReader> defaultCahce = new ThreadsafeHashArrayMap<MapperKey, IInputReader>();

        private readonly MapperFactory mapperFactory;

        private readonly IDelegateFactory delegateFactory;

        public ByteMapperInputFormatter(MapperFactory mapperFactory)
            : this(mapperFactory, null)
        {
        }

        public ByteMapperInputFormatter(MapperFactory mapperFactory, IDelegateFactory delegateFactory)
        {
            this.mapperFactory = mapperFactory;
            this.delegateFactory = delegateFactory ?? DelegateFactory.Default;
        }

        public override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            var profile = context.HttpContext.Items.TryGetValue(Consts.ProfileKey, out var value) ? value as string : null;

            var reader = defaultCahce.AddIfNotExist(new MapperKey(context.ModelType, profile), CreateReader);

            var request = context.HttpContext.Response;

            var model = reader.Read(request.Body, request.ContentLength);

            return InputFormatterResult.SuccessAsync(model);
        }

        private IInputReader CreateReader(MapperKey key)
        {
            var readerType = ResolveReaderType(key.Type);

            var factory = delegateFactory.CreateFactory0(readerType.GetConstructor(RederConstructorTypes));

            return (IInputReader)Activator.CreateInstance(readerType, mapperFactory, key.Profile, factory);
        }

        private Type ResolveReaderType(Type type)
        {
            if (type.IsArray)
            {
                return ArrayReaderType.MakeGenericType(type.GetElementType());
            }

            if (TypeHelper.IsEnumerableType(type))
            {
                return ListReaderType.MakeGenericType(type.GenericTypeArguments[0]);
            }

            return SingleReaderType.MakeGenericType(type);
        }

        public interface IInputReader
        {
            object Read(Stream stream, long? length);
        }

        public sealed class SingleInputReader<T> : IInputReader
        {
            private readonly ITypeMapper<T> mapper;

            private readonly Func<object> factory;

            public SingleInputReader(MapperFactory mapperFactory, string profile, Func<object> factory)
            {
                mapper = mapperFactory.Create<T>(profile);
                this.factory = factory;
            }

            public object Read(Stream stream, long? length)
            {
                var buffer = new byte[mapper.Size];
                if (stream.Read(buffer, 0, buffer.Length) != buffer.Length)
                {
                    return default;
                }

                var target = (T)factory();
                mapper.FromByte(buffer, 0, target);
                return target;
            }
        }

        public sealed class ArrayInputReader<T> : IInputReader
        {
            private readonly ITypeMapper<T> mapper;

            private readonly Func<object> factory;

            public ArrayInputReader(MapperFactory mapperFactory, string profile, Func<object> factory)
            {
                mapper = mapperFactory.Create<T>(profile);
                this.factory = factory;
            }

            public object Read(Stream stream, long? length)
            {
                if (length.HasValue)
                {
                    var array = new T[length.Value / mapper.Size];

                    var index = 0;
                    var buffer = new byte[mapper.Size];
                    while (stream.Read(buffer, 0, buffer.Length) == buffer.Length)
                    {
                        var target = (T)factory();
                        mapper.FromByte(buffer, 0, target);
                        array[index] = target;
                        index++;
                    }

                    return array;
                }
                else
                {
                    var list = new List<T>();

                    var buffer = new byte[mapper.Size];
                    while (stream.Read(buffer, 0, buffer.Length) == buffer.Length)
                    {
                        var target = (T)factory();
                        mapper.FromByte(buffer, 0, target);
                        list.Add(target);
                    }

                    return list.ToArray();
                }
            }
        }

        public sealed class ListInputReader<T> : IInputReader
        {
            private readonly ITypeMapper<T> mapper;

            private readonly Func<object> factory;

            public ListInputReader(MapperFactory mapperFactory, string profile, Func<object> factory)
            {
                mapper = mapperFactory.Create<T>(profile);
                this.factory = factory;
            }

            public object Read(Stream stream, long? length)
            {
                var list = length.HasValue ? new List<T>((int)(length.Value / mapper.Size)) : new List<T>();

                var buffer = new byte[mapper.Size];
                while (stream.Read(buffer, 0, buffer.Length) == buffer.Length)
                {
                    var target = (T)factory();
                    mapper.FromByte(buffer, 0, target);
                    list.Add(target);
                }

                return list;
            }
        }
    }
}