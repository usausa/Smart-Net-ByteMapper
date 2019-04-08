namespace Smart.IO.ByteMapper.Expressions
{
    using Smart.IO.ByteMapper.Builders;

    internal sealed class MapByteExpression : IMemberMapExpression
    {
        //--------------------------------------------------------------------------------
        // Expression
        //--------------------------------------------------------------------------------

        IMapConverterBuilder IMemberMapExpression.GetMapConverterBuilder() => ByteConverterBuilder.Default;
    }
}
