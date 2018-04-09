namespace Smart.IO.Mapper.Expressions
{
    using Smart.IO.Mapper.Builders;

    internal sealed class MapConstantExpression : ITypeMapExpression
    {
        private readonly ConstantTypeMapperBuilder builder = new ConstantTypeMapperBuilder();

        public MapConstantExpression(byte[] content)
        {
            builder.Content = content;
        }

        ITypeMapperBuilder ITypeMapExpression.GetTypeMapperBuilder()
        {
            return builder;
        }
    }
}
