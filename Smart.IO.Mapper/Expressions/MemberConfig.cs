namespace Smart.IO.Mapper.Expressions
{
    using System;

    internal class MemberConfig : IMemberConfigSyntax
    {
        public IMemberMapExpression Expression { get; private set; }

        public void Map(IMemberMapExpression expression)
        {
            Expression = expression;
        }

        public IMemberMapConfigSyntax Array(int length)
        {
            // TODO thisを返す
            throw new NotImplementedException();
        }
    }

    internal class MemberMapArrayBuilder : IMemberMapConfigSyntax
    {
        public void Map(IMemberMapExpression expression)
        {
            throw new NotImplementedException();
        }
    }
}
