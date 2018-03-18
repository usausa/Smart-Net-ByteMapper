namespace Smart.IO.Mapper
{
    using System;
    using System.Collections.Generic;

    using Smart.ComponentModel;

    public class MapperBuilderConfig : IMapperBuilderConfig
    {
        private readonly ComponentConfig config = new ComponentConfig();

        private readonly Dictionary<string, object> parameters = new Dictionary<string, object>();

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

        public IComponentContainer ResolveComponents()
        {
            return config.ToContainer();
        }

        Dictionary<string, object> IMapperBuilderConfig.ResolveParameters()
        {
            return parameters;
        }
    }
}
