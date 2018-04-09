namespace Smart.IO.Mapper.Expressions
{
    using System;

    using Smart.ComponentModel;
    using Smart.IO.Mapper.Converters;
    using Smart.IO.Mapper.Helpers;

    public interface IMapArraySyntax
    {
        //IMapArraySyntax Count(int length); 必須

        // TODO Member
    }

    internal sealed class MapArrayExpression : IMemberMapFactory, IMapArraySyntax, IMemberMapConfigSyntax
    {
        public void Map(IMemberMapFactory factory)
        {
            throw new NotImplementedException();
        }

        public int CalcSize(Type type)
        {
            throw new NotImplementedException();
        }

        public IMapConverter CreateConverter(IComponentContainer components, IMappingParameter parameters, Type type)
        {
            throw new NotImplementedException();
        }
    }
}
