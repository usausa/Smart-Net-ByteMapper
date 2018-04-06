namespace Smart.IO.Mapper
{
    using System;
    using Smart.IO.Mapper.Expressions;

    public static class ByteMapperConfigExpressionExtensions
    {
        //--------------------------------------------------------------------------------
        // ByteMapperConfig
        //--------------------------------------------------------------------------------

        public static ITypeConfigSyntax<T> MapByExpression<T>(this ByteMapperConfig config, int size)
        {
            return config.MapByExpression<T>(size, null);
        }

        public static ITypeConfigSyntax<T> MapByExpression<T>(this ByteMapperConfig config, int size, string profile)
        {
            var expression = new TypeMapExpression<T>(typeof(T), profile, size);
            config.AddMapping(expression);
            return expression;
        }

        public static ITypeConfigSyntax<object> MapByExpression(this ByteMapperConfig config, Type type, int size)
        {
            return config.MapByExpression(type, size, null);
        }

        public static ITypeConfigSyntax<object> MapByExpression(this ByteMapperConfig config, Type type, int size, string profile)
        {
            var expression = new TypeMapExpression<object>(type, profile, size);
            config.AddMapping(expression);
            return expression;
        }

        //--------------------------------------------------------------------------------
        // Type
        //--------------------------------------------------------------------------------

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

        // TODO Default

        // Const

        public static ITypeConfigSyntax<T> Constant<T>(this ITypeConfigSyntax<T> syntax, byte[] content)
        {
            syntax.AddTypeMapFactory(new MapConstantExpression(content));
            return syntax;
        }

        public static ITypeConfigSyntax<T> Constant<T>(this ITypeConfigSyntax<T> syntax, int offset, byte[] content)
        {
            syntax.AddTypeMapFactory(offset, new MapConstantExpression(content));
            return syntax;
        }

        // Filler

        public static ITypeConfigSyntax<T> Filler<T>(this ITypeConfigSyntax<T> syntax, int length)
        {
            syntax.AddTypeMapFactory(new MapFillerExpression(length));
            return syntax;
        }

        public static ITypeConfigSyntax<T> Filler<T>(this ITypeConfigSyntax<T> syntax, int length, byte filler)
        {
            syntax.AddTypeMapFactory(new MapFillerExpression(length));
            return syntax;
        }

        public static ITypeConfigSyntax<T> Filler<T>(this ITypeConfigSyntax<T> syntax, int offset, int length)
        {
            syntax.AddTypeMapFactory(offset, new MapFillerExpression(length));
            return syntax;
        }

        public static ITypeConfigSyntax<T> Filler<T>(this ITypeConfigSyntax<T> syntax, int offset, int length, byte filler)
        {
            syntax.AddTypeMapFactory(offset, new MapFillerExpression(length, filler));
            return syntax;
        }

        //--------------------------------------------------------------------------------
        // Member
        //--------------------------------------------------------------------------------

        // Binary

        public static void Binary(this IMemberMapConfigSyntax syntax)
        {
            syntax.SetMemberMapFactory(new MapBinaryExpression(null));
        }

        public static void Binary(this IMemberMapConfigSyntax syntax, Endian endian)
        {
            syntax.SetMemberMapFactory(new MapBinaryExpression(endian));
        }

        // Boolean

        public static IMapBooleanSyntax Boolean(this IMemberMapConfigSyntax syntax)
        {
            var expression = new MapBooleanExpression();
            syntax.SetMemberMapFactory(expression);
            return expression;
        }

        // 2
        // 3

        // TODO for MemberMap/Member
        // Byte
        // Bytes
        // DateTime
        // Number
        // String
    }
}
