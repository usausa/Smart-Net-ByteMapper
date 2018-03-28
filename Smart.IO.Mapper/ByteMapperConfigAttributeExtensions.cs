namespace Smart.IO.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Smart.IO.Mapper.Attributes;

    public static class ByteMapperConfigAttributeExtensions
    {
        public static ByteMapperConfig Map<T>(this ByteMapperConfig config)
        {
            return config.Map(typeof(T));
        }

        public static ByteMapperConfig Map(this ByteMapperConfig config, Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var mapAttribute = type.GetCustomAttribute<MapAttribute>();
            if (mapAttribute == null)
            {
                throw new ArgumentException($"No MapAttribute. type=[{type.FullName}]", nameof(type));
            }

            MapInternal(config, type, mapAttribute);

            return config;
        }

        public static ByteMapperConfig Map(this ByteMapperConfig config, IEnumerable<Type> types)
        {
            if (types == null)
            {
                throw new ArgumentNullException(nameof(types));
            }

            var targets = types
                .Select(x => new
                {
                    Type = x,
                    Attribute = x.GetCustomAttribute<MapAttribute>()
                })
                .Where(x => x.Attribute != null);
            foreach (var pair in targets)
            {
                MapInternal(config, pair.Type, pair.Attribute);
            }

            return config;
        }

        private static void MapInternal(ByteMapperConfig config, Type type, MapAttribute mapAttribute)
        {
            var parameters = type.GetCustomAttributes()
                .OfType<ITypeDefaultAttribute>()
                .ToDictionary(x => x.Key, x => x.Value);

            var members = type.GetProperties()
                .Select(x => new
                {
                    Property = x,
                    Attribute = x.GetCustomAttributes().OfType<IPropertyAttribute>().FirstOrDefault()
                })
                .OrderBy(x => x.Attribute.Offset)
                .ToList();

            // TODO constraint ?
            // TODO profiles単位
            // TODO Fillの追加
            // TODO Array 優先？
            // TODO propを渡して！ FactoryのFuncを返す？

            var entry = new MapEntry(
                type,
                mapAttribute.Size,
                parameters,
                null);  // TODO

            config.AddMapEntry(string.Empty, entry);
        }
    }
}
