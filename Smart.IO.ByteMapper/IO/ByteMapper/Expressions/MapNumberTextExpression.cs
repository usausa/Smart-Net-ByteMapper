namespace Smart.IO.ByteMapper.Expressions
{
    using System;
    using System.Globalization;
    using System.Text;

    using Smart.IO.ByteMapper.Builders;

    public interface IMapNumberTextSyntax
    {
        IMapNumberTextSyntax Encoding(Encoding value);

        IMapNumberTextSyntax Trim(bool value);

        IMapNumberTextSyntax Padding(Padding value);

        IMapNumberTextSyntax Filler(byte value);

        IMapNumberTextSyntax Style(NumberStyles value);

        IMapNumberTextSyntax Provider(IFormatProvider value);
    }

    internal sealed class MapNumberTextTextExpression : IMemberMapExpression, IMapNumberTextSyntax
    {
        private readonly NumberTextConverterBuilder builder = new NumberTextConverterBuilder();

        public MapNumberTextTextExpression(int length)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            builder.Length = length;
        }

        public MapNumberTextTextExpression(int length, string format)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            builder.Length = length;
            builder.Format = format;
        }

        //--------------------------------------------------------------------------------
        // Syntax
        //--------------------------------------------------------------------------------

        public IMapNumberTextSyntax Encoding(Encoding value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            builder.Encoding = value;
            return this;
        }

        public IMapNumberTextSyntax Trim(bool value)
        {
            builder.Trim = value;
            return this;
        }

        public IMapNumberTextSyntax Padding(Padding value)
        {
            builder.Padding = value;
            return this;
        }

        public IMapNumberTextSyntax Filler(byte value)
        {
            builder.Filler = value;
            return this;
        }

        public IMapNumberTextSyntax Style(NumberStyles value)
        {
            builder.Style = value;
            return this;
        }

        public IMapNumberTextSyntax Provider(IFormatProvider value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            builder.Provider = value;
            return this;
        }

        //--------------------------------------------------------------------------------
        // Expression
        //--------------------------------------------------------------------------------

        IMapConverterBuilder IMemberMapExpression.GetMapConverterBuilder() => builder;
    }
}
