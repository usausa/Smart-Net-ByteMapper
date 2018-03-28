namespace Smart.IO.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Smart.IO.Mapper.Attributes;

    public static class ByteMapperConfigAttributeExtensions
    {
        private static readonly string[] DefaultProfiles = { string.Empty };

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
            var members = type.GetProperties()
                .Select(x => new
                {
                    Property = x,
                    Attribute = x.GetCustomAttributes().OfType<IPropertyMappingAttribute>().FirstOrDefault(),
                    ArrayAttribute = x.GetCustomAttribute<ArrayAttribute>()
                })
                .ToList();

            foreach (var member in members)
            {
                if (!member.Attribute.Match(member.Property))
                {
                    throw new InvalidOperationException(
                        "Property attribute unmatch. " +
                        $"type=[{type.FullName}], " +
                        $"property=[{member.Property.Name}], " +
                        $"attribute=[{member.Attribute.GetType().FullName}]");
                }
            }

            var typeAttributes = type.GetCustomAttributes()
                .OfType<IMappingAttribute>()
                .ToList();

            var profiles = DefaultProfiles
                .Concat(mapAttribute.FillProfiles ?? Empty<string>.Array)
                .Concat(typeAttributes
                    .Concat(members.Select(x => x.Attribute))
                    .SelectMany(x => x.Profiles ?? Empty<string>.Array))
                .Distinct();

            var parameters = type.GetCustomAttributes()
                .OfType<ITypeDefaultAttribute>()
                .ToDictionary(x => x.Key, x => x.Value);

            // TODO.OrderBy(x => x.Attribute.Offset)
            // TODO (Array 優先？,計算に必要)
            // TODO 重なり、範囲チェック
            // TODO (Fillの追加
            // TODO (Terminatorの追加 実際の評価は最後？

            // TODO propを渡して！ FactoryのFuncを返す？
            foreach (var profile in profiles)
            {
                var entry = new MapEntry(
                    type,
                    mapAttribute.Size,
                    parameters,
                    null);  // TODO ここも遅延評価にして?

                config.AddMapEntry(profile, entry);
            }
        }
    }
}
