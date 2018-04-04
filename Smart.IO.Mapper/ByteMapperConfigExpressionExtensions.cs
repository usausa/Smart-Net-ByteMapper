namespace Smart.IO.Mapper
{
    using System;
    using Smart.IO.Mapper.Expressions;

    public static class ByteMapperConfigExpressionExtensions
    {
        public static ITypeSyntax<T> MapByExpression<T>(this ByteMapperConfig config)
        {
            var builder = new MapBuilder<T>(typeof(T));
            config.AddMapping(builder);
            return builder;
        }

        public static ITypeSyntax MapByExpression(this ByteMapperConfig config, Type type)
        {
            // TODO ?
            var builder = new MapBuilder<object>(type);
            config.AddMapping(builder);
            return builder;
        }

        public static IByteMapperConfig MapByExpression<T>(this ByteMapperConfig config, Action<ITypeSyntax<T>> action)
        {
            var builder = new MapBuilder<T>(typeof(T));
            action(builder);
            config.AddMapping(builder);
            return config;
        }

        public static IByteMapperConfig MapByExpression(this ByteMapperConfig config, Type type, Action<ITypeSyntax> action)
        {
            // TODO ?
            var builder = new MapBuilder<object>(type);
            action(builder);
            config.AddMapping(builder);
            return config;
        }
    }
}
