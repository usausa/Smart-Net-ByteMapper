namespace Smart.IO.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Smart.Collections.Generic;
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
            var typeAttributes = type.GetCustomAttributes()
                .OfType<ITypeMappingAttribute>()
                .ToList();

            var members = type.GetProperties()
                .Select(x => new
                {
                    Property = x,
                    Attributes = x.GetCustomAttributes().OfType<IPropertyMappingAttribute>().ToArray(),
                    ArrayAttributes = x.GetCustomAttributes<ArrayAttribute>().ToArray()
                })
                .ToList();

            // Validation
            foreach (var entry in typeAttributes
                .SelectMany(attr => attr.Profiles.EmptyIfNull()
                    .Select(profile => new { Attribute = attr, Profile = profile })))
            {
                if (!mapAttribute.Profiles.Contains(entry.Profile))
                {
                    throw new ByteMapperException(
                        "Profile not exists in MapAttribute. " +
                        $"type=[{type.FullName}], " +
                        $"attribute=[{entry.Attribute.GetType().FullName}], " +
                        $"profile=[{entry.Profile}]");
                }
            }

            foreach (var entry in members
                .SelectMany(x => x.Attributes
                    .SelectMany(attr => attr.Profiles.EmptyIfNull()
                        .Select(profile => new { x.Property, Attribute = (Attribute)attr, Profile = profile })))
                .Concat(members
                    .SelectMany(x => x.ArrayAttributes
                        .SelectMany(attr => attr.Profiles.EmptyIfNull()
                            .Select(profile => new { x.Property, Attribute = (Attribute)attr, Profile = profile })))))
            {
                if (!mapAttribute.Profiles.Contains(entry.Profile))
                {
                    throw new ByteMapperException(
                        "Profile not exists in MapAttribute. " +
                        $"type=[{type.FullName}], " +
                        $"property=[{entry.Property.Name}], " +
                        $"attribute=[{entry.Attribute.GetType().FullName}], " +
                        $"profile=[{entry.Profile}]");
                }
            }

            var parameters = type.GetCustomAttributes()
                .OfType<ITypeDefaultAttribute>()
                .ToDictionary(x => x.Key, x => x.Value);

            foreach (var profile in mapAttribute.Profiles)
            {
                var entry = new MapEntry(
                    type,
                    mapAttribute.Size,
                    parameters,
                    context =>
                    {
                        var typeMappings = typeAttributes
                            .Where(x => MatchProfile(x.Profiles, profile))
                            .Select(x => new { x.Offset, Size = x.CalcSize(type), Mapping = x.CreateMapping(context, type) });

                        //var propertyMappings

                        // TODO.OrderBy(x => x.Attribute.Offset)
                        // TODO Array 優先？,計算に必要)
                        // TODO 重なり、範囲チェック 遅延評価が必要？
                        // TODO (Fillの追加
                        // TODO (Terminatorの追加 実際の評価は最後？
                        // TODO 改行もわかるのはランタイム、＝全体もわかるのはランタイム！？

                        return null;
                    });

                config.AddMapEntry(profile, entry);
            }
        }

        private static bool MatchProfile(string[] profiles, string profile)
        {
            return profiles == null ||
                   profiles.Length == 0 ||
                   profiles.Contains(profile);
        }
    }
}
