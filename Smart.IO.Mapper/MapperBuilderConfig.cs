namespace Smart.IO.Mapper
{
    using System;
    using System.Collections.Generic;

    using Smart.ComponentModel;

    public class MapperBuilderConfig : IMapperBuilderConfig
    {
        private readonly ComponentConfig config = new ComponentConfig();

        private readonly Dictionary<Type, object> parameters = new Dictionary<Type, object>();

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
            parameters[typeof(T)] = parameter;

            return this;
        }

        public IComponentContainer ResolveComponents()
        {
            return config.ToContainer();
        }

        Dictionary<Type, object> IMapperBuilderConfig.ResolveParameters()
        {
            return parameters;
        }
    }
}
