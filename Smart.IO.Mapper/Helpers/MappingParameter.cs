namespace Smart.IO.Mapper.Helpers
{
    using System.Collections.Generic;

    public class MappingParameter : IMappingParameter
    {
        private readonly IDictionary<string, object> globalParameters;

        private readonly IDictionary<string, object> typeParameters;

        public MappingParameter(
            IDictionary<string, object> globalParameters,
            IDictionary<string, object> typeParameters)
        {
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
