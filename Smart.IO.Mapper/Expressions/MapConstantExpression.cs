namespace Smart.IO.Mapper.Expressions
{
    using System;

    using Smart.ComponentModel;
    using Smart.IO.Mapper.Helpers;
    using Smart.IO.Mapper.Mappers;

    internal sealed class MapConstantExpression : ITypeMapFactory
    {
        private readonly byte[] content;

        public MapConstantExpression(byte[] content)
        {
            this.content = content;
        }

        public int CalcSize(Type type)
        {
            return content.Length;
        }

        public IMapper CreateMapper(int offset, IComponentContainer components, IMappingParameter parameters, Type type)
        {
            return new ConstantMapper(offset, content);
        }
    }
}
