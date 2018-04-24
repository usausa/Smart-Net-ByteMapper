﻿namespace Smart.IO.ByteMapper
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    using Smart.ComponentModel;
    using Smart.Reflection;

    public sealed class MapperFactoryConfig : IMapperFactoryConfig
    {
        private readonly ComponentConfig config = new ComponentConfig();

        private readonly IDictionary<string, object> parameters = new Dictionary<string, object>();

        private readonly IList<IMappingFactory> factories = new List<IMappingFactory>();

        private readonly IList<IMapperProfile> profiles = new List<IMapperProfile>();

        public MapperFactoryConfig()
        {
            config.Add<IDelegateFactory>(DelegateFactory.Default);

            this.DefaultDelimiter(0x0D, 0x0A);
            this.DefaultEncoding(Encoding.ASCII);
            this.DefaultTrim(true);
            this.DefaultTextPadding(Padding.Right);
            this.DefaultFiller(0x20);
            this.DefaultTextFiller(0x20);
            this.DefaultEndian(Endian.Big);
            this.DefaultTrueValue(0x31);
            this.DefaultFalseValue(0x30);

            this.DefaultDateTimeTextEncoding(Encoding.ASCII);
            this.DefaultDateTimeTextProvider(CultureInfo.InvariantCulture);
            this.DefaultDateTimeTextStyle(DateTimeStyles.None);

            this.DefaultNumberTextEncoding(Encoding.ASCII);
            this.DefaultNumberTextProvider(CultureInfo.InvariantCulture);
            this.DefaultNumberTextNumberStyle(NumberStyles.Integer);
            this.DefaultNumberTextDecimalStyle(NumberStyles.Number);
            this.DefaultNumberTextPadding(Padding.Left);
            this.DefaultNumberTextFiller(0x20);
        }

        public MapperFactoryConfig Configure(Action<ComponentConfig> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            action(config);

            return this;
        }

        public MapperFactoryConfig AddParameter<T>(string name, T parameter)
        {
            parameters[name] = parameter;

            return this;
        }

        public MapperFactoryConfig AddMappingFactory(IMappingFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            factories.Add(factory);
            return this;
        }

        public MapperFactoryConfig AddProfile(IMapperProfile profile)
        {
            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            profiles.Add(profile);
            return this;
        }

        public MapperFactoryConfig AddProfile<TProfile>()
            where TProfile : IMapperProfile, new()
        {
            profiles.Add(new TProfile());
            return this;
        }

        //--------------------------------------------------------------------------------
        // IByteMapperConfig
        //--------------------------------------------------------------------------------

        IComponentContainer IMapperFactoryConfig.ResolveComponents()
        {
            return config.ToContainer();
        }

        IDictionary<string, object> IMapperFactoryConfig.ResolveParameters()
        {
            return new Dictionary<string, object>(parameters);
        }

        IEnumerable<IMappingFactory> IMapperFactoryConfig.ResolveMappingFactories()
        {
            return factories.Concat(profiles.SelectMany(x => x.ResolveMappingFactories()));
        }
    }
}
