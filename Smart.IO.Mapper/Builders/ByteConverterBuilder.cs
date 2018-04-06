namespace Smart.IO.Mapper.Builders
{
    using System;

    using Smart.IO.Mapper.Converters;

    public class ByteConverterBuilder : IMapConverterBuilder
    {
        public int CalcSize(IBuilderContext context, Type type)
        {
            throw new NotImplementedException();
        }

        public IMapConverter CreateConverter(IBuilderContext context, Type type)
        {
            throw new NotImplementedException();
        }
    }
}
