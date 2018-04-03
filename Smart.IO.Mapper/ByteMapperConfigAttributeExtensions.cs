namespace Smart.IO.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Smart.Collections.Generic;
    using Smart.IO.Mapper.Attributes;
    using Smart.IO.Mapper.Mappings;
    using Smart.Reflection;

    public static class ByteMapperConfigAttributeExtensions
    {
        public static ByteMapperConfig MapByAttribute<T>(this ByteMapperConfig config)
        {
            return config.MapByAttribute(Profile.Default, typeof(T));
        }

        public static ByteMapperConfig MapByAttribute<T>(this ByteMapperConfig config, string profile)
        {
            return config.MapByAttribute(profile, true, typeof(T));
        }

        public static ByteMapperConfig MapByAttribute<T>(this ByteMapperConfig config, bool validate)
        {
            return config.MapByAttribute(Profile.Default, validate, typeof(T));
        }

        public static ByteMapperConfig MapByAttribute<T>(this ByteMapperConfig config, string profile, bool validate)
        {
            return config.MapByAttribute(profile, validate, typeof(T));
        }

        public static ByteMapperConfig MapByAttribute(this ByteMapperConfig config, Type type)
        {
            return config.MapByAttribute(Profile.Default, true, type);
        }

        public static ByteMapperConfig MapByAttribute(this ByteMapperConfig config, string profile, Type type)
        {
            return config.MapByAttribute(profile, true, type);
        }

        public static ByteMapperConfig MapByAttribute(this ByteMapperConfig config, bool validate, Type type)
        {
            return config.MapByAttribute(Profile.Default, validate, type);
        }

        public static ByteMapperConfig MapByAttribute(this ByteMapperConfig config, string profile, bool validate, Type type)
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

            MapInternal(config, profile, validate, type, mapAttribute);

            return config;
        }

        public static ByteMapperConfig MapByAttribute(this ByteMapperConfig config, IEnumerable<Type> types)
        {
            return MapByAttribute(config, Profile.Default, true, types);
        }

        public static ByteMapperConfig MapByAttribute(this ByteMapperConfig config, string profile, IEnumerable<Type> types)
        {
            return MapByAttribute(config, profile, true, types);
        }

        public static ByteMapperConfig MapByAttribute(this ByteMapperConfig config, bool validate, IEnumerable<Type> types)
        {
            return MapByAttribute(config, Profile.Default, validate, types);
        }

        public static ByteMapperConfig MapByAttribute(this ByteMapperConfig config, string profile, bool validate, IEnumerable<Type> types)
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
                MapInternal(config, profile, validate, pair.Type, pair.Attribute);
            }

            return config;
        }

        private static void MapInternal(ByteMapperConfig config, string profile, bool validate, Type type, MapAttribute mapAttribute)
        {
            var parameters = type.GetCustomAttributes()
                .OfType<ITypeDefaultAttribute>()
                .ToDictionary(x => x.Key, x => x.Value);

            var entry = new MapEntry(
                type,
                mapAttribute.Size,
                parameters,
                context =>
                {
                    var list = new List<MappingEntry>();
                    list.AddRange(CreateTypeEntries(context, type));
                    list.AddRange(CreateMemberEntries(context, type));

                    if (mapAttribute.AutoDelimitter)
                    {
                        var delimiter = context.GetParameter<byte[]>(Parameter.Delimiter);
                        if ((delimiter != null) && (delimiter.Length > 0))
                        {
                            var offset = mapAttribute.Size - delimiter.Length;
                            list.Add(new MappingEntry(offset, delimiter.Length, new ConstMapping(offset, delimiter)));
                        }
                    }

                    list.Sort(Comparer);

                    var filler = default(byte?);
                    var fillers = new List<MappingEntry>();
                    for (var i = 0; i < list.Count; i++)
                    {
                        var end = list[i].Offset + list[i].Size;
                        var next = i < list.Count - 1 ? list[i + 1].Offset : mapAttribute.Size;

                        if (validate && (end > next))
                        {
                            throw new ByteMapperException(
                                "Range overlap. " +
                                $"type=[{type.FullName}], " +
                                $"range=[{list[i].Offset}..{end}], " +
                                $"next=[{next}]");
                        }

                        if (mapAttribute.AutoFiller && (end < next))
                        {
                            if (!filler.HasValue)
                            {
                                filler = context.GetParameter<byte>(Parameter.Filler);
                            }

                            var length = next - end;
                            fillers.Add(new MappingEntry(
                                end,
                                length,
                                new FillMapping(end, length, filler.Value)));
                        }
                    }

                    if (fillers.Count > 0)
                    {
                        list.AddRange(fillers);
                        list.Sort(Comparer);
                    }

                    return list.Select(x => x.Mapping).ToArray();
                });

            config.AddMapEntry(profile ?? Profile.Default, entry);
        }

        private static IEnumerable<MappingEntry> CreateTypeEntries(IMappingCreateContext context, Type type)
        {
            return type.GetCustomAttributes()
                .OfType<ITypeMappingAttribute>()
                .Select(x => new MappingEntry(
                    x.Offset,
                    x.CalcSize(type),
                    x.CreateMapping(context, type)));
        }

        private static IEnumerable<MappingEntry> CreateMemberEntries(IMappingCreateContext context, Type type)
        {
            return type.GetProperties()
                .Select(x => new
                {
                    Property = x,
                    Attribute = x.GetCustomAttributes().OfType<IPropertyMappingAttribute>().FirstOrDefault(),
                    ArrayAttribute = x.GetCustomAttribute<MapArrayAttribute>()
                })
                .Where(x => x.Attribute != null)
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
                                $"attribute=[{typeof(MapArrayAttribute).FullName}]");
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

                        return new MappingEntry(
                            x.Attribute.Offset,
                            x.ArrayAttribute.CalcSize(elementSize),
                            new MemberMapping(
                                x.Attribute.Offset,
                                x.ArrayAttribute.CreateArrayConverter(
                                    context,
                                    delegateFactory.CreateArrayAllocator(elementType),
                                    elementSize,
                                    converter),
                                delegateFactory.CreateGetter(x.Property),
                                delegateFactory.CreateSetter(x.Property)));
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

                        return new MappingEntry(
                            x.Attribute.Offset,
                            x.Attribute.CalcSize(x.Property.PropertyType),
                            new MemberMapping(
                                x.Attribute.Offset,
                                converter,
                                delegateFactory.CreateGetter(x.Property),
                                delegateFactory.CreateSetter(x.Property)));
                    }
                });
        }

        private static readonly IComparer<MappingEntry> Comparer = Comparers.Delegate<MappingEntry>((x, y) => x.Offset - y.Offset);

        internal class MappingEntry
        {
            public int Offset { get; }

            public int Size { get; }

            public IMapping Mapping { get; }

            public MappingEntry(int offset, int size, IMapping mapping)
            {
                Offset = offset;
                Size = size;
                Mapping = mapping;
            }
        }
    }
}
