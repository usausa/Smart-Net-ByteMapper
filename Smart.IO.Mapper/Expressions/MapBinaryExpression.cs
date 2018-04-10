namespace Smart.IO.Mapper.Expressions
{
    using Smart.IO.Mapper.Builders;

    public interface IMapBinarySyntax
    {
        IMapBinarySyntax Endian(Endian value);
    }

    internal sealed class MapBinaryExpression : IMemberMapExpression, IMapBinarySyntax
    {
        private readonly BinaryConverterBuilder builder = new BinaryConverterBuilder();

        //--------------------------------------------------------------------------------
        // Syntax
        //--------------------------------------------------------------------------------

        public IMapBinarySyntax Endian(Endian value)
        {
            builder.Endian = value;
            return this;
        }

        //--------------------------------------------------------------------------------
        // Expression
        //--------------------------------------------------------------------------------

        IMapConverterBuilder IMemberMapExpression.GetMapConverterBuilder()
        {
            return builder;
        }
    }
}
