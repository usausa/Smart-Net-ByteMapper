namespace Smart.IO.Mapper.Expressions
{
    internal sealed class ElementConfig : IMemberMapConfigSyntax
    {
        public IMemberMapExpression Expression { get; private set; }

        public void Map(IMemberMapExpression expression)
        {
            Expression = expression;
        }
    }
}
