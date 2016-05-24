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
        public static IMemberConfigurationExpression Formatter(
            this IMemberConfigurationExpression expression,
            string format)
        {
            return Formatter(expression, format, null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="format"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static IMemberConfigurationExpression Formatter(
            this IMemberConfigurationExpression expression,
            string format,
            IFormatProvider provider)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            return expression.Converter(new DefaultConverter(format, provider));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static IMemberConfigurationExpression DateTime(
            this IMemberConfigurationExpression expression,
            string format)
        {
            return DateTime(expression, format, null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="format"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static IMemberConfigurationExpression DateTime(
            this IMemberConfigurationExpression expression,
            string format,
            IFormatProvider provider)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            return expression.Converter(new DateTimeConverter(format, provider));
        }
    }
}
