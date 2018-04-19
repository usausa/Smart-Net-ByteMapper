namespace Smart.IO.ByteMapper.Expressions
{
    using Smart.IO.ByteMapper.Builders;

    public interface IMapDateTimeSyntax
    {
        IMapDateTimeSyntax Filler(byte value);
    }

    internal sealed class MapDateTimeExpression : IMemberMapExpression, IMapDateTimeSyntax
    {
        private readonly DateTimeConverterBuilder builder = new DateTimeConverterBuilder();

        public MapDateTimeExpression(string format)
        {
            builder.Format = format;
        }

        //--------------------------------------------------------------------------------
        // Syntax
        //--------------------------------------------------------------------------------

        public IMapDateTimeSyntax Filler(byte value)
        {
            builder.Filler = value;
            return this;
        }

        //--------------------------------------------------------------------------------
        // Expression
        //--------------------------------------------------------------------------------

        public IMapConverterBuilder GetMapConverterBuilder()
        {
            return builder;
        }
    }
}
