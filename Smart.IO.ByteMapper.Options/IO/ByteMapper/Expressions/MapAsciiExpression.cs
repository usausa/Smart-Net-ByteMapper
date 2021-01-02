namespace Smart.IO.ByteMapper.Expressions
{
    using System;

    using Smart.IO.ByteMapper.Builders;

    public interface IMapAsciiSyntax
    {
        IMapAsciiSyntax Trim(bool value);

        IMapAsciiSyntax Padding(Padding value);

        IMapAsciiSyntax Filler(byte value);
    }

    internal sealed class MapAsciiExpression : IMemberMapExpression, IMapAsciiSyntax
    {
        private readonly AsciiConverterBuilder builder = new();

        public MapAsciiExpression(int length)
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

        public IMapAsciiSyntax Trim(bool value)
        {
            builder.Trim = value;
            return this;
        }

        public IMapAsciiSyntax Padding(Padding value)
        {
            builder.Padding = value;
            return this;
        }

        public IMapAsciiSyntax Filler(byte value)
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
