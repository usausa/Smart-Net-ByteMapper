namespace Smart.IO.ByteMapper.Expressions;

internal sealed class MemberConfigExpression : IMemberConfigSyntax
{
    public IMemberMapExpression Expression { get; private set; }

    void IMemberMapConfigSyntax.Map(IMemberMapExpression expression)
    {
        Expression = expression;
    }

    //--------------------------------------------------------------------------------
    // Syntax
    //--------------------------------------------------------------------------------

    public IMapArraySyntax Array(int length, Action<IMemberMapConfigSyntax> config)
    {
        if (length < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        if (config is null)
        {
            throw new ArgumentNullException(nameof(config));
        }

        var element = new ElementConfigExpression();
        config(element);

        if (element.Expression is null)
        {
            throw new InvalidOperationException("Element is not mapped.");
        }

        var expression = new MapArrayExpression(length, element.Expression.GetMapConverterBuilder());
        Expression = expression;

        return expression;
    }
}
