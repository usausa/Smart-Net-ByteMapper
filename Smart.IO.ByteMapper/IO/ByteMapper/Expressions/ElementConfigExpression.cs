namespace Smart.IO.ByteMapper.Expressions;

internal sealed class ElementConfigExpression : IMemberMapConfigSyntax
{
    public IMemberMapExpression Expression { get; private set; }

    //--------------------------------------------------------------------------------
    // Syntax
    //--------------------------------------------------------------------------------

    void IMemberMapConfigSyntax.Map(IMemberMapExpression expression)
    {
        if (expression is null)
        {
            throw new ArgumentNullException(nameof(expression));
        }

        Expression = expression;
    }
}
