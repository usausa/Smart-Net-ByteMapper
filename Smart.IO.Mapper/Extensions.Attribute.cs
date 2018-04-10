namespace Smart.IO.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Smart.IO.Mapper.Attributes;

    public static class AttributeExtensions
    {
        //--------------------------------------------------------------------------------
        // Config.Single
        //--------------------------------------------------------------------------------

        public static ByteMapperConfig CreateMapByAttribute<T>(this ByteMapperConfig config)
        {
            return config.CreateMapByAttribute(typeof(T), null, true);
        }

        public static ByteMapperConfig CreateMapByAttribute<T>(this ByteMapperConfig config, string profile)
        {
            return config.CreateMapByAttribute(typeof(T), profile, true);
        }

        public static ByteMapperConfig CreateMapByAttribute<T>(this ByteMapperConfig config, bool validation)
        {
            return config.CreateMapByAttribute(typeof(T), null, validation);
        }

        public static ByteMapperConfig CreateMapByAttribute<T>(this ByteMapperConfig config, string profile, bool validation)
        {
            return config.CreateMapByAttribute(typeof(T), profile, validation);
        }

        public static ByteMapperConfig CreateMapByAttribute(this ByteMapperConfig config, Type type)
        {
            return config.CreateMapByAttribute(type, null, true);
        }

        public static ByteMapperConfig CreateMapByAttribute(this ByteMapperConfig config, Type type, string profile)
        {
            return config.CreateMapByAttribute(type, profile, true);
        }

        public static ByteMapperConfig CreateMapByAttribute(this ByteMapperConfig config, Type type, bool validation)
        {
            return config.CreateMapByAttribute(type, null, validation);
        }

        public static ByteMapperConfig CreateMapByAttribute(this ByteMapperConfig config, Type type, string profile, bool validation)
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

            config.AddMapping(new AttributeMapping(type, mapAttribute, profile, validation));

            return config;
        }

        //--------------------------------------------------------------------------------
        // Config.Multi
        //--------------------------------------------------------------------------------

        public static ByteMapperConfig CreateMapByAttribute(this ByteMapperConfig config, IEnumerable<Type> types)
        {
            return CreateMapByAttribute(config, types, null, true);
        }

        public static ByteMapperConfig CreateMapByAttribute(this ByteMapperConfig config, IEnumerable<Type> types, string profile)
        {
            return CreateMapByAttribute(config, types, profile, true);
        }

        public static ByteMapperConfig CreateMapByAttribute(this ByteMapperConfig config, IEnumerable<Type> types, bool validation)
        {
            return CreateMapByAttribute(config, types, null, validation);
        }

        public static ByteMapperConfig CreateMapByAttribute(this ByteMapperConfig config, IEnumerable<Type> types, string profile, bool validation)
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
                config.AddMapping(new AttributeMapping(pair.Type, pair.Attribute, profile, validation));
            }

            return config;
        }

        //--------------------------------------------------------------------------------
        // Profile.Single
        //--------------------------------------------------------------------------------

        public static ByteMapperProfile CreateMapByAttribute<T>(this ByteMapperProfile profile)
        {
            return profile.CreateMapByAttribute(typeof(T), true);
        }

        public static ByteMapperProfile CreateMapByAttribute<T>(this ByteMapperProfile profile, bool validation)
        {
            return profile.CreateMapByAttribute(typeof(T), validation);
        }

        public static ByteMapperProfile CreateMapByAttribute(this ByteMapperProfile profile, Type type)
        {
            return profile.CreateMapByAttribute(type, true);
        }

        public static ByteMapperProfile CreateMapByAttribute(this ByteMapperProfile profile, Type type, bool validation)
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

            profile.AddMapping(new AttributeMapping(type, mapAttribute, profile.Name, validation));

            return profile;
        }

        //--------------------------------------------------------------------------------
        // Profile.Multi
        //--------------------------------------------------------------------------------

        public static ByteMapperProfile CreateMapByAttribute(this ByteMapperProfile profile, IEnumerable<Type> types)
        {
            return CreateMapByAttribute(profile, types, true);
        }

        public static ByteMapperProfile CreateMapByAttribute(this ByteMapperProfile profile, IEnumerable<Type> types, bool validation)
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
                profile.AddMapping(new AttributeMapping(pair.Type, pair.Attribute, profile.Name, validation));
            }

            return profile;
        }
    }
}
