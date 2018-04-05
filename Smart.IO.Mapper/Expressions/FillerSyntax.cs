namespace Smart.IO.Mapper.Expressions
{
    using System;

    using Smart.ComponentModel;
    using Smart.IO.Mapper.Helpers;
    using Smart.IO.Mapper.Mappers;

    public interface IFillerSyntax
    {
        // TODO
    }

    internal sealed class FillerMapBuilder : ITypeMapFactory, IFillerSyntax
    {
        public int CalcSize(Type type)
        {
            throw new NotImplementedException();
        }

        public IMapper CreateMapper(IComponentContainer components, IMappingParameter parameters, Type type)
        {
            throw new NotImplementedException();
        }

        // TODO
    }
}
