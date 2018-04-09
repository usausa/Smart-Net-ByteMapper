namespace Smart.IO.Mapper.Expressions
{
    using Smart.IO.Mapper.Builders;

    internal sealed class MapArrayExpression : IMemberMapExpression
    {
        private readonly ArrayConverterBuilder builder = new ArrayConverterBuilder();

        public MapArrayExpression(int length, IMapConverterBuilder elementConverterBuilder)
        {
            builder.Length = length;
            builder.ElementConverterBuilder = elementConverterBuilder;
        }

        public IMapConverterBuilder GetMapConverterBuilder()
        {
            return builder;
        }
    }
}
