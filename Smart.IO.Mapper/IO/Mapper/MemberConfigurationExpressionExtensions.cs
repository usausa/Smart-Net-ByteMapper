namespace Smart.IO.Mapper
{
    using System;

    using Smart.IO.Mapper.Converters;

    /// <summary>
    ///
    /// </summary>
    public static class MemberConfigurationExpressionExtensions
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static IMemberConfigurationExpression Format(
            this IMemberConfigurationExpression expression,
            string format)
        {
            return Format(expression, format, null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="format"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static IMemberConfigurationExpression Format(
            this IMemberConfigurationExpression expression,
            string format,
            IFormatProvider provider)
        {
            return expression.Converter(new DefaultConverter(format, provider));
        }
    }
}
