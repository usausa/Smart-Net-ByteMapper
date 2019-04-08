namespace Smart.IO.ByteMapper.Expressions
{
    using System;

    using Smart.IO.ByteMapper.Builders;

    internal sealed class MapFillerExpression : ITypeMapExpression
    {
        private readonly FillerTypeMapperBuilder builder = new FillerTypeMapperBuilder();

        public MapFillerExpression(int length)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            builder.Length = length;
        }

        public MapFillerExpression(int length, byte filler)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            builder.Length = length;
            builder.Filler = filler;
        }

        //--------------------------------------------------------------------------------
        // Expression
        //--------------------------------------------------------------------------------

        ITypeMapperBuilder ITypeMapExpression.GetTypeMapperBuilder() => builder;
    }
}
