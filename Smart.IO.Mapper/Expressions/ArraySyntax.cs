namespace Smart.IO.Mapper.Expressions
{
    using System;

    using Smart.ComponentModel;
    using Smart.IO.Mapper.Converters;
    using Smart.IO.Mapper.Helpers;

    public interface IArraySyntax
    {
        IArraySyntax Count(int count);

        // TODO Property
    }

    public sealed class ArrayMapBuilder : IPropertyMapFactory, IArraySyntax, IPropertyMapSyntax
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

        public IArraySyntax Count(int count)
        {
            // TODO
            return this;
        }
    }

    public static class ArrayMapExtensions
    {
        // TODO
    }
}
