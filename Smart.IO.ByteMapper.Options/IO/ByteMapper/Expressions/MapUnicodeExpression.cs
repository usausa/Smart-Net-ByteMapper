namespace Smart.IO.ByteMapper.Expressions
{
    using System;

    using Smart.IO.ByteMapper.Builders;

    public interface IMapUnicodeSyntax
    {
        IMapUnicodeSyntax Trim(bool value);

        IMapUnicodeSyntax Padding(Padding value);

        IMapUnicodeSyntax Filler(char value);
    }

    internal sealed class MapUnicodeExpression : IMemberMapExpression, IMapUnicodeSyntax
    {
        private readonly UnicodeConverterBuilder builder = new UnicodeConverterBuilder();

        public MapUnicodeExpression(int length)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            if (length % 2 != 0)
            {
                throw new ArgumentException("Invalid length.", nameof(length));
            }

            builder.Length = length;
        }

        //--------------------------------------------------------------------------------
        // Syntax
        //--------------------------------------------------------------------------------

        public IMapUnicodeSyntax Trim(bool value)
        {
            builder.Trim = value;
            return this;
        }

        public IMapUnicodeSyntax Padding(Padding value)
        {
            builder.Padding = value;
            return this;
        }

        public IMapUnicodeSyntax Filler(char value)
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
