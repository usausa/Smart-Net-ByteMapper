namespace Smart.IO.Mapper.Expressions
{
    using System;

    using Smart.IO.Mapper.Builders;

    public interface IMapBytesSyntax
    {
        IMapBytesSyntax Filler(byte value);
    }

    internal sealed class MapBytesExpression : IMemberMapExpression, IMapBytesSyntax
    {
        private readonly BytesConverterBuilder builder = new BytesConverterBuilder();

        public MapBytesExpression(int length)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            builder.Length = length;
        }

        public IMapBytesSyntax Filler(byte value)
        {
            builder.Filler = value;
            return this;
        }

        IMapConverterBuilder IMemberMapExpression.GetMapConverterBuilder()
        {
            return builder;
        }
    }
}
