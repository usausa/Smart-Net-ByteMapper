namespace Smart.IO.ByteMapper.Expressions
{
    using System;

    using Smart.IO.ByteMapper.Builders;

    public interface IMapDecimalSyntax
    {
        IMapDecimalSyntax UseGrouping(bool value);

        IMapDecimalSyntax GroupingSize(int value);

        IMapDecimalSyntax Padding(Padding value);

        IMapDecimalSyntax ZeroFill(bool value);

        IMapDecimalSyntax Filler(byte value);
    }

    internal sealed class MapDecimalExpression : IMemberMapExpression, IMapDecimalSyntax
    {
        private readonly DecimalConverterBuilder builder = new DecimalConverterBuilder();

        public MapDecimalExpression(int length)
            : this(length, 0)
        {
        }

        public MapDecimalExpression(int length, byte scale)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            if ((scale != 0) && (scale + 1 >= length))
            {
                throw new ArgumentException($"Invalid scale. length=[{length}], scale=[{scale}]");
            }

            builder.Length = length;
            builder.Scale = scale;
        }

        //--------------------------------------------------------------------------------
        // Syntax
        //--------------------------------------------------------------------------------

        public IMapDecimalSyntax UseGrouping(bool value)
        {
            builder.UseGrouping = value;
            return this;
        }

        public IMapDecimalSyntax GroupingSize(int value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            builder.GroupingSize = value;
            builder.UseGrouping = value > 0;
            return this;
        }

        public IMapDecimalSyntax Padding(Padding value)
        {
            builder.Padding = value;
            return this;
        }

        public IMapDecimalSyntax ZeroFill(bool value)
        {
            builder.ZeroFill = value;
            return this;
        }

        public IMapDecimalSyntax Filler(byte value)
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
