namespace Smart.IO.Mapper.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Smart.ComponentModel;
    using Smart.IO.Mapper.Builders;
    using Smart.IO.Mapper.Helpers;
    using Smart.IO.Mapper.Mappers;
    using Smart.Reflection;

    internal sealed class AttributeMapping : IMapping
    {
        private readonly MapAttribute mapAttribute;

        private readonly bool validation;

        public Type Type { get; }

        public string Profile { get; }

        public int Size => mapAttribute.Size;

        public AttributeMapping(Type type, MapAttribute mapAttribute, string profile, bool validation)
        {
            Type = type;
            this.mapAttribute = mapAttribute;
            Profile = profile;
            this.validation = validation;
        }

        public IMapper[] CreateMappers(IComponentContainer components, IDictionary<string, object> parameters)
        {
            var context = new BuilderContext(
                components,
                parameters,
                Type.GetCustomAttributes().OfType<ITypeDefaultAttribute>().ToDictionary(x => x.Key, x => x.Value));

            var list = new List<MapperPosition>();
            list.AddRange(CreateTypeEntries(context));
            list.AddRange(CreateMemberEntries(context));

            MapperPositionHelper.Layout(
                list,
                mapAttribute.Size,
                Type.FullName,
                validation,
                mapAttribute.UseDelimitter ? context.GetParameter<byte[]>(Parameter.Delimiter) : null,
                mapAttribute.AutoFiller ? (byte?)context.GetParameter<byte>(Parameter.Filler) : null);

            return list.Select(x => x.Mapper).ToArray();
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
                        builder.CalcSize(Type),
                        builder.CreateMapper(context, Type));
                });
        }

        private IEnumerable<MapperPosition> CreateMemberEntries(IBuilderContext context)
        {
            var delegateFactory = context.Components.Get<IDelegateFactory>();

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

                        var elementBuilder = x.Attribute.GetConverterBuilder();
                        var elementConverter = elementBuilder.CreateConverter(context, elementType);
                        if (elementConverter == null)
                        {
                            throw new ByteMapperException(
                                "Attribute does not match property. " +
                                $"type=[{x.Property.DeclaringType?.FullName}], " +
                                $"property=[{x.Property.Name}], " +
                                $"attribute=[{x.Attribute.GetType().FullName}]");
                        }

                        var arrayBuilder = x.ArrayAttribute.GetArrayConverterBuilder();
                        arrayBuilder.ElementConverterBuilder = elementBuilder;

                        return new MapperPosition(
                            x.Attribute.Offset,
                            arrayBuilder.CalcSize(elementType),
                            new MemberMapper(
                                x.Attribute.Offset,
                                arrayBuilder.CreateConverter(context, x.Property.PropertyType),
                                delegateFactory.CreateGetter(x.Property),
                                delegateFactory.CreateSetter(x.Property)));
                    }

                    var builder = x.Attribute.GetConverterBuilder();
                    var converter = builder.CreateConverter(context, x.Property.PropertyType);
                    if (converter == null)
                    {
                        throw new ByteMapperException(
                            "Attribute does not match property. " +
                            $"type=[{x.Property.DeclaringType?.FullName}], " +
                            $"property=[{x.Property.Name}], " +
                            $"attribute=[{x.Attribute.GetType().FullName}]");
                    }

                    return new MapperPosition(
                        x.Attribute.Offset,
                        builder.CalcSize(x.Property.PropertyType),
                        new MemberMapper(
                            x.Attribute.Offset,
                            converter,
                            delegateFactory.CreateGetter(x.Property),
                            delegateFactory.CreateSetter(x.Property)));
                });
        }
    }
}
