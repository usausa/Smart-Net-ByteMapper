namespace Smart.IO.Mapper.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Smart.IO.Mapper.Converters;
    using Smart.IO.Mapper.Mappers;

    /// <summary>
    ///
    /// </summary>
    internal class MapperConfigurationExpression : IMapperConfigurationExpresion, IDefaultSettings
    {
        private readonly MapperConfig mapperConfig;

        private readonly Dictionary<Type, Padding> paddingOfType = new Dictionary<Type, Padding>();

        private readonly Dictionary<Type, byte> paddingBytesOfType = new Dictionary<Type, byte>();

        private readonly Dictionary<Type, bool> trimOfType = new Dictionary<Type, bool>();

        private readonly Dictionary<Type, bool> nullIfEmptyOfType = new Dictionary<Type, bool>();

        private readonly Dictionary<Type, byte[]> nullValueOfType = new Dictionary<Type, byte[]>();

        private readonly Dictionary<Type, IValueConverter> formatterOfType = new Dictionary<Type, IValueConverter>();

        private Padding defaultPadding = Padding.Right;

        private byte defaultPaddingByte;

        private bool defaultTrim = true;

        private bool defaultNullIfEmpty = true;

        private byte[] defaultNullValue;

        private IValueConverter defaultConverter = new DefaultConverter();

        private byte defaultFiller;

        private byte[] defaultDelimitter;

        /// <summary>
        ///
        /// </summary>
        /// <param name="mapperConfig"></param>
        public MapperConfigurationExpression(MapperConfig mapperConfig)
        {
            this.mapperConfig = mapperConfig;
        }

        public IMapperConfigurationExpresion DefaultFiller(byte value)
        {
            defaultFiller = value;
            return this;
        }

        public IMapperConfigurationExpresion DefaultDelimitter(byte[] value)
        {
            defaultDelimitter = value;
            return this;
        }

        public IMapperConfigurationExpresion DefaultEncording(Encoding encoding)
        {
            mapperConfig.Encoding = encoding;
            return this;
        }

        public IMapperConfigurationExpresion DefaultPadding(Padding padding)
        {
            defaultPadding = padding;
            return this;
        }

        public IMapperConfigurationExpresion DefaultPadding(byte filler)
        {
            defaultPaddingByte = filler;
            return this;
        }

        public IMapperConfigurationExpresion DefaultPadding(Padding padding, byte filler)
        {
            defaultPadding = padding;
            defaultPaddingByte = filler;
            return this;
        }

        public IMapperConfigurationExpresion DefaultTrim(bool value)
        {
            defaultTrim = value;
            return this;
        }

        public IMapperConfigurationExpresion DefaultNullIfEmpty(bool value)
        {
            defaultNullIfEmpty = value;
            return this;
        }

        public IMapperConfigurationExpresion DefaultNullValue(byte[] value)
        {
            defaultNullValue = value;
            return this;
        }

        public IMapperConfigurationExpresion DefaultConverter(IValueConverter value)
        {
            defaultConverter = value;
            return this;
        }

        public IMapperConfigurationExpresion DefaultPadding(Type type, byte filler)
        {
            paddingBytesOfType[type] = filler;
            return this;
        }

        public IMapperConfigurationExpresion DefaultPadding(Type type, Padding direction)
        {
            paddingOfType[type] = direction;
            return this;
        }

        public IMapperConfigurationExpresion DefaultPadding(Type type, Padding direction, byte filler)
        {
            paddingOfType[type] = direction;
            paddingBytesOfType[type] = filler;
            return this;
        }

        public IMapperConfigurationExpresion DefaultTrim(Type type, bool value)
        {
            trimOfType[type] = value;
            return this;
        }

        public IMapperConfigurationExpresion DefaultNullIfEmpty(Type type, bool value)
        {
            nullIfEmptyOfType[type] = value;
            return this;
        }

        public IMapperConfigurationExpresion DefaultValue(Type type, byte[] value)
        {
            nullValueOfType[type] = value;
            return this;
        }

        public IMapperConfigurationExpresion DefaultConverter(Type type, IValueConverter value)
        {
            formatterOfType[type] = value;
            return this;
        }

        public Encoding GetEncoding()
        {
            return mapperConfig.Encoding;
        }

        Padding IDefaultSettings.GetPadding(Type type)
        {
            Padding value;
            return paddingOfType.TryGetValue(Nullable.GetUnderlyingType(type) ?? type, out value) ? value : defaultPadding;
        }

        byte IDefaultSettings.GetPaddingByte(Type type)
        {
            byte value;
            return paddingBytesOfType.TryGetValue(Nullable.GetUnderlyingType(type) ?? type, out value) ? value : defaultPaddingByte;
        }

        bool IDefaultSettings.GetTrim(Type type)
        {
            bool value;
            return trimOfType.TryGetValue(Nullable.GetUnderlyingType(type) ?? type, out value) ? value : defaultTrim;
        }

        bool IDefaultSettings.GetNullIfEmpty(Type type)
        {
            bool value;
            return nullIfEmptyOfType.TryGetValue(Nullable.GetUnderlyingType(type) ?? type, out value) ? value : defaultNullIfEmpty;
        }

        public byte[] GetNullValue(Type type)
        {
            byte[] value;
            return nullValueOfType.TryGetValue(Nullable.GetUnderlyingType(type) ?? type, out value) ? value : defaultNullValue;
        }

        IValueConverter IDefaultSettings.GetConverter(Type type)
        {
            IValueConverter value;
            return formatterOfType.TryGetValue(Nullable.GetUnderlyingType(type) ?? type, out value) ? value : defaultConverter;
        }

        public ITypeExpression<T> CreateMap<T>(int length)
        {
            return CreateMap<T>(length, defaultFiller, defaultDelimitter);
        }

        public ITypeExpression<T> CreateMap<T>(int length, byte filler)
        {
            return CreateMap<T>(length, filler, defaultDelimitter);
        }

        public ITypeExpression<T> CreateMap<T>(int length, byte[] delimitter)
        {
            return CreateMap<T>(length, defaultFiller, delimitter);
        }

        public ITypeExpression<T> CreateMap<T>(int length, byte filler, byte[] delimitter)
        {
            var typeMapper = new TypeMapper(length, filler);
            mapperConfig.AddTypeMapper(typeof(T), typeMapper);

            var expression = new TypeExpression<T>(this, typeMapper);

            if (delimitter != null)
            {
                typeMapper.AddFiled(new ConstantMapper(length - delimitter.Length, delimitter));
            }

            return expression;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        public void CreateMap(Type type)
        {
            // TODO Attrbute version
            throw new NotImplementedException();
        }
    }
}
