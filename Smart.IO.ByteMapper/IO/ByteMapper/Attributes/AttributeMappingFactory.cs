namespace Smart.IO.ByteMapper.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Smart.ComponentModel;
    using Smart.IO.ByteMapper.Builders;
    using Smart.IO.ByteMapper.Helpers;

    internal sealed class AttributeMappingFactory : IMappingFactory
    {
        private readonly MapAttribute mapAttribute;

        private readonly bool validation;

        public Type Type { get; }

        public string Name { get; }

        public AttributeMappingFactory(Type type, MapAttribute mapAttribute, string name, bool validation)
        {
            Type = type;
            this.mapAttribute = mapAttribute;
            Name = name;
            this.validation = validation;
        }

        IMapping IMappingFactory.Create(ComponentContainer components, IDictionary<string, object> parameters)
        {
            var context = new BuilderContext(
                components,
                parameters,
                Type.GetCustomAttributes().OfType<ITypeDefaultAttribute>().ToDictionary(x => x.Key, x => x.Value));

            var filler = context.GetParameter<byte>(Parameter.Filler);

            var list = new List<MapperPosition>();
            list.AddRange(CreateTypeEntries(context));
            list.AddRange(CreateMemberEntries(context));

            MapperPositionHelper.Layout(
                list,
                mapAttribute.Size,
                Type.FullName,
                validation,
                mapAttribute.UseDelimiter ? context.GetParameter<byte[]>(Parameter.Delimiter) : null,
                mapAttribute.AutoFiller ? filler : null);

            return new Mapping(
                Type,
                mapAttribute.Size,
                mapAttribute.HasNullFiller ? mapAttribute.NullFiller : filler,
                list.Select(x => x.Mapper).ToArray());
        }

        private IEnumerable<MapperPosition> CreateTypeEntries(IBuilderContext context)
        {
            return Type.GetCustomAttributes()
                .OfType<AbstractTypeMapAttribute>()
                .Select(x =>
                {
                    var builder = x.GetTypeMapperBuilder();
                    return new MapperPosition(
                        builder.Offset,
                        builder.CalcSize(),
                        builder.CreateMapper(context));
                });
        }

        private IEnumerable<MapperPosition> CreateMemberEntries(IBuilderContext context)
        {
            return Type.GetProperties()
                .Select(x => new
                {
                    Property = x,
                    Attribute = x.GetCustomAttributes().OfType<AbstractMemberMapAttribute>().FirstOrDefault(),
                    ArrayAttribute = x.GetCustomAttribute<MapArrayAttribute>()
                })
                .Where(x => x.Attribute != null)
                .Select(x =>
                {
                    var converterBuilder = CreateConverterBuilder(x.ArrayAttribute, x.Attribute);
                    if (!converterBuilder.Match(x.Property.PropertyType))
                    {
                        throw new ByteMapperException(
                            "Attribute does not match property. " +
                            $"type=[{x.Property.DeclaringType.FullName}], " +
                            $"property=[{x.Property.Name}], " +
                            $"attribute=[{typeof(MapArrayAttribute).FullName}]");
                    }

                    var builder = new MemberMapperBuilder(converterBuilder)
                    {
                        Offset = x.Attribute.Offset,
                        Property = x.Property
                    };

                    return new MapperPosition(builder.Offset, builder.CalcSize(), builder.CreateMapper(context));
                });
        }

        private static IMapConverterBuilder CreateConverterBuilder(
            MapArrayAttribute arrayAttribute,
            AbstractMemberMapAttribute memberAttribute)
        {
            if (arrayAttribute is null)
            {
                return memberAttribute.GetConverterBuilder();
            }

            var builder = arrayAttribute.GetArrayConverterBuilder();
            builder.ElementConverterBuilder = memberAttribute.GetConverterBuilder();
            return builder;
        }
    }
}
