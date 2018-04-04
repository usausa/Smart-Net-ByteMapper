namespace Smart.IO.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Smart.Collections.Generic;
    using Smart.ComponentModel;
    using Smart.IO.Mapper.Attributes;
    using Smart.IO.Mapper.Helpers;
    using Smart.IO.Mapper.Mappers;
    using Smart.Reflection;

    public static class ByteMapperConfigAttributeExtensions
    {
        public static ByteMapperConfig MapByAttribute<T>(this ByteMapperConfig config)
        {
            return config.MapByAttribute(typeof(T), null, true);
        }

        public static ByteMapperConfig MapByAttribute<T>(this ByteMapperConfig config, string profile)
        {
            return config.MapByAttribute(typeof(T), profile, true);
        }

        public static ByteMapperConfig MapByAttribute<T>(this ByteMapperConfig config, bool validate)
        {
            return config.MapByAttribute(typeof(T), null, validate);
        }

        public static ByteMapperConfig MapByAttribute<T>(this ByteMapperConfig config, string profile, bool validate)
        {
            return config.MapByAttribute(typeof(T), profile, validate);
        }

        public static ByteMapperConfig MapByAttribute(this ByteMapperConfig config, Type type)
        {
            return config.MapByAttribute(type, null, true);
        }

        public static ByteMapperConfig MapByAttribute(this ByteMapperConfig config, Type type, string profile)
        {
            return config.MapByAttribute(type, profile, true);
        }

        public static ByteMapperConfig MapByAttribute(this ByteMapperConfig config, Type type, bool validate)
        {
            return config.MapByAttribute(type, null, validate);
        }

        public static ByteMapperConfig MapByAttribute(this ByteMapperConfig config, Type type, string profile, bool validate)
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

            config.AddMapping(new AttributeMapping(type, mapAttribute, profile, validate));

            return config;
        }

        public static ByteMapperConfig MapByAttribute(this ByteMapperConfig config, IEnumerable<Type> types)
        {
            return MapByAttribute(config, types, null, true);
        }

        public static ByteMapperConfig MapByAttribute(this ByteMapperConfig config, IEnumerable<Type> types, string profile)
        {
            return MapByAttribute(config, types, profile, true);
        }

        public static ByteMapperConfig MapByAttribute(this ByteMapperConfig config, IEnumerable<Type> types, bool validate)
        {
            return MapByAttribute(config, types, null, validate);
        }

        public static ByteMapperConfig MapByAttribute(this ByteMapperConfig config, IEnumerable<Type> types, string profile, bool validate)
        {
            if (types == null)
            {
                throw new ArgumentNullException(nameof(types));
            }

            var targets = types
                .Where(x => x != null)
                .Select(x => new
                {
                    Type = x,
                    Attribute = x.GetCustomAttribute<MapAttribute>()
                })
                .Where(x => x.Attribute != null);
            foreach (var pair in targets)
            {
                config.AddMapping(new AttributeMapping(pair.Type, pair.Attribute, profile, validate));
            }

            return config;
        }

        internal class AttributeMapping : IMapping
        {
            private readonly MapAttribute mapAttribute;

            private readonly bool validate;

            public Type Type { get; }

            public string Profile { get; }

            public int Size => mapAttribute.Size;

            public AttributeMapping(Type type, MapAttribute mapAttribute, string profile, bool validate)
            {
                Type = type;
                this.mapAttribute = mapAttribute;
                Profile = profile;
                this.validate = validate;
            }

            public IMapper[] CreateMappers(IComponentContainer components, IDictionary<string, object> parameters)
            {
                var mappingParameter = new MappingParameter(
                    parameters,
                    Type.GetCustomAttributes().OfType<ITypeDefaultAttribute>().ToDictionary(x => x.Key, x => x.Value));

                var list = new List<MappingEntry>();
                list.AddRange(CreateTypeEntries(components, mappingParameter));
                list.AddRange(CreateMemberEntries(components, mappingParameter));

                if (mapAttribute.AutoDelimitter)
                {
                    var delimiter = mappingParameter.GetParameter<byte[]>(Parameter.Delimiter);
                    if ((delimiter != null) && (delimiter.Length > 0))
                    {
                        var offset = mapAttribute.Size - delimiter.Length;
                        list.Add(new MappingEntry(offset, delimiter.Length, new ConstMapper(offset, delimiter)));
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
                            $"type=[{Type.FullName}], " +
                            $"range=[{list[i].Offset}..{end}], " +
                            $"next=[{next}]");
                    }

                    if (mapAttribute.AutoFiller && (end < next))
                    {
                        if (!filler.HasValue)
                        {
                            filler = mappingParameter.GetParameter<byte>(Parameter.Filler);
                        }

                        var length = next - end;
                        fillers.Add(new MappingEntry(
                            end,
                            length,
                            new FillMapper(end, length, filler.Value)));
                    }
                }

                if (fillers.Count > 0)
                {
                    list.AddRange(fillers);
                    list.Sort(Comparer);
                }

                return list.Select(x => x.Mapper).ToArray();
            }

            private IEnumerable<MappingEntry> CreateTypeEntries(IComponentContainer components, IMappingParameter parameters)
            {
                return Type.GetCustomAttributes()
                    .OfType<ITypeMappingAttribute>()
                    .Select(x => new MappingEntry(
                        x.Offset,
                        x.CalcSize(Type),
                        x.CreateMapper(components, parameters, Type)));
            }

            private IEnumerable<MappingEntry> CreateMemberEntries(IComponentContainer components, IMappingParameter parameters)
            {
                return Type.GetProperties()
                    .Select(x => new
                    {
                        Property = x,
                        Attribute = x.GetCustomAttributes().OfType<IPropertyMappingAttribute>().FirstOrDefault(),
                        ArrayAttribute = x.GetCustomAttribute<MapArrayAttribute>()
                    })
                    .Where(x => x.Attribute != null)
                    .Select(x =>
                    {
                        var delegateFactory = components.Get<IDelegateFactory>();

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

                            var converter = x.Attribute.CreateConverter(components, parameters, elementType);
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
                                new MemberMapper(
                                    x.Attribute.Offset,
                                    x.ArrayAttribute.CreateArrayConverter(
                                        components,
                                        parameters,
                                        delegateFactory.CreateArrayAllocator(elementType),
                                        elementSize,
                                        converter),
                                    delegateFactory.CreateGetter(x.Property),
                                    delegateFactory.CreateSetter(x.Property)));
                        }
                        else
                        {
                            var converter = x.Attribute.CreateConverter(components, parameters, x.Property.PropertyType);
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
                                new MemberMapper(
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

                public IMapper Mapper { get; }

                public MappingEntry(int offset, int size, IMapper mapper)
                {
                    Offset = offset;
                    Size = size;
                    Mapper = mapper;
                }
            }
        }
    }
}
