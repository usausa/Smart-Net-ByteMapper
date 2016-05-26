namespace Smart.IO.Mapper.Configuration
{
    using System;
    using System.Linq.Expressions;

    using Smart.IO.Mapper.Mappers;
    using Smart.Reflection;

    internal class TypeExpression<T> : ITypeExpression<T>
    {
        private readonly IDefaultSettings defaultSettings;

        private readonly TypeMapper typeMapper;

        private int lastOffset;

        public TypeExpression(IDefaultSettings defaultSettings, TypeMapper typeMapper)
        {
            this.defaultSettings = defaultSettings;
            this.typeMapper = typeMapper;
        }

        public ITypeExpression<T> Filler(int length, byte value)
        {
            return Filler(lastOffset, length, value);
        }

        public ITypeExpression<T> Filler(int offset, int length, byte value)
        {
            typeMapper.AddFiled(new FillerMapper(offset, length, value));
            lastOffset += length;
            return this;
        }

        public ITypeExpression<T> Constant(byte[] value)
        {
            return Constant(lastOffset, value);
        }

        public ITypeExpression<T> Constant(int offset, byte[] value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            typeMapper.AddFiled(new ConstantMapper(offset, value));
            lastOffset += value.Length;
            return this;
        }

        public ITypeExpression<T> ForMember(string name, int length, Action<IMemberConfigurationExpression> config)
        {
            return ForMember(name, lastOffset, length, config);
        }

        public ITypeExpression<T> ForMember(string name, int offset, int length, Action<IMemberConfigurationExpression> config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var type = typeof(T);
            var prop = type.GetProperty(name);
            if (prop == null)
            {
                throw new ArgumentException("Name is invalid.", nameof(name));
            }

            var accessor = prop.ToAccessor();
            var memberType = accessor.Type;

            var memberMapper = new MemberMapper(offset, length)
            {
                Padding = defaultSettings.GetPadding(memberType),
                PaddingByte = defaultSettings.GetPaddingByte(memberType),
                Trim = defaultSettings.GetTrim(memberType),
                NullIfEmpty = defaultSettings.GetNullIfEmpty(memberType),
                NullValue = defaultSettings.GetNullValue(memberType),
                Converter = defaultSettings.GetConverter(memberType),
                Accessor = accessor
            };

            typeMapper.AddFiled(memberMapper);
            lastOffset += length;

            config(new MemberConfigurationExpression(memberMapper));

            return this;
        }

        public ITypeExpression<T> ForMember(string name, int length)
        {
            return ForMember(name, lastOffset, length, option => { });
        }

        public ITypeExpression<T> ForMember(string name, int offset, int length)
        {
            return ForMember(name, offset, length, option => { });
        }

        public ITypeExpression<T> ForMember(Expression<Func<T, object>> expr, int length, Action<IMemberConfigurationExpression> config)
        {
            return ForMember(ExpressionHelper.GetMemberName(expr), lastOffset, length, config);
        }

        public ITypeExpression<T> ForMember(Expression<Func<T, object>> expr, int offset, int length, Action<IMemberConfigurationExpression> config)
        {
            return ForMember(ExpressionHelper.GetMemberName(expr), offset, length, config);
        }

        public ITypeExpression<T> ForMember(Expression<Func<T, object>> expr, int length)
        {
            return ForMember(ExpressionHelper.GetMemberName(expr), lastOffset, length, option => { });
        }

        public ITypeExpression<T> ForMember(Expression<Func<T, object>> expr, int offset, int length)
        {
            return ForMember(ExpressionHelper.GetMemberName(expr), offset, length, option => { });
        }
    }
}
