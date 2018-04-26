namespace Smart.AspNetCore.Formatters
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc.ApiExplorer;
    using Microsoft.AspNetCore.Mvc.Formatters;

    using Smart.IO.ByteMapper;

    public class ByteMapperInputFormatter : IInputFormatter, IApiRequestFormatMetadataProvider
    {
        private readonly MapperFactory mapperFactory;

        public ByteMapperInputFormatter(MapperFactory mapperFactory)
        {
            this.mapperFactory = mapperFactory;
        }

        public IReadOnlyList<string> GetSupportedContentTypes(string contentType, Type objectType)
        {
            throw new NotImplementedException();
        }

        public bool CanRead(InputFormatterContext context)
        {
            throw new System.NotImplementedException();
        }

        public Task<InputFormatterResult> ReadAsync(InputFormatterContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}