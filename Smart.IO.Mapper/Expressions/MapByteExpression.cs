namespace Smart.IO.Mapper.Expressions
{
    using Smart.IO.Mapper.Builders;

    internal sealed class MapByteExpression : IMemberMapExpression
    {
        IMapConverterBuilder IMemberMapExpression.GetMapConverterBuilder()
        {
            return ByteConverterBuilder.Default;
        }
    }
}
