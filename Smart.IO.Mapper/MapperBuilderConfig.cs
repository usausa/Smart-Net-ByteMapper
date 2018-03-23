namespace Smart.IO.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    using Smart.ComponentModel;
    using Smart.Reflection;

    public class MapperBuilderConfig : IMapperBuilderConfig
    {
        private readonly ComponentConfig config = new ComponentConfig();

        private readonly Dictionary<string, object> parameters = new Dictionary<string, object>();

        private readonly List<MappingEntry> mappings = new List<MappingEntry>();

        public MapperBuilderConfig()
        {
            config.Add<IDelegateFactory>(DelegateFactory.Default);

            this.DefaultDelimiter(new byte[] { 0x0d, 0x0a });
            this.DefaultEncoding(Encoding.ASCII);
            this.DefaultNumberFormat(NumberFormatInfo.InvariantInfo);
            this.DefaultNumberStyle(NumberStyles.Integer);
            this.DefaultDecimalStyle(NumberStyles.Number);
            this.DefaultDateTimeFormat(DateTimeFormatInfo.InvariantInfo);
            this.DefaultDateTimeStyle(DateTimeStyles.None);
            this.DefaultTrim(true);
            this.DefaultTextPadding(Padding.Right);
            this.DefaultNumberPadding(Padding.Left);
            this.DefaultTextFiller(0x20);
            this.DefaultNumberFiller(0x20);
            this.DefaultEndian(Endian.Big);
            this.DefaultTrue(0x31);
            this.DefaultFalse(0x30);
        }

        public MapperBuilderConfig Configure(Action<ComponentConfig> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            action(config);

            return this;
        }

        public MapperBuilderConfig AddParameter<T>(T parameter)
        {
            parameters[typeof(T).Name] = parameter;

            return this;
        }

        public MapperBuilderConfig AddParameter<T>(string name, T parameter)
        {
            parameters[name] = parameter;

            return this;
        }

        public MapperBuilderConfig AddMapping<T>(IMemberMapperFactory[] factories)
        {
            // TODO タイプ自体のメタデータ？
            return AddMapping<T>(string.Empty, factories);
        }

        public MapperBuilderConfig AddMapping<T>(string profile, IMemberMapperFactory[] factories)
        {
            mappings.Add(new MappingEntry(typeof(T), profile ?? string.Empty, factories));

            return this;
        }

        IComponentContainer IMapperBuilderConfig.ResolveComponents()
        {
            return config.ToContainer();
        }

        Dictionary<string, object> IMapperBuilderConfig.ResolveParameters()
        {
            return new Dictionary<string, object>(parameters);
        }

        MappingEntry[] IMapperBuilderConfig.ResolveMappings()
        {
            return mappings.ToArray();
        }
    }
}
