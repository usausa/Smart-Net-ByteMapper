namespace Smart.IO.Mapper.Expressions
{
    using System;

    using Smart.ComponentModel;
    using Smart.IO.Mapper.Helpers;
    using Smart.IO.Mapper.Mappers;

    internal sealed class FillerMapBuilder : ITypeMapFactory
    {
        private readonly int length;

        private readonly byte? filler;

        public FillerMapBuilder(int length)
        {
            this.length = length;
        }

        public FillerMapBuilder(int length, byte filler)
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
