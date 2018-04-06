namespace Smart.IO.Mapper
{
    using System;

    using Smart.IO.Mapper.Expressions;

    public static class ByteMapperConfigExpressionExtensions
    {
        //--------------------------------------------------------------------------------
        // ByteMapperConfig
        //--------------------------------------------------------------------------------

        public static ITypeConfigSyntax<T> CreateMapByExpression<T>(this ByteMapperConfig config, int size)
        {
            return config.CreateMapByExpression<T>(size, null);
        }

        public static ITypeConfigSyntax<T> CreateMapByExpression<T>(this ByteMapperConfig config, int size, string profile)
        {
            var expression = new TypeMapExpression<T>(typeof(T), profile, size);
            config.AddMapping(expression);
            return expression;
        }

        public static ITypeConfigSyntax<object> CreateMapByExpression(this ByteMapperConfig config, Type type, int size)
        {
            return config.CreateMapByExpression(type, size, null);
        }

        public static ITypeConfigSyntax<object> CreateMapByExpression(this ByteMapperConfig config, Type type, int size, string profile)
        {
            var expression = new TypeMapExpression<object>(type, profile, size);
            config.AddMapping(expression);
            return expression;
        }

        //--------------------------------------------------------------------------------
        // Type
        //--------------------------------------------------------------------------------

        // Default

        // TODO    //ITypeConfigSyntax<T> UseFiller(byte value);
        // TODO    //ITypeConfigSyntax<T> UseDelimitter(params byte[] delimitter);

        //    //public ITypeConfigSyntax<T> UseDelimitter(params byte[] delimitter)
        //    //{
        //    //    if ((delimitter != null) && (delimitter.Length > 0))
        //    //    {
        //    //        useDelimitter = true;
        //    //        parameters[Parameter.Delimiter] = delimitter;
        //    //    }
        //    //    else
        //    //    {
        //    //        useDelimitter = false;
        //    //    }

        //    //    return this;
        //    //}

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
            var expression = new MapDateTimeExpression();
            syntax.Map(expression);
            return expression;
        }

        // Number TODO formatオプション版？

        public static IMapNumberSyntax Number(this IMemberMapConfigSyntax syntax, int length)
        {
            var expression = new MapNumberExpression();
            syntax.Map(expression);
            return expression;
        }

        // String

        public static IMapTextSyntax Text(this IMemberMapConfigSyntax syntax, int length)
        {
            var expression = new MapTextExpression();
            syntax.Map(expression);
            return expression;
        }
    }
}
