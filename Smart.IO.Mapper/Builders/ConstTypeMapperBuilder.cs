namespace Smart.IO.Mapper.Builders
{
    using System;

    using Smart.IO.Mapper.Mappers;

    public class ConstTypeMapperBuilder : ITypeMapperBuilder
    {
        public int Offset { get; set; }

        public int CalcSize(IBuilderContext context, Type type)
        {
            throw new NotImplementedException();
        }

        public IMapper CreateMapper(IBuilderContext context, Type type)
        {
            throw new NotImplementedException();
        }
    }
}
