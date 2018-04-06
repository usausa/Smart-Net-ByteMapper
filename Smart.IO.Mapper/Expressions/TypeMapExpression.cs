namespace Smart.IO.Mapper.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using Smart.ComponentModel;
    using Smart.IO.Mapper.Helpers;
    using Smart.IO.Mapper.Mappers;

    internal class TypeMapExpression<T> : ITypeConfigSyntax<T>, IMapping
    {
        private readonly Dictionary<string, object> typeParameters = new Dictionary<string, object>();

        private readonly List<TypeMapEntry> typeMapEntries = new List<TypeMapEntry>();

        private readonly List<MemberMapEntry> memberMapEntries = new List<MemberMapEntry>();

        private bool useFiller = true;

        private bool useDelimitter = true;

        private int lastOffset;

        public Type Type { get; }

        public string Profile { get; }

        public int Size { get; }

        public TypeMapExpression(Type type, string profile, int size)
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

        public ITypeConfigSyntax<T> UseFiller(bool value)
        {
            useFiller = value;
            return this;
        }

        public ITypeConfigSyntax<T> UseDelimitter(bool value)
        {
            useDelimitter = value;
            return this;
        }

        // Mapper

        public ITypeConfigSyntax<T> AddTypeMapFactory(ITypeMapFactory factory)
        {
            return AddTypeMapFactory(lastOffset, factory);
        }

        public ITypeConfigSyntax<T> AddTypeMapFactory(int offset, ITypeMapFactory factory)
        {
            typeMapEntries.Add(new TypeMapEntry(offset, factory.CalcSize(typeof(T)), factory));
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

            var builder = new MemberMapExpression();
            config(builder);

            if (builder.Factory == null)
            {
                throw new InvalidOperationException("Property is not mapped.");
            }

            // TODO array
            var entry = new MemberMapEntry(offset, builder.Factory.CalcSize(pi.PropertyType), builder.Factory);
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

            public ITypeMapFactory Factory { get; }

            public TypeMapEntry(int offset, int size, ITypeMapFactory factory)
            {
                Offset = offset;
                Size = size;
                Factory = factory;
            }
        }

        private class MemberMapEntry
        {
            public int Offset { get; }

            public int Size { get; }

            public IMemberMapFactory Factory { get; }

            public MemberMapEntry(int offset, int size, IMemberMapFactory factory)
            {
                Offset = offset;
                Size = size;
                Factory = factory;
            }
        }
    }
}
