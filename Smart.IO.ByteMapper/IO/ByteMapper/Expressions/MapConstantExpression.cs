namespace Smart.IO.ByteMapper.Expressions;

using Smart.IO.ByteMapper.Builders;

internal sealed class MapConstantExpression : ITypeMapExpression
{
    private readonly ConstantTypeMapperBuilder builder = new();

    public MapConstantExpression(params byte[] content)
    {
        builder.Content = content;
    }

    //--------------------------------------------------------------------------------
    // Expression
    //--------------------------------------------------------------------------------

    ITypeMapperBuilder ITypeMapExpression.GetTypeMapperBuilder() => builder;
}
