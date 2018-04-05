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

    public sealed class FillerMapBuilder : ITypeMapFactory, IFillerSyntax
    {
        public int Offset { get; set; } // TODO

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

    public static class FillerMapExtensions
    {
        // TODO
    }
}
