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

        public static MapperFactoryConfig CreateMapByExpression<T>(this MapperFactoryConfig config, int size, Action<ITypeConfigSyntax<T>> action)
        {
            return config.CreateMapByExpression(size, null, action);
        }

        public static MapperFactoryConfig CreateMapByExpression<T>(this MapperFactoryConfig config, int size, string profile, Action<ITypeConfigSyntax<T>> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var expression = new TypeConfigExpression<T>(typeof(T), profile, size);
            action(expression);
            config.AddMappingFactory(expression);
            return config;
        }

        public static MapperFactoryConfig CreateMapByExpression(this MapperFactoryConfig config, Type type, int size, Action<ITypeConfigSyntax<object>> action)
        {
            return config.CreateMapByExpression(type, size, null, action);
        }

        public static MapperFactoryConfig CreateMapByExpression(this MapperFactoryConfig config, Type type, int size, string profile, Action<ITypeConfigSyntax<object>> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var expression = new TypeConfigExpression<object>(type, profile, size);
            action(expression);
            config.AddMappingFactory(expression);
            return config;
        }

        //--------------------------------------------------------------------------------
        // ByteMapperProfile
        //--------------------------------------------------------------------------------

        public static MapperProfile CreateMapByExpression<T>(this MapperProfile profile, int size, Action<ITypeConfigSyntax<T>> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var expression = new TypeConfigExpression<T>(typeof(T), profile.Name, size);
            action(expression);
            profile.AddMappingFactory(expression);
            return profile;
        }

        public static MapperProfile CreateMapByExpression(this MapperProfile profile, Type type, int size, Action<ITypeConfigSyntax<object>> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var expression = new TypeConfigExpression<object>(type, profile.Name, size);
            action(expression);
            profile.AddMappingFactory(expression);
            return profile;
        }

        //--------------------------------------------------------------------------------
        // Type
        //--------------------------------------------------------------------------------

        // Default

        public static ITypeConfigSyntax<T> TypeDelimiter<T>(this ITypeConfigSyntax<T> syntax, params byte[] value)
        {
            return syntax.TypeDefault(Parameter.Delimiter, value);
        }

        public static ITypeConfigSyntax<T> TypeEncoding<T>(this ITypeConfigSyntax<T> syntax, Encoding value)
        {
            return syntax.TypeDefault(Parameter.Encoding, value);
        }

        public static ITypeConfigSyntax<T> TypeTrim<T>(this ITypeConfigSyntax<T> syntax, bool value)
        {
            return syntax.TypeDefault(Parameter.Trim, value);
        }

        public static ITypeConfigSyntax<T> TypeTextPadding<T>(this ITypeConfigSyntax<T> syntax, Padding value)
        {
            return syntax.TypeDefault(Parameter.TextPadding, value);
        }

        public static ITypeConfigSyntax<T> TypeFiller<T>(this ITypeConfigSyntax<T> syntax, byte value)
        {
            return syntax.TypeDefault(Parameter.Filler, value);
        }

        public static ITypeConfigSyntax<T> TypeTextFiller<T>(this ITypeConfigSyntax<T> syntax, byte value)
        {
            return syntax.TypeDefault(Parameter.TextFiller, value);
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

        public static ITypeConfigSyntax<T> TypeDateTimeTextEncoding<T>(this ITypeConfigSyntax<T> syntax, Encoding value)
        {
            return syntax.TypeDefault(Parameter.DateTimeTextEncoding, value);
        }

        public static ITypeConfigSyntax<T> TypeDateTimeTextProvider<T>(this ITypeConfigSyntax<T> syntax, IFormatProvider value)
        {
            return syntax.TypeDefault(Parameter.DateTimeTextProvider, value);
        }

        public static ITypeConfigSyntax<T> TypeDateTimeTextStyle<T>(this ITypeConfigSyntax<T> syntax, DateTimeStyles value)
        {
            return syntax.TypeDefault(Parameter.DateTimeTextStyle, value);
        }

        public static ITypeConfigSyntax<T> TypeNumberTextEncoding<T>(this ITypeConfigSyntax<T> syntax, Encoding value)
        {
            return syntax.TypeDefault(Parameter.NumberTextEncoding, value);
        }

        public static ITypeConfigSyntax<T> TypeNumberTextProvider<T>(this ITypeConfigSyntax<T> syntax, IFormatProvider value)
        {
            return syntax.TypeDefault(Parameter.NumberTextProvider, value);
        }

        public static ITypeConfigSyntax<T> TypeNumberTextNumberStyle<T>(this ITypeConfigSyntax<T> syntax, NumberStyles value)
        {
            return syntax.TypeDefault(Parameter.NumberTextNumberStyle, value);
        }

        public static ITypeConfigSyntax<T> TypeNumberTextDecimalStyle<T>(this ITypeConfigSyntax<T> syntax, NumberStyles value)
        {
            return syntax.TypeDefault(Parameter.NumberTextDecimalStyle, value);
        }

        public static ITypeConfigSyntax<T> TypeNumberTextPadding<T>(this ITypeConfigSyntax<T> syntax, Padding value)
        {
            return syntax.TypeDefault(Parameter.NumberTextPadding, value);
        }

        public static ITypeConfigSyntax<T> TypeNumberTextFiller<T>(this ITypeConfigSyntax<T> syntax, byte value)
        {
            return syntax.TypeDefault(Parameter.NumberTextFiller, value);
        }

        // Type

        public static ITypeConfigSyntax<T> UseDelimitter<T>(this ITypeConfigSyntax<T> syntax, params byte[] value)
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
            syntax.Map(new MapFillerExpression(length, filler));
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

        // String

        public static IMapTextSyntax Text(this IMemberMapConfigSyntax syntax, int length)
        {
            var expression = new MapTextExpression(length);
            syntax.Map(expression);
            return expression;
        }

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
