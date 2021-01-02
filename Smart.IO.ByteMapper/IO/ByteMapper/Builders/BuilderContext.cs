namespace Smart.IO.ByteMapper.Builders
{
    using System.Collections.Generic;

    using Smart.ComponentModel;

    public sealed class BuilderContext : IBuilderContext
    {
        private readonly IDictionary<string, object> globalParameters;

        private readonly IDictionary<string, object> typeParameters;

        public ComponentContainer Components { get; }

        public BuilderContext(
            ComponentContainer components,
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
                if (obj is null)
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
                if (obj is null)
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

        public bool TryGetParameter<T>(string key, out T value)
        {
            if (typeParameters.TryGetValue(key, out var obj))
            {
                if (obj is null)
                {
                    value = default;
                    return true;
                }

                if (obj is T t)
                {
                    value = t;
                    return true;
                }
            }

            if (globalParameters.TryGetValue(key, out obj))
            {
                if (obj is null)
                {
                    value = default;
                    return true;
                }

                if (obj is T t)
                {
                    value = t;
                    return true;
                }
            }

            value = default;
            return false;
        }
    }
}
