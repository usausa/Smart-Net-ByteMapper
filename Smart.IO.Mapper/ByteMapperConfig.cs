namespace Smart.IO.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    using Smart.ComponentModel;
    using Smart.Reflection;

    public sealed class ByteMapperConfig : IByteMapperConfig
    {
        private readonly ComponentConfig config = new ComponentConfig();

        private readonly Dictionary<string, object> parameters = new Dictionary<string, object>();

        private readonly List<IMapping> mappings = new List<IMapping>();

        public ByteMapperConfig()
        {
            config.Add<IDelegateFactory>(DelegateFactory.Default);

            this.DefaultDelimiter(new byte[] { 0x0d, 0x0a });
            this.DefaultEncoding(Encoding.ASCII);
            this.DefaultNumberProvider(CultureInfo.InvariantCulture);
            this.DefaultDateTimeProvider(CultureInfo.InvariantCulture);
            this.DefaultNumberStyle(NumberStyles.Integer);
            this.DefaultDecimalStyle(NumberStyles.Number);
            this.DefaultDateTimeStyle(DateTimeStyles.None);
            this.DefaultTrim(true);
            this.DefaultTextPadding(Padding.Right);
            this.DefaultNumberPadding(Padding.Left);
            this.DefaultFiller(0x20);
            this.DefaultTextFiller(0x20);
            this.DefaultNumberFiller(0x20);
            this.DefaultEndian(Endian.Big);
            this.DefaultTrueValue(0x31);
            this.DefaultFalseValue(0x30);
        }

        public ByteMapperConfig Configure(Action<ComponentConfig> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            action(config);

            return this;
        }

        public ByteMapperConfig AddParameter<T>(string name, T parameter)
        {
            parameters[name] = parameter;

            return this;
        }

        public ByteMapperConfig AddMapping(IMapping mapping)
        {
            if (mapping == null)
            {
                throw new ArgumentNullException(nameof(mapping));
            }

            mappings.Add(mapping);
            return this;
        }

        IComponentContainer IByteMapperConfig.ResolveComponents()
        {
            return config.ToContainer();
        }

        IDictionary<string, object> IByteMapperConfig.ResolveParameters()
        {
            return new Dictionary<string, object>(parameters);
        }

        IEnumerable<IMapping> IByteMapperConfig.ResolveMappings()
        {
            return mappings;
        }
    }
}
