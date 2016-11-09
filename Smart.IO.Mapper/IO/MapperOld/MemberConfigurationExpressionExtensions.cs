namespace Smart.IO.MapperOld
{
    using System;
    using System.Globalization;

    using Smart.IO.MapperOld.Converters;

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
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            return expression.Converter(new DefaultConverter(format, null));
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
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            return expression.Converter(new DateTimeConverter(format, null));
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

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="format"></param>
        /// <param name="provider"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        public static IMemberConfigurationExpression DateTime(
            this IMemberConfigurationExpression expression,
            string format,
            IFormatProvider provider,
            DateTimeStyles style)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            return expression.Converter(new DateTimeConverter(format, provider, style));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="trueValue"></param>
        /// <param name="falseValue"></param>
        /// <returns></returns>
        public static IMemberConfigurationExpression Boolean(
            this IMemberConfigurationExpression expression,
            byte trueValue,
            byte falseValue)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            return expression.Converter(new BooleanConverter(new[] { trueValue }, new[] { falseValue }));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="trueValue"></param>
        /// <param name="falseValue"></param>
        /// <returns></returns>
        public static IMemberConfigurationExpression Boolean(
            this IMemberConfigurationExpression expression,
            byte[] trueValue,
            byte[] falseValue)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            return expression.Converter(new BooleanConverter(trueValue, falseValue));
        }
    }
}
