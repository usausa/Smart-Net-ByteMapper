namespace Smart.IO.ByteMapper
{
    using System;
    using System.Globalization;
    using System.Text;

    using Smart.IO.ByteMapper.Expressions;

    public static class ExpressionExtensions
    {
        //--------------------------------------------------------------------------------
        // ByteMapperConfig
        //--------------------------------------------------------------------------------

        public static ITypeConfigSyntax<T> CreateMapByExpression<T>(this MapperFactoryConfig config, int size)
        {
            return config.CreateMapByExpression<T>(size, null);
        }

        public static ITypeConfigSyntax<T> CreateMapByExpression<T>(this MapperFactoryConfig config, int size, string profile)
        {
            var expression = new TypeConfigExpression<T>(typeof(T), profile, size);
            config.AddMappingFactory(expression);
            return expression;
        }

        public static ITypeConfigSyntax<object> CreateMapByExpression(this MapperFactoryConfig config, Type type, int size)
        {
            return config.CreateMapByExpression(type, size, null);
        }

        public static ITypeConfigSyntax<object> CreateMapByExpression(this MapperFactoryConfig config, Type type, int size, string profile)
        {
            var expression = new TypeConfigExpression<object>(type, profile, size);
            config.AddMappingFactory(expression);
            return expression;
        }

        //--------------------------------------------------------------------------------
        // ByteMapperProfile
        //--------------------------------------------------------------------------------

        public static ITypeConfigSyntax<T> CreateMapByExpression<T>(this MapperProfile profile, int size)
        {
            var expression = new TypeConfigExpression<T>(typeof(T), profile.Name, size);
            profile.AddMappingFactory(expression);
            return expression;
        }

        public static ITypeConfigSyntax<object> CreateMapByExpression(this MapperProfile profile, Type type, int size)
        {
            var expression = new TypeConfigExpression<object>(type, profile.Name, size);
            profile.AddMappingFactory(expression);
            return expression;
        }

        //--------------------------------------------------------------------------------
        // Type
        //--------------------------------------------------------------------------------

        // Default

        public static ITypeConfigSyntax<T> TypeDelimiter<T>(this ITypeConfigSyntax<T> syntax, byte[] value)
        {
            return syntax.TypeDefault(Parameter.Delimiter, value);
        }

        public static ITypeConfigSyntax<T> TypeEncoding<T>(this ITypeConfigSyntax<T> syntax, Encoding value)
        {
            return syntax.TypeDefault(Parameter.Encoding, value);
        }

        public static ITypeConfigSyntax<T> TypeNumberProvider<T>(this ITypeConfigSyntax<T> syntax, IFormatProvider value)
        {
            return syntax.TypeDefault(Parameter.NumberProvider, value);
        }

        public static ITypeConfigSyntax<T> TypeDateTimeProvider<T>(this ITypeConfigSyntax<T> syntax, IFormatProvider value)
        {
            return syntax.TypeDefault(Parameter.DateTimeProvider, value);
        }

        public static ITypeConfigSyntax<T> TypeNumberStyle<T>(this ITypeConfigSyntax<T> syntax, NumberStyles value)
        {
            return syntax.TypeDefault(Parameter.NumberStyle, value);
        }

        public static ITypeConfigSyntax<T> TypeDecimalStyle<T>(this ITypeConfigSyntax<T> syntax, NumberStyles value)
        {
            return syntax.TypeDefault(Parameter.DecimalStyle, value);
        }

        public static ITypeConfigSyntax<T> TypeDateTimeStyle<T>(this ITypeConfigSyntax<T> syntax, DateTimeStyles value)
        {
            return syntax.TypeDefault(Parameter.DateTimeStyle, value);
        }

        public static ITypeConfigSyntax<T> TypeTrim<T>(this ITypeConfigSyntax<T> syntax, bool value)
        {
            return syntax.TypeDefault(Parameter.Trim, value);
        }

        public static ITypeConfigSyntax<T> TypeTextPadding<T>(this ITypeConfigSyntax<T> syntax, Padding value)
        {
            return syntax.TypeDefault(Parameter.TextPadding, value);
        }

        public static ITypeConfigSyntax<T> TypeNumberPadding<T>(this ITypeConfigSyntax<T> syntax, Padding value)
        {
            return syntax.TypeDefault(Parameter.NumberPadding, value);
        }

        public static ITypeConfigSyntax<T> TypeFiller<T>(this ITypeConfigSyntax<T> syntax, byte value)
        {
            return syntax.TypeDefault(Parameter.Filler, value);
        }

        public static ITypeConfigSyntax<T> TypeTextFiller<T>(this ITypeConfigSyntax<T> syntax, byte value)
        {
            return syntax.TypeDefault(Parameter.TextFiller, value);
        }

        public static ITypeConfigSyntax<T> TypeNumberFiller<T>(this ITypeConfigSyntax<T> syntax, byte value)
        {
            return syntax.TypeDefault(Parameter.NumberFiller, value);
        }

        public static ITypeConfigSyntax<T> TypeEndian<T>(this ITypeConfigSyntax<T> syntax, Endian value)
        {
            return syntax.TypeDefault(Parameter.Endian, value);
        }

        public static ITypeConfigSyntax<T> TypeTrueValue<T>(this ITypeConfigSyntax<T> syntax, byte value)
        {
            return syntax.TypeDefault(Parameter.TrueValue, value);
        }

        public static ITypeConfigSyntax<T> TypeFalseValue<T>(this ITypeConfigSyntax<T> syntax, byte value)
        {
            return syntax.TypeDefault(Parameter.FalseValue, value);
        }

        // Type

        public static ITypeConfigSyntax<T> UseDelimitter<T>(this ITypeConfigSyntax<T> syntax, byte[] value)
        {
            if ((value != null) && (value.Length > 0))
            {
                syntax.UseDelimitter(true).TypeDelimiter(value);
            }
            else
            {
                syntax.UseDelimitter(false).TypeDelimiter(null);
            }

            return syntax;
        }

        // Const

        public static ITypeConfigSyntax<T> Constant<T>(this ITypeConfigSyntax<T> syntax, byte[] content)
        {
            syntax.Map(new MapConstantExpression(content));
            return syntax;
        }

        public static ITypeConfigSyntax<T> Constant<T>(this ITypeConfigSyntax<T> syntax, int offset, byte[] content)
        {
            syntax.Map(offset, new MapConstantExpression(content));
            return syntax;
        }

        // Filler

        public static ITypeConfigSyntax<T> Filler<T>(this ITypeConfigSyntax<T> syntax, int length)
        {
            syntax.Map(new MapFillerExpression(length));
            return syntax;
        }

        public static ITypeConfigSyntax<T> Filler<T>(this ITypeConfigSyntax<T> syntax, int length, byte filler)
        {
            syntax.Map(new MapFillerExpression(length));
            return syntax;
        }

        public static ITypeConfigSyntax<T> Filler<T>(this ITypeConfigSyntax<T> syntax, int offset, int length)
        {
            syntax.Map(offset, new MapFillerExpression(length));
            return syntax;
        }

        public static ITypeConfigSyntax<T> Filler<T>(this ITypeConfigSyntax<T> syntax, int offset, int length, byte filler)
        {
            syntax.Map(offset, new MapFillerExpression(length, filler));
            return syntax;
        }

        //--------------------------------------------------------------------------------
        // Member
        //--------------------------------------------------------------------------------

        // Binary

        public static IMapBinarySyntax Binary(this IMemberMapConfigSyntax syntax)
        {
            var expression = new MapBinaryExpression();
            syntax.Map(expression);
            return expression;
        }

        public static void Binary(this IMemberMapConfigSyntax syntax, Endian endian)
        {
            var expression = new MapBinaryExpression();
            expression
                .Endian(endian);
            syntax.Map(expression);
        }

        // Boolean

        public static IMapBooleanSyntax Boolean(this IMemberMapConfigSyntax syntax)
        {
            var expression = new MapBooleanExpression();
            syntax.Map(expression);
            return expression;
        }

        public static IMapBooleanSyntax Boolean(this IMemberMapConfigSyntax syntax, byte trueValue, byte falseValue)
        {
            var expression = new MapBooleanExpression();
            expression
                .True(trueValue)
                .False(falseValue);
            syntax.Map(expression);
            return expression;
        }

        public static void Boolean(this IMemberMapConfigSyntax syntax, byte trueValue, byte falseValue, byte nullValue)
        {
            var expression = new MapBooleanExpression();
            expression
                .True(trueValue)
                .False(falseValue)
                .Null(nullValue);
            syntax.Map(expression);
        }

        // Byte

        public static void Byte(this IMemberMapConfigSyntax syntax)
        {
            syntax.Map(new MapByteExpression());
        }

        // Bytes

        public static IMapBytesSyntax Bytes(this IMemberMapConfigSyntax syntax, int length)
        {
            var expression = new MapBytesExpression(length);
            syntax.Map(expression);
            return expression;
        }

        // DateTime

        public static IMapDateTimeSyntax DateTime(this IMemberMapConfigSyntax syntax, int length, string format)
        {
            var expression = new MapDateTimeTextExpression(length, format);
            syntax.Map(expression);
            return expression;
        }

        // Number

        public static IMapNumberSyntax Number(this IMemberMapConfigSyntax syntax, int length)
        {
            var expression = new MapNumberTextExpression(length);
            syntax.Map(expression);
            return expression;
        }

        public static IMapNumberSyntax Number(this IMemberMapConfigSyntax syntax, int length, string format)
        {
            var expression = new MapNumberTextExpression(length, format);
            syntax.Map(expression);
            return expression;
        }

        // String

        public static IMapTextSyntax Text(this IMemberMapConfigSyntax syntax, int length)
        {
            var expression = new MapTextExpression(length);
            syntax.Map(expression);
            return expression;
        }
    }
}
