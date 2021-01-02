namespace Smart.IO.ByteMapper.Expressions
{
    using System;

    using Smart.IO.ByteMapper.Builders;

    public interface IMapIntegerSyntax
    {
        IMapIntegerSyntax Padding(Padding value);

        IMapIntegerSyntax ZeroFill(bool value);

        IMapIntegerSyntax Filler(byte value);
    }

    internal sealed class MapIntegerExpression : IMemberMapExpression, IMapIntegerSyntax
    {
        private readonly IntegerConverterBuilder builder = new();

        public MapIntegerExpression(int length)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            builder.Length = length;
        }

        //--------------------------------------------------------------------------------
        // Syntax
        //--------------------------------------------------------------------------------

        public IMapIntegerSyntax Padding(Padding value)
        {
            builder.Padding = value;
            return this;
        }

        public IMapIntegerSyntax ZeroFill(bool value)
        {
            builder.ZeroFill = value;
            return this;
        }

        public IMapIntegerSyntax Filler(byte value)
        {
            builder.Filler = value;
            return this;
        }

        //--------------------------------------------------------------------------------
        // Expression
        //--------------------------------------------------------------------------------

        public IMapConverterBuilder GetMapConverterBuilder() => builder;
    }
}
