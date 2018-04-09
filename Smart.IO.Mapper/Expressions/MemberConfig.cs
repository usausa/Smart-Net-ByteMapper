namespace Smart.IO.Mapper.Expressions
{
    using System;

    internal sealed class MemberConfig : IMemberConfigSyntax
    {
        public IMemberMapExpression Expression { get; private set; }

        public void Map(IMemberMapExpression expression)
        {
            Expression = expression;
        }

        public void Array(int length, Action<IMemberMapConfigSyntax> config)
        {
            // TODO arrayチェック！

            var element = new ElementConfig();
            config(element);

            if (element.Expression == null)
            {
                throw new InvalidOperationException("Element is not mapped.");
            }

            Expression = new MapArrayExpression(length, element.Expression.GetMapConverterBuilder());
        }
    }
}
