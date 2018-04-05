namespace Smart.IO.Mapper.Expressions
{
    using System;

    using Smart.ComponentModel;
    using Smart.IO.Mapper.Helpers;
    using Smart.IO.Mapper.Mappers;

    internal sealed class MapFillerExpression : ITypeMapFactory
    {
        private readonly int length;

        private readonly byte? filler;

        public MapFillerExpression(int length)
        {
            this.length = length;
        }

        public MapFillerExpression(int length, byte filler)
        {
            this.length = length;
            this.filler = filler;
        }

        public int CalcSize(Type type)
        {
            return length;
        }

        public IMapper CreateMapper(int offset, IComponentContainer components, IMappingParameter parameters, Type type)
        {
            return new FillMapper(
                offset,
                length,
                filler ?? parameters.GetParameter<byte>(Parameter.Filler));
        }
    }
}
