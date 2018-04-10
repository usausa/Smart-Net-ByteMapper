namespace Smart.IO.Mapper.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Smart.ComponentModel;
    using Smart.IO.Mapper.Builders;
    using Smart.IO.Mapper.Helpers;
    using Smart.IO.Mapper.Mappers;
    using Smart.Reflection;

    internal sealed class TypeConfigExpression<T> : ITypeConfigSyntax<T>, IMapping
    {
        private readonly List<TypeMapEntry> typeMapEntries = new List<TypeMapEntry>();

        private readonly List<MemberMapEntry> memberMapEntries = new List<MemberMapEntry>();

        private readonly Dictionary<string, object> typeParameters = new Dictionary<string, object>();

        private bool validation = true;

        private bool autoFiller = true;

        private bool useDelimitter = true;

        private int lastOffset;

        public Type Type { get; }

        public string Profile { get; }

        public int Size { get; }

        public TypeConfigExpression(Type type, string profile, int size)
        {
            Type = type;
            Profile = profile;
            Size = size;
        }

        // Type setting

        public ITypeConfigSyntax<T> WithValidation(bool value)
        {
            validation = value;
            return this;
        }

        // Type setting

        public ITypeConfigSyntax<T> AutoFiller(bool value)
        {
            autoFiller = value;
            return this;
        }

        public ITypeConfigSyntax<T> UseDelimitter(bool value)
        {
            useDelimitter = value;
            return this;
        }

        // Type default

        public ITypeConfigSyntax<T> TypeDefault(string key, object value)
        {
            typeParameters[key] = value;
            return this;
        }

        // Mapper

        ITypeConfigSyntax<T> ITypeConfigSyntax<T>.Map(ITypeMapExpression expression)
        {
            return MapInternal(lastOffset, expression);
        }

        ITypeConfigSyntax<T> ITypeConfigSyntax<T>.Map(int offset, ITypeMapExpression expression)
        {
            return MapInternal(offset, expression);
        }

        private ITypeConfigSyntax<T> MapInternal(int offset, ITypeMapExpression expression)
        {
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            var builder = expression.GetTypeMapperBuilder();
            var entry = new TypeMapEntry(offset, builder.CalcSize(Type), builder);
            typeMapEntries.Add(entry);

            lastOffset = Math.Max(offset, lastOffset) + entry.Size;

            return this;
        }

        // ForMember

        public ITypeConfigSyntax<T> ForMember(string name, Action<IMemberConfigSyntax> config)
        {
            return ForMemberInternal(name, lastOffset, config);
        }

        public ITypeConfigSyntax<T> ForMember(string name, int offset, Action<IMemberConfigSyntax> config)
        {
            return ForMemberInternal(name, offset, config);
        }

        public ITypeConfigSyntax<T> ForMember(Expression<Func<T, object>> expr, Action<IMemberConfigSyntax> config)
        {
            return ForMemberInternal(ExpressionHelper.GetMemberName(expr), lastOffset, config);
        }

        public ITypeConfigSyntax<T> ForMember(Expression<Func<T, object>> expr, int offset, Action<IMemberConfigSyntax> config)
        {
            return ForMemberInternal(ExpressionHelper.GetMemberName(expr), offset, config);
        }

        private ITypeConfigSyntax<T> ForMemberInternal(string name, int offset, Action<IMemberConfigSyntax> config)
        {
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var type = typeof(T);
            var pi = type.GetProperty(name);
            if (pi == null)
            {
                throw new ArgumentException("Name is invalid.", nameof(name));
            }

            var member = new MemberConfigExpression();
            config(member);

            if (member.Expression == null)
            {
                throw new InvalidOperationException("Property is not mapped.");
            }

            var builder = member.Expression.GetMapConverterBuilder();
            var entry = new MemberMapEntry(pi, offset, builder.CalcSize(pi.PropertyType), builder);
            memberMapEntries.Add(entry);

            lastOffset = Math.Max(offset, lastOffset) + entry.Size;

            return this;
        }

        IMapper[] IMapping.CreateMappers(IComponentContainer components, IDictionary<string, object> parameters)
        {
            var context = new BuilderContext(components, parameters, typeParameters);

            var list = new List<MapperPosition>();
            list.AddRange(CreateTypeEntries(context));
            list.AddRange(CreateMemberEntries(context));

            MapperPositionHelper.Layout(
                list,
                Size,
                Type.FullName,
                validation,
                useDelimitter ? context.GetParameter<byte[]>(Parameter.Delimiter) : null,
                autoFiller ? (byte?)context.GetParameter<byte>(Parameter.Filler) : null);

            return list.Select(x => x.Mapper).ToArray();
        }

        private IEnumerable<MapperPosition> CreateTypeEntries(IBuilderContext context)
        {
            return typeMapEntries
                .Select(x => new MapperPosition(
                    x.Offset,
                    x.Size,
                    x.Builder.CreateMapper(context, Type)));
        }

        private IEnumerable<MapperPosition> CreateMemberEntries(IBuilderContext context)
        {
            var delegateFactory = context.Components.Get<IDelegateFactory>();

            return memberMapEntries
                .Select(x => new MapperPosition(
                    x.Offset,
                    x.Size,
                    new MemberMapper(
                        x.Offset,
                        x.Builder.CreateConverter(context, x.Property.PropertyType),
                        delegateFactory.CreateGetter(x.Property),
                        delegateFactory.CreateSetter(x.Property))));
        }

        private class TypeMapEntry
        {
            public int Offset { get; }

            public int Size { get; }

            public ITypeMapperBuilder Builder { get; }

            public TypeMapEntry(int offset, int size, ITypeMapperBuilder builder)
            {
                Offset = offset;
                Size = size;
                Builder = builder;
            }
        }

        private class MemberMapEntry
        {
            public PropertyInfo Property { get; }

            public int Offset { get; }

            public int Size { get; }

            public IMapConverterBuilder Builder { get; }

            public MemberMapEntry(PropertyInfo property, int offset, int size, IMapConverterBuilder builder)
            {
                Property = property;
                Offset = offset;
                Size = size;
                Builder = builder;
            }
        }
    }
}
