namespace Smart.IO.ByteMapper.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Smart.ComponentModel;
    using Smart.IO.ByteMapper.Builders;
    using Smart.IO.ByteMapper.Helpers;

    internal sealed class TypeConfigExpression<T> : ITypeConfigSyntax<T>, IMappingFactory
    {
        private readonly List<ITypeMapperBuilder> typeMapBuilders = new List<ITypeMapperBuilder>();

        private readonly List<IMemberMapperBuilder> memberMapBuilders = new List<IMemberMapperBuilder>();

        private readonly Dictionary<string, object> typeParameters = new Dictionary<string, object>();

        private readonly int size;

        private bool validation = true;

        private byte? nullFiller;

        private bool autoFiller = true;

        private bool useDelimiter = true;

        private int lastOffset;

        public Type Type { get; }

        public string Name { get; }

        public TypeConfigExpression(Type type, string name, int size)
        {
            Type = type;
            Name = name;
            this.size = size;
        }

        //--------------------------------------------------------------------------------
        // Syntax
        //--------------------------------------------------------------------------------

        // Validation

        public ITypeConfigSyntax<T> WithValidation(bool value)
        {
            validation = value;
            return this;
        }

        // Type setting

        public ITypeConfigSyntax<T> NullFiller(byte value)
        {
            nullFiller = value;
            return this;
        }

        public ITypeConfigSyntax<T> AutoFiller(bool value)
        {
            autoFiller = value;
            return this;
        }

        public ITypeConfigSyntax<T> UseDelimiter(bool value)
        {
            useDelimiter = value;
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
            builder.Offset = offset;
            typeMapBuilders.Add(builder);

            lastOffset = Math.Max(offset, lastOffset) + builder.CalcSize();

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

            if (config is null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var type = typeof(T);
            var pi = type.GetProperty(name);
            if (pi is null)
            {
                throw new ArgumentException("Name is invalid.", nameof(name));
            }

            var member = new MemberConfigExpression();
            config(member);

            if (member.Expression is null)
            {
                throw new InvalidOperationException("Property is not mapped.");
            }

            var converterBuilder = member.Expression.GetMapConverterBuilder();
            if (!converterBuilder.Match(pi.PropertyType))
            {
                throw new ByteMapperException(
                    "Expression does not match property. " +
                    $"type=[{pi.DeclaringType.FullName}], " +
                    $"property=[{pi.Name}]");
            }

            var builder = new MemberMapperBuilder(converterBuilder)
            {
                Property = pi,
                Offset = offset
            };
            memberMapBuilders.Add(builder);

            lastOffset = Math.Max(offset, lastOffset) + builder.CalcSize();

            return this;
        }

        //--------------------------------------------------------------------------------
        // IMappingFactory
        //--------------------------------------------------------------------------------

        IMapping IMappingFactory.Create(ComponentContainer components, IDictionary<string, object> parameters)
        {
            var context = new BuilderContext(components, parameters, typeParameters);

            var filler = context.GetParameter<byte>(Parameter.Filler);

            var list = new List<MapperPosition>();
            list.AddRange(typeMapBuilders.Select(x => new MapperPosition(x.Offset, x.CalcSize(), x.CreateMapper(context))));
            list.AddRange(memberMapBuilders.Select(x => new MapperPosition(x.Offset, x.CalcSize(), x.CreateMapper(context))));

            MapperPositionHelper.Layout(
                list,
                size,
                Type.FullName,
                validation,
                useDelimiter ? context.GetParameter<byte[]>(Parameter.Delimiter) : null,
                autoFiller ? (byte?)filler : null);

            return new Mapping(Type, size, nullFiller ?? filler, list.Select(x => x.Mapper).ToArray());
        }
    }
}
