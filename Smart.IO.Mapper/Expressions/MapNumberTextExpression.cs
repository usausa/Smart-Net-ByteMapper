namespace Smart.IO.Mapper.Expressions
{
    using System;
    using System.Globalization;
    using System.Text;

    using Smart.IO.Mapper.Builders;

    public interface IMapNumberSyntax
    {
        IMapNumberSyntax Encoding(Encoding value);

        IMapNumberSyntax Trim(bool value);

        IMapNumberSyntax Padding(Padding value);

        IMapNumberSyntax Filler(byte value);

        IMapNumberSyntax Style(NumberStyles value);

        IMapNumberSyntax Provider(IFormatProvider value);
    }

    internal sealed class MapNumberTextExpression : IMemberMapExpression, IMapNumberSyntax
    {
        private readonly NumberTextConverterBuilder builder = new NumberTextConverterBuilder();

        public MapNumberTextExpression(int length)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            builder.Length = length;
        }

        public MapNumberTextExpression(int length, string format)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            builder.Length = length;
            builder.Format = format;
        }

        public IMapNumberSyntax Encoding(Encoding value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            builder.Encoding = value;
            return this;
        }

        public IMapNumberSyntax Trim(bool value)
        {
            builder.Trim = value;
            return this;
        }

        public IMapNumberSyntax Padding(Padding value)
        {
            builder.Padding = value;
            return this;
        }

        public IMapNumberSyntax Filler(byte value)
        {
            builder.Filler = value;
            return this;
        }

        public IMapNumberSyntax Style(NumberStyles value)
        {
            builder.Style = value;
            return this;
        }

        public IMapNumberSyntax Provider(IFormatProvider value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            builder.Provider = value;
            return this;
        }

        IMapConverterBuilder IMemberMapExpression.GetMapConverterBuilder()
        {
            return builder;
        }
    }
}
