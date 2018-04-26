namespace Smart.AspNetCore.Formatters
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc.Formatters;

    public class ByteMapperInputFormatter : InputFormatter
    {
        //private readonly MapperFactory mapperFactory;

        //public ByteMapperInputFormatter(MapperFactory mapperFactory)
        //{
        //    this.mapperFactory = mapperFactory;
        //}

        public override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            throw new NotImplementedException();
        }
    }
}