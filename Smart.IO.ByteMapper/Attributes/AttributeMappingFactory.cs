namespace Smart.IO.ByteMapper.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Smart.ComponentModel;
    using Smart.IO.ByteMapper.Builders;
    using Smart.IO.ByteMapper.Helpers;
    using Smart.IO.ByteMapper.Mappers;
    using Smart.Reflection;

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

        IMapping IMappingFactory.Create(IComponentContainer components, IDictionary<string, object> parameters)
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
                mapAttribute.UseDelimitter ? context.GetParameter<byte[]>(Parameter.Delimiter) : null,
                mapAttribute.AutoFiller ? (byte?)filler : null);

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
                        var arrayBuilder = x.ArrayAttribute.GetArrayConverterBuilder();
                        arrayBuilder.ElementConverterBuilder = x.Attribute.GetConverterBuilder();

                        if (!arrayBuilder.Match(x.Property.PropertyType))
                        {
                            throw new ByteMapperException(
                                "Attribute does not match property. " +
                                $"type=[{x.Property.DeclaringType.FullName}], " +
                                $"property=[{x.Property.Name}], " +
                                $"attribute=[{typeof(MapArrayAttribute).FullName}]");
                        }

                        return new MapperPosition(
                            x.Attribute.Offset,
                            arrayBuilder.CalcSize(x.Property.PropertyType),
                            new MemberMapper(
                                x.Attribute.Offset,
                                arrayBuilder.CreateConverter(context, x.Property.PropertyType),
                                delegateFactory.CreateGetter(x.Property),
                                delegateFactory.CreateSetter(x.Property)));
                    }

                    var builder = x.Attribute.GetConverterBuilder();
                    if (!builder.Match(x.Property.PropertyType))
                    {
                        throw new ByteMapperException(
                            "Attribute does not match property. " +
                            $"type=[{x.Property.DeclaringType.FullName}], " +
                            $"property=[{x.Property.Name}], " +
                            $"attribute=[{x.Attribute.GetType().FullName}]");
                    }

                    return new MapperPosition(
                        x.Attribute.Offset,
                        builder.CalcSize(x.Property.PropertyType),
                        new MemberMapper(
                            x.Attribute.Offset,
                            builder.CreateConverter(context, x.Property.PropertyType),
                            delegateFactory.CreateGetter(x.Property),
                            delegateFactory.CreateSetter(x.Property)));
                });
        }
    }
}
