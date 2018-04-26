namespace Smart.AspNetCore.Formatters
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc.ApiExplorer;
    using Microsoft.AspNetCore.Mvc.Formatters;

    using Smart.IO.ByteMapper;

    public class ByteMapperOutputFormatter : IOutputFormatter, IApiResponseTypeMetadataProvider
    {
        private const string ContentType = "application/x-record";

        // TODO DI?
        private readonly MapperFactory mapperFactory;

        public ByteMapperOutputFormatter(MapperFactory mapperFactory)
        {
            this.mapperFactory = mapperFactory;
        }

        public IReadOnlyList<string> GetSupportedContentTypes(string contentType, Type objectType)
        {
            return new[] { ContentType };
        }

        public bool CanWriteResult(OutputFormatterCanWriteContext context)
        {
            // TODO strategy, mapper reuse?
            return true;
        }

        public async Task WriteAsync(OutputFormatterWriteContext context)
        {
            // TODO
            if (context.Object == null)
            {
                return;
            }

            // TODO single or multiple element type / generic argments 0
            //if (context.ObjectType == typeof(object))

            // TODO get maper

            // TODO Write with loop
            var streamWriter = new StreamWriter(context.HttpContext.Response.Body);
            await streamWriter.WriteAsync("test");
            await streamWriter.FlushAsync();
        }
    }
}