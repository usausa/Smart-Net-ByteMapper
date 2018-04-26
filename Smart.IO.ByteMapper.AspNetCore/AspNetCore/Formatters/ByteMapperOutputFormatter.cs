namespace Smart.AspNetCore.Formatters
{
    using System.Collections;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc.Formatters;

    using Smart.IO.ByteMapper;

    public class ByteMapperOutputFormatter : OutputFormatter
    {
        private readonly MapperFactory mapperFactory;

        private readonly string profile;

        // TODO buffersize

        public ByteMapperOutputFormatter(MapperFactory mapperFactory)
            : this(mapperFactory, null)
        {
        }

        public ByteMapperOutputFormatter(MapperFactory mapperFactory, string profile)
        {
            this.mapperFactory = mapperFactory;
            this.profile = profile;
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
        {
            if (context.Object == null)
            {
                return;
            }

            var type = TypeHelper.GetEnumerableElementType(context.ObjectType);
            var multiple = type != null;
            type = type ?? context.ObjectType;

            var mapper = mapperFactory.Create(type, profile);
            var stream = context.HttpContext.Response.Body;

            if (multiple)
            {
                await mapper.ToStreamMultipleAsync(stream, (IEnumerable)context.Object);
            }
            else
            {
                await mapper.ToStreamAsync(stream, context.Object);
            }

            await stream.FlushAsync();
        }
    }
}