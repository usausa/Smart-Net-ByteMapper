namespace Smart.IO.Mapper.Expressions
{
    using System;

    using Smart.ComponentModel;
    using Smart.IO.Mapper.Converters;
    using Smart.IO.Mapper.Helpers;

    public interface IMapNumberSyntax
    {
        // TODO
    }

    internal sealed class MapNumberExpression : IMemberMapFactory, IMapNumberSyntax
    {
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
}
