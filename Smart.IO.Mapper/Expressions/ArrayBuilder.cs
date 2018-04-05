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

    internal sealed class ArrayMapBuilder : IPropertyMapFactory, IArraySyntax, IPropertyMapConfigSyntax
    {
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
}
