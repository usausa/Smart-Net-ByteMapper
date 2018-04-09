namespace Smart.IO.Mapper.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using Smart.ComponentModel;
    using Smart.IO.Mapper.Builders;
    using Smart.IO.Mapper.Converters;
    using Smart.IO.Mapper.Helpers;
    using Smart.IO.Mapper.Mappers;

    internal class TypeConfig<T> : ITypeConfigSyntax<T>, IMapping
    {
        private readonly Dictionary<string, object> typeParameters = new Dictionary<string, object>();

        private readonly List<TypeMapEntry> typeMapEntries = new List<TypeMapEntry>();

        private readonly List<MemberMapEntry> memberMapEntries = new List<MemberMapEntry>();

        private bool autoFiller = true;

        private bool useDelimitter = true;

        private int lastOffset;

        public Type Type { get; }

        public string Profile { get; }

        public int Size { get; }

        public TypeConfig(Type type, string profile, int size)
        {
            Type = type;
            Profile = profile;
            Size = size;
        }

        // Type default

        public ITypeConfigSyntax<T> TypeDefault(string key, object value)
        {
            typeParameters[key] = value;
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

        // Mapper

        public ITypeConfigSyntax<T> Map(ITypeMapExpression expression)
        {
            return Map(lastOffset, expression);
        }

        public ITypeConfigSyntax<T> Map(int offset, ITypeMapExpression expression)
        {
            var builder = expression.GetTypeMapperBuilder();
            typeMapEntries.Add(new TypeMapEntry(offset, builder.CalcSize(Type), builder));

            // TODO size
            return this;
        }

        // ForMember

        public ITypeConfigSyntax<T> ForMember(string name, Action<IMemberConfigSyntax> config)
        {
            return ForMember(name, lastOffset, config);
        }

        public ITypeConfigSyntax<T> ForMember(string name, int offset, Action<IMemberConfigSyntax> config)
        {
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

            var member = new MemberConfig();
            config(member);

            if (member.Expression == null)
            {
                throw new InvalidOperationException("Property is not mapped.");
            }

            // TODO array
            var convertBuilder = member.Expression.GetMapConverterBuilder();
            var entry = new MemberMapEntry(pi.PropertyType,  offset, convertBuilder.CalcSize(pi.PropertyType), convertBuilder);
            memberMapEntries.Add(entry);

            lastOffset = Math.Max(offset, lastOffset) + entry.Size;

            return this;
        }

        public ITypeConfigSyntax<T> ForMember(Expression<Func<T, object>> expr, Action<IMemberConfigSyntax> config)
        {
            return ForMember(ExpressionHelper.GetMemberName(expr), lastOffset, config);
        }

        public ITypeConfigSyntax<T> ForMember(Expression<Func<T, object>> expr, int offset, Action<IMemberConfigSyntax> config)
        {
            return ForMember(ExpressionHelper.GetMemberName(expr), offset, config);
        }

        public IMapper[] CreateMappers(IComponentContainer components, IDictionary<string, object> parameters)
        {
            throw new NotImplementedException();
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
            public Type Type { get; }

            public int Offset { get; }

            public int Size { get; }

            public IMapConverterBuilder Builder { get; }

            public MemberMapEntry(Type type, int offset, int size, IMapConverterBuilder builder)
            {
                Offset = offset;
                Size = size;
                Builder = builder;
            }
        }
    }
}
