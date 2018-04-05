namespace Smart.IO.Mapper
{
    using System;
    using Smart.IO.Mapper.Expressions;

    public static class ByteMapperConfigExpressionExtensions
    {
        //--------------------------------------------------------------------------------
        // ByteMapperConfig
        //--------------------------------------------------------------------------------

        public static ITypeConfigSyntax<T> MapByExpression<T>(this ByteMapperConfig config)
        {
            var builder = new MapBuilder<T>(typeof(T));
            config.AddMapping(builder);
            return builder;
        }

        public static ITypeConfigSyntax<object> MapByExpression(this ByteMapperConfig config, Type type)
        {
            // TODO ?
            var builder = new MapBuilder<object>(type);
            config.AddMapping(builder);
            return builder;
        }

        public static IByteMapperConfig MapByExpression<T>(this ByteMapperConfig config, Action<ITypeConfigSyntax<T>> action)
        {
            var builder = new MapBuilder<T>(typeof(T));
            action(builder);
            config.AddMapping(builder);
            return config;
        }

        public static IByteMapperConfig MapByExpression(this ByteMapperConfig config, Type type, Action<ITypeConfigSyntax<object>> action)
        {
            // TODO ?
            var builder = new MapBuilder<object>(type);
            action(builder);
            config.AddMapping(builder);
            return config;
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
            syntax.AddMapper(new MapConstantExpression(content));
            return syntax;
        }

        public static ITypeConfigSyntax<T> Constant<T>(this ITypeConfigSyntax<T> syntax, int offset, byte[] content)
        {
            syntax.AddMapper(offset, new MapConstantExpression(content));
            return syntax;
        }

        // Filler

        public static ITypeConfigSyntax<T> Filler<T>(this ITypeConfigSyntax<T> syntax, int length)
        {
            syntax.AddMapper(new MapFillerExpression(length));
            return syntax;
        }

        public static ITypeConfigSyntax<T> Filler<T>(this ITypeConfigSyntax<T> syntax, int length, byte filler)
        {
            syntax.AddMapper(new MapFillerExpression(length));
            return syntax;
        }

        public static ITypeConfigSyntax<T> Filler<T>(this ITypeConfigSyntax<T> syntax, int offset, int length)
        {
            syntax.AddMapper(offset, new MapFillerExpression(length));
            return syntax;
        }

        public static ITypeConfigSyntax<T> Filler<T>(this ITypeConfigSyntax<T> syntax, int offset, int length, byte filler)
        {
            syntax.AddMapper(offset, new MapFillerExpression(length, filler));
            return syntax;
        }

        //--------------------------------------------------------------------------------
        // Member
        //--------------------------------------------------------------------------------

        // TODO for MemberMap/Member

        // Array
        // Binary
        // Bool
        // Byte
        // Bytes
        // DateTime
        // Number
        // String
    }
}
