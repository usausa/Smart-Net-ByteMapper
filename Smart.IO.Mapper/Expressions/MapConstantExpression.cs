namespace Smart.IO.Mapper.Expressions
{
    using System;

    using Smart.IO.Mapper.Builders;

    internal sealed class MapConstantExpression : ITypeMapExpression
    {
        private readonly ConstantTypeMapperBuilder builder = new ConstantTypeMapperBuilder();

        public MapConstantExpression(byte[] content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            builder.Content = content;
        }

        //--------------------------------------------------------------------------------
        // Expression
        //--------------------------------------------------------------------------------

        ITypeMapperBuilder ITypeMapExpression.GetTypeMapperBuilder()
        {
            return builder;
        }
    }
}
