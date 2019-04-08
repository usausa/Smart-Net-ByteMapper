namespace Smart.IO.ByteMapper.Expressions
{
    using Smart.IO.ByteMapper.Builders;

    public interface IMapBooleanSyntax
    {
        IMapBooleanSyntax True(byte value);

        IMapBooleanSyntax False(byte value);

        IMapBooleanSyntax Null(byte value);
    }

    internal sealed class MapBooleanExpression : IMemberMapExpression, IMapBooleanSyntax
    {
        private readonly BooleanConverterBuilder builder = new BooleanConverterBuilder();

        //--------------------------------------------------------------------------------
        // Syntax
        //--------------------------------------------------------------------------------

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

        //--------------------------------------------------------------------------------
        // Expression
        //--------------------------------------------------------------------------------

        IMapConverterBuilder IMemberMapExpression.GetMapConverterBuilder() => builder;
    }
}
