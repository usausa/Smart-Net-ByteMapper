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

        // Const

        public static ITypeConfigSyntax<T> Const<T>(this ITypeConfigSyntax<T> syntax, byte[] content)
        {
            syntax.AddMapper(new ConstMapBuilder(content));
            return syntax;
        }

        public static ITypeConfigSyntax<T> Const<T>(this ITypeConfigSyntax<T> syntax, int offset, byte[] content)
        {
            syntax.AddMapper(offset, new ConstMapBuilder(content));
            return syntax;
        }

        // Filler

        public static ITypeConfigSyntax<T> Filler<T>(this ITypeConfigSyntax<T> syntax, int length)
        {
            syntax.AddMapper(new FillerMapBuilder(length));
            return syntax;
        }

        public static ITypeConfigSyntax<T> Filler<T>(this ITypeConfigSyntax<T> syntax, int length, byte filler)
        {
            syntax.AddMapper(new FillerMapBuilder(length));
            return syntax;
        }

        public static ITypeConfigSyntax<T> Filler<T>(this ITypeConfigSyntax<T> syntax, int offset, int length)
        {
            syntax.AddMapper(offset, new FillerMapBuilder(length));
            return syntax;
        }

        public static ITypeConfigSyntax<T> Filler<T>(this ITypeConfigSyntax<T> syntax, int offset, int length, byte filler)
        {
            syntax.AddMapper(offset, new FillerMapBuilder(length, filler));
            return syntax;
        }

        //--------------------------------------------------------------------------------
        // Property
        //--------------------------------------------------------------------------------

        // TODO for PropetyMap/Property

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
