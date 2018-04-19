namespace Smart.IO.ByteMapper
{
    using System;
    using System.Globalization;
    using System.Text;

    using Smart.IO.ByteMapper.Expressions;

    public static class OptionsExpressionExtensions
    {
        //--------------------------------------------------------------------------------
        // Type
        //--------------------------------------------------------------------------------

        // Default

        public static ITypeConfigSyntax<T> TypeDateTimeTextEncoding<T>(this ITypeConfigSyntax<T> syntax, Encoding value)
        {
            return syntax.TypeDefault(DateTimeTextParameter.Encoding, value);
        }

        public static ITypeConfigSyntax<T> TypeDateTimeTextProvider<T>(this ITypeConfigSyntax<T> syntax, IFormatProvider value)
        {
            return syntax.TypeDefault(DateTimeTextParameter.Provider, value);
        }

        public static ITypeConfigSyntax<T> TypeDateTimeTextStyle<T>(this ITypeConfigSyntax<T> syntax, DateTimeStyles value)
        {
            return syntax.TypeDefault(DateTimeTextParameter.Style, value);
        }

        public static ITypeConfigSyntax<T> TypeNumberTextEncoding<T>(this ITypeConfigSyntax<T> syntax, Encoding value)
        {
            return syntax.TypeDefault(NumberTextParameter.Encoding, value);
        }

        public static ITypeConfigSyntax<T> TypeNumberTextProvider<T>(this ITypeConfigSyntax<T> syntax, IFormatProvider value)
        {
            return syntax.TypeDefault(NumberTextParameter.Provider, value);
        }

        public static ITypeConfigSyntax<T> TypeNumberTextNumberStyle<T>(this ITypeConfigSyntax<T> syntax, NumberStyles value)
        {
            return syntax.TypeDefault(NumberTextParameter.NumberStyle, value);
        }

        public static ITypeConfigSyntax<T> TypeNumberTextDecimalStyle<T>(this ITypeConfigSyntax<T> syntax, NumberStyles value)
        {
            return syntax.TypeDefault(NumberTextParameter.DecimalStyle, value);
        }

        public static ITypeConfigSyntax<T> TypeNumberTextPadding<T>(this ITypeConfigSyntax<T> syntax, Padding value)
        {
            return syntax.TypeDefault(NumberTextParameter.Padding, value);
        }

        public static ITypeConfigSyntax<T> TypeNumberTextFiller<T>(this ITypeConfigSyntax<T> syntax, byte value)
        {
            return syntax.TypeDefault(NumberTextParameter.Filler, value);
        }

        //--------------------------------------------------------------------------------
        // Member
        //--------------------------------------------------------------------------------

        // DateTime

        public static IMapDateTimeTextSyntax DateTimeText(this IMemberMapConfigSyntax syntax, int length, string format)
        {
            var expression = new MapDateTimeTextExpression(length, format);
            syntax.Map(expression);
            return expression;
        }

        // Number

        public static IMapNumberTextSyntax NumberText(this IMemberMapConfigSyntax syntax, int length)
        {
            var expression = new MapNumberTextTextExpression(length);
            syntax.Map(expression);
            return expression;
        }

        public static IMapNumberTextSyntax NumberText(this IMemberMapConfigSyntax syntax, int length, string format)
        {
            var expression = new MapNumberTextTextExpression(length, format);
            syntax.Map(expression);
            return expression;
        }
    }
}
