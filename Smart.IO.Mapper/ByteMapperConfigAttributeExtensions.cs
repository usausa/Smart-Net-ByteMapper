namespace Smart.IO.Mapper
{
    using System;
    using System.Collections.Generic;

    public static class ByteMapperConfigAttributeExtensions
    {
        public static ByteMapperConfig Map<T>(this ByteMapperConfig config)
        {
            return config.Map(typeof(T));
        }

        public static ByteMapperConfig Map(this ByteMapperConfig config, Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            // TODO check attribute

            // TODO common

            return config;
        }

        public static ByteMapperConfig Map(this ByteMapperConfig config, IEnumerable<Type> types)
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
