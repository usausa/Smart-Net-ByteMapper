namespace Smart.IO.Mapper
{
    using System;
    using System.Collections.Generic;

    public static class MapperBuilderConfigAttributeExtensions
    {
        public static MapperBuilderConfig Map<T>(this MapperBuilderConfig config)
        {
            return config.Map(typeof(T));
        }

        public static MapperBuilderConfig Map(this MapperBuilderConfig config, Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            // TODO check attribute

            // TODO common

            return config;
        }

        public static MapperBuilderConfig Map(this MapperBuilderConfig config, IEnumerable<Type> types)
        {
            if (types == null)
            {
                throw new ArgumentNullException(nameof(types));
            }

            // TODO common where attribute

            return config;
        }
    }
}
