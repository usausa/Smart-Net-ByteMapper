namespace Smart.IO.Mapper.Expressions
{
    using System;

    using Smart.ComponentModel;
    using Smart.IO.Mapper.Converters;
    using Smart.IO.Mapper.Helpers;

    public interface IMapArraySyntax
    {
        IMapArraySyntax Count(int count);

        // TODO Member
    }

    internal sealed class MapArrayExpression : IMemberMapFactory, IMapArraySyntax, IMemberMapConfigSyntax
    {
        public int CalcSize(Type type)
        {
            throw new NotImplementedException();
        }

        public IByteConverter CreateConverter(IComponentContainer components, IMappingParameter parameters, Type type)
        {
            throw new NotImplementedException();
        }

        public IMapArraySyntax Count(int count)
        {
            // TODO
            return this;
        }
    }
}
