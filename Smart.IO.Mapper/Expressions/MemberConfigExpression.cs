namespace Smart.IO.Mapper.Expressions
{
    using System;

    internal sealed class MemberConfigExpression : IMemberConfigSyntax
    {
        public IMemberMapExpression Expression { get; private set; }

        void IMemberMapConfigSyntax.Map(IMemberMapExpression expression)
        {
            Expression = expression;
        }

        public void Array(int length, Action<IMemberMapConfigSyntax> config)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

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
