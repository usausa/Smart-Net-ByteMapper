namespace Smart.IO.ByteMapper.Builders
{
    using System;

    using Smart.IO.ByteMapper.Mappers;

    public sealed class ConstantTypeMapperBuilder : ITypeMapperBuilder
    {
        public int Offset { get; set; }

        public byte[] Content { get; set; }

        public int CalcSize(Type type)
        {
            return Content.Length;
        }

        public IMapper CreateMapper(IBuilderContext context, Type type)
        {
            return new ConstantMapper(Offset, Content);
        }
    }
}
