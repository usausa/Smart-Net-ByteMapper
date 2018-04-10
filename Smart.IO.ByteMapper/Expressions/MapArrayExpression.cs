namespace Smart.IO.ByteMapper.Expressions
{
    using System;

    using Smart.IO.ByteMapper.Builders;

    internal sealed class MapArrayExpression : IMemberMapExpression
    {
        private readonly ArrayConverterBuilder builder = new ArrayConverterBuilder();

        public MapArrayExpression(int length, IMapConverterBuilder elementConverterBuilder)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            builder.Length = length;
            builder.ElementConverterBuilder = elementConverterBuilder;
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
