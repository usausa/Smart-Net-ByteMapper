namespace Smart.IO.ByteMapper.Expressions
{
    using Smart.IO.ByteMapper.Builders;

    public interface ITypeMapExpression
    {
        ITypeMapperBuilder GetTypeMapperBuilder();
    }
}
