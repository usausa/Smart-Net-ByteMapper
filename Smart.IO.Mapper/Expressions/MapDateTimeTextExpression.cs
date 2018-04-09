namespace Smart.IO.Mapper.Expressions
{
    using System;
    using System.Globalization;
    using System.Text;

    using Smart.IO.Mapper.Builders;

    public interface IMapDateTimeSyntax
    {
        IMapDateTimeSyntax Encoding(Encoding value);

        IMapDateTimeSyntax Filler(byte value);

        IMapDateTimeSyntax Style(DateTimeStyles value);

        IMapDateTimeSyntax Provider(IFormatProvider value);
    }

    internal sealed class MapDateTimeTextExpression : IMemberMapExpression, IMapDateTimeSyntax
    {
        private readonly DateTimeTextConverterBuilder builder = new DateTimeTextConverterBuilder();

        public MapDateTimeTextExpression(int length, string format)
        {
            builder.Length = length;
            builder.Format = format;
        }

        public IMapDateTimeSyntax Encoding(Encoding value)
        {
            builder.Encoding = value;
            return this;
        }

        public IMapDateTimeSyntax Filler(byte value)
        {
            builder.Filler = value;
            return this;
        }

        public IMapDateTimeSyntax Style(DateTimeStyles value)
        {
            builder.Style = value;
            return this;
        }

        public IMapDateTimeSyntax Provider(IFormatProvider value)
        {
            builder.Provider = value;
            return this;
        }

        IMapConverterBuilder IMemberMapExpression.GetMapConverterBuilder()
        {
            return builder;
        }
    }
}
