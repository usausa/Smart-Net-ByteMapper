namespace Smart.IO.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Smart.IO.Mapper.Attributes;

    public static class ByteMapperConfigAttributeExtensions
    {
        //--------------------------------------------------------------------------------
        // Single
        //--------------------------------------------------------------------------------

        public static ByteMapperConfig MapByAttribute<T>(this ByteMapperConfig config)
        {
            return config.MapByAttribute(typeof(T), null, true);
        }

        public static ByteMapperConfig MapByAttribute<T>(this ByteMapperConfig config, string profile)
        {
            return config.MapByAttribute(typeof(T), profile, true);
        }

        public static ByteMapperConfig MapByAttribute<T>(this ByteMapperConfig config, bool validate)
        {
            return config.MapByAttribute(typeof(T), null, validate);
        }

        public static ByteMapperConfig MapByAttribute<T>(this ByteMapperConfig config, string profile, bool validate)
        {
            return config.MapByAttribute(typeof(T), profile, validate);
        }

        public static ByteMapperConfig MapByAttribute(this ByteMapperConfig config, Type type)
        {
            return config.MapByAttribute(type, null, true);
        }

        public static ByteMapperConfig MapByAttribute(this ByteMapperConfig config, Type type, string profile)
        {
            return config.MapByAttribute(type, profile, true);
        }

        public static ByteMapperConfig MapByAttribute(this ByteMapperConfig config, Type type, bool validate)
        {
            return config.MapByAttribute(type, null, validate);
        }

        public static ByteMapperConfig MapByAttribute(this ByteMapperConfig config, Type type, string profile, bool validate)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var mapAttribute = type.GetCustomAttribute<MapAttribute>();
            if (mapAttribute == null)
            {
                throw new ArgumentException($"No MapAttribute. type=[{type.FullName}]", nameof(type));
            }

            config.AddMapping(new AttributeMapping(type, mapAttribute, profile, validate));

            return config;
        }

        //--------------------------------------------------------------------------------
        // Multi
        //--------------------------------------------------------------------------------

        public static ByteMapperConfig MapByAttribute(this ByteMapperConfig config, IEnumerable<Type> types)
        {
            return MapByAttribute(config, types, null, true);
        }

        public static ByteMapperConfig MapByAttribute(this ByteMapperConfig config, IEnumerable<Type> types, string profile)
        {
            return MapByAttribute(config, types, profile, true);
        }

        public static ByteMapperConfig MapByAttribute(this ByteMapperConfig config, IEnumerable<Type> types, bool validate)
        {
            return MapByAttribute(config, types, null, validate);
        }

        public static ByteMapperConfig MapByAttribute(this ByteMapperConfig config, IEnumerable<Type> types, string profile, bool validate)
        {
            if (types == null)
            {
                throw new ArgumentNullException(nameof(types));
            }

            var targets = types
                .Where(x => x != null)
                .Select(x => new
                {
                    Type = x,
                    Attribute = x.GetCustomAttribute<MapAttribute>()
                })
                .Where(x => x.Attribute != null);
            foreach (var pair in targets)
            {
                config.AddMapping(new AttributeMapping(pair.Type, pair.Attribute, profile, validate));
            }

            return config;
        }
    }
}
