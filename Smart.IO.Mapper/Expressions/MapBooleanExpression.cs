namespace Smart.IO.Mapper.Expressions
{
    using Smart.IO.Mapper.Builders;

    public interface IMapBooleanSyntax
    {
        IMapBooleanSyntax True(byte value);

        IMapBooleanSyntax False(byte value);

        IMapBooleanSyntax Null(byte value);
    }

    internal sealed class MapBooleanExpression : IMemberMapExpression, IMapBooleanSyntax
    {
        private readonly BooleanConverterBuilder builder = new BooleanConverterBuilder();

        public IMapBooleanSyntax True(byte value)
        {
            builder.TrueValue = value;
            return this;
        }

        public IMapBooleanSyntax False(byte value)
        {
            builder.FalseValue = value;
            return this;
        }

        public IMapBooleanSyntax Null(byte value)
        {
            builder.NullValue = value;
            return this;
        }

        IMapConverterBuilder IMemberMapExpression.GetMapConverterBuilder()
        {
            return builder;
        }
    }
}
