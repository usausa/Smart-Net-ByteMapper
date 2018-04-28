namespace Smart.AspNetCore.Formatters
{
    using System.Collections;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc.Formatters;

    using Smart.IO.ByteMapper;

    public class ByteMapperOutputFormatter : OutputFormatter
    {
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

            var profile = context.HttpContext.Items.TryGetValue(Consts.ProfileKey, out var value) ? value as string : null;

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