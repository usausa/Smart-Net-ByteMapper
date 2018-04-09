namespace Smart.IO.Mapper.Expressions
{
    using Smart.IO.Mapper.Builders;

    internal sealed class MapFillerExpression : ITypeMapExpression
    {
        private readonly FillerTypeMapperBuilder builder = new FillerTypeMapperBuilder();

        public MapFillerExpression(int length)
        {
            builder.Length = length;
        }

        public MapFillerExpression(int length, byte filler)
        {
            builder.Length = length;
            builder.Filler = filler;
        }

        ITypeMapperBuilder ITypeMapExpression.GetTypeMapperBuilder()
        {
            return builder;
        }
    }
}
