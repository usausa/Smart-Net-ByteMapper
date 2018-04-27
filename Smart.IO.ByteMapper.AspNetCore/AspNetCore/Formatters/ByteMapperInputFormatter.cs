namespace Smart.AspNetCore.Formatters
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc.Formatters;

    using Smart.AspNetCore.Helpers;
    using Smart.Collections.Concurrent;
    using Smart.IO.ByteMapper;
    using Smart.Reflection;

    public class ByteMapperInputFormatter : InputFormatter
    {
        private readonly ThreadsafeTypeHashArrayMap<FactoryMetadata> factoryCache = new ThreadsafeTypeHashArrayMap<FactoryMetadata>();

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

            //var request = context.HttpContext.Request;
            var type = TypeHelper.GetEnumerableElementType(context.ModelType);
            var multiple = type != null;
            type = type ?? context.ModelType;

            var mapper = mapperFactory.Create(type, profile);
            if (multiple)
            {
                // TODO GenericなGeneratorを作る？

                // TODO IE対応？
                var factory = factoryCache.AddIfNotExist(type, CreateFactoryMetadata);

                var len = context.HttpContext.Request.ContentLength;

                //delegateFactory.CreateArrayAllocator()

                //var list = mapper.FromStreamMultiple(context.HttpContext.Request.Body, null)

                // TODO
                return InputFormatterResult.SuccessAsync(null);
            }
            else
            {
                // TODO
                return InputFormatterResult.SuccessAsync(null);
            }
        }

        private FactoryMetadata CreateFactoryMetadata(Type type)
        {
            return new FactoryMetadata(
                delegateFactory.CreateArrayAllocator(type),
                delegateFactory.CreateFactory0(type.GetConstructor(Type.EmptyTypes)));
        }

        private class FactoryMetadata
        {
            public Func<int, Array> ArrayFactory { get; }

            public Func<object> Factory { get; }

            public FactoryMetadata(Func<int, Array> arrayFactory, Func<object> factory)
            {
                ArrayFactory = arrayFactory;
                Factory = factory;
            }
        }
    }
}