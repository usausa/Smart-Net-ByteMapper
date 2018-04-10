namespace Smart.IO.ByteMapper.Builders
{
    using System.Collections.Generic;

    using Smart.ComponentModel;

    public sealed class BuilderContext : IBuilderContext
    {
        private readonly IDictionary<string, object> globalParameters;

        private readonly IDictionary<string, object> typeParameters;

        public IComponentContainer Components { get; }

        public BuilderContext(
            IComponentContainer components,
            IDictionary<string, object> globalParameters,
            IDictionary<string, object> typeParameters)
        {
            Components = components;
            this.globalParameters = globalParameters;
            this.typeParameters = typeParameters;
        }

        public T GetParameter<T>(string key)
        {
            if (typeParameters.TryGetValue(key, out var obj))
            {
                if (obj == null)
                {
                    return default;
                }

                if (obj is T value)
                {
                    return value;
                }
            }

            if (globalParameters.TryGetValue(key, out obj))
            {
                if (obj == null)
                {
                    return default;
                }

                if (obj is T value)
                {
                    return value;
                }
            }

            throw new ByteMapperException($"Parameter not found. key=[{key}]");
        }
    }
}
