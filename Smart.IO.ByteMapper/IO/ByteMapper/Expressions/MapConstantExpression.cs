namespace Smart.IO.ByteMapper.Expressions
{
    using System;

    using Smart.IO.ByteMapper.Builders;

    internal sealed class MapConstantExpression : ITypeMapExpression
    {
        private readonly ConstantTypeMapperBuilder builder = new ConstantTypeMapperBuilder();

        public MapConstantExpression(params byte[] content)
        {
            if (content is null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            builder.Content = content;
        }

        //--------------------------------------------------------------------------------
        // Expression
        //--------------------------------------------------------------------------------

        ITypeMapperBuilder ITypeMapExpression.GetTypeMapperBuilder() => builder;
    }
}
