namespace Smart.IO.Mapper
{
    using System.Collections.Generic;

    using Smart.ComponentModel;

    public class MappingCreateContext : IMappingCreateContext
    {
        private readonly IDictionary<string, object> globalParameters;

        private readonly IDictionary<string, object> typeParameters;

        public IComponentContainer Components { get; }

        public MappingCreateContext(
            IDictionary<string, object> globalParameters,
            IDictionary<string, object> typeParameters,
            IComponentContainer components)
        {
            this.globalParameters = globalParameters;
            this.typeParameters = typeParameters;
            Components = components;
        }

        public bool TryGetParameter<T>(string key, out T value)
        {
            if (typeParameters.TryGetValue(key, out var obj) &&
                (obj?.GetType() == typeof(T)))
            {
                value = (T)obj;
                return true;
            }

            if (globalParameters.TryGetValue(key, out obj) &&
                (obj?.GetType() == typeof(T)))
            {
                value = (T)obj;
                return true;
            }

            value = default;
            return false;
        }

        public T GetParameter<T>(string key)
        {
            if (typeParameters.TryGetValue(key, out var obj) &&
                (obj?.GetType() == typeof(T)))
            {
                return (T)obj;
            }

            if (globalParameters.TryGetValue(key, out obj) &&
                (obj?.GetType() == typeof(T)))
            {
                return (T)obj;
            }

            throw new ByteMapperException($"Parameter not found. key=[{key}]");
        }
    }
}
