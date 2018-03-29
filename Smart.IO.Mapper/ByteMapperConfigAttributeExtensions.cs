namespace Smart.IO.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Smart.IO.Mapper.Attributes;
    using Smart.IO.Mapper.Converters;
    using Smart.IO.Mapper.Mappings;
    using Smart.Reflection;

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
                    Attribute = x.GetCustomAttributes().OfType<IPropertyMappingAttribute>().FirstOrDefault(),
                    ArrayAttribute = x.GetCustomAttribute<ArrayAttribute>()
                })
                .Where(x => x.Attribute != null)
                .ToList();

            var parameters = type.GetCustomAttributes()
                .OfType<ITypeDefaultAttribute>()
                .ToDictionary(x => x.Key, x => x.Value);

            var entry = new MapEntry(
                type,
                mapAttribute.Size,
                parameters,
                context =>
                {
                    var typeEntries = typeAttributes
                        .Select(x => new
                        {
                            x.Offset,
                            Size = x.CalcSize(type),
                            Mapping = x.CreateMapping(context, type)
                        });

                    var propertyEntries = members
                        .Select(x =>
                        {
                            var delegateFactory = context.Components.Get<IDelegateFactory>();

                            if (x.ArrayAttribute != null)
                            {
                                if (!x.Property.PropertyType.IsArray)
                                {
                                    throw new ByteMapperException(
                                        "Attribute does not match property. " +
                                        $"type=[{x.Property.DeclaringType?.FullName}], " +
                                        $"property=[{x.Property.Name}], " +
                                        $"attribute=[{typeof(ArrayAttribute).FullName}]");
                                }

                                var elementType = x.Property.PropertyType.GetElementType();
                                var elementSize = x.Attribute.CalcSize(elementType);

                                var converter = x.Attribute.CreateConverter(context, elementType);
                                if (converter == null)
                                {
                                    throw new ByteMapperException(
                                        "Attribute does not match property. " +
                                        $"type=[{x.Property.DeclaringType?.FullName}], " +
                                        $"property=[{x.Property.Name}], " +
                                        $"attribute=[{x.Attribute.GetType().FullName}]");
                                }

                                return new
                                {
                                    x.Attribute.Offset,
                                    Size = elementSize * x.ArrayAttribute.Count,
                                    Mapping = new MemberMapping(
                                        x.Attribute.Offset,
                                        new ArrayConverter(
                                            delegateFactory.CreateArrayAllocator(elementType),
                                            x.ArrayAttribute.Count,
                                            elementSize,
                                            converter),
                                        delegateFactory.CreateGetter(x.Property),
                                        delegateFactory.CreateSetter(x.Property))
                                };
                            }
                            else
                            {
                                var converter = x.Attribute.CreateConverter(context, x.Property.PropertyType);
                                if (converter == null)
                                {
                                    throw new ByteMapperException(
                                        "Attribute does not match property. " +
                                        $"type=[{x.Property.DeclaringType?.FullName}], " +
                                        $"property=[{x.Property.Name}], " +
                                        $"attribute=[{x.Attribute.GetType().FullName}]");
                                }

                                return new
                                {
                                    x.Attribute.Offset,
                                    Size = x.Attribute.CalcSize(x.Property.PropertyType),
                                    Mapping = new MemberMapping(
                                        x.Attribute.Offset,
                                        converter,
                                        delegateFactory.CreateGetter(x.Property),
                                        delegateFactory.CreateSetter(x.Property))
                                };
                            }
                        });

                    // TODO (Terminatorの追加 実際の評価は最後？

                    // TODO.OrderBy(x => x.Attribute.Offset)

                    // TODO 重なり、範囲チェック 遅延評価が必要？
                    // TODO (Fillの追加

                    // TODO マージ＆OrderBy(x => x.Attribute.Offset)

                    return null;
                });

            config.AddMapEntry(Profile.Default, entry);
        }
    }
}
