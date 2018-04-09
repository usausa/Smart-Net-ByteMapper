namespace Smart.IO.Mapper.Builders
{
    using System;

    using Smart.IO.Mapper.Mappers;

    public class ConstantTypeMapperBuilder : ITypeMapperBuilder
    {
        public int Offset { get; set; }

        public byte[] Content { get; set; }

        public int CalcSize(IBuilderContext context, Type type)
        {
            return Content.Length;
        }

        public IMapper CreateMapper(IBuilderContext context, Type type)
        {
            return new ConstantMapper(Offset, Content);
        }
    }
}
