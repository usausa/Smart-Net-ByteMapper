namespace Smart.IO.Mapper.Expressions
{
    using System;

    using Smart.ComponentModel;
    using Smart.IO.Mapper.Converters;
    using Smart.IO.Mapper.Helpers;

    public interface INumberSyntax
    {
        // TODO
    }

    public sealed class NumberMapBuilder : IPropertyMapFactory, INumberSyntax
    {
        public int Offset { get; set; } // TODO

        public int CalcSize(Type type)
        {
            throw new NotImplementedException();
        }

        public IByteConverter CreateConverter(IComponentContainer components, IMappingParameter parameters, Type type)
        {
            throw new NotImplementedException();
        }

        // TODO
    }

    public static class NumberMapExtensions
    {
        // TODO
    }
}
