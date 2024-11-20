namespace Smart.IO.ByteMapper;

using System.Reflection;

using Smart.IO.ByteMapper.Attributes;

#pragma warning disable CA1062
#pragma warning disable CA2263
public static class AttributeExtensions
{
    //--------------------------------------------------------------------------------
    // Config.Single
    //--------------------------------------------------------------------------------

    public static MapperFactoryConfig CreateMapByAttribute<T>(this MapperFactoryConfig config)
    {
        return config.CreateMapByAttribute(typeof(T), null, true);
    }

    public static MapperFactoryConfig CreateMapByAttribute<T>(this MapperFactoryConfig config, string profile)
    {
        return config.CreateMapByAttribute(typeof(T), profile, true);
    }

    public static MapperFactoryConfig CreateMapByAttribute<T>(this MapperFactoryConfig config, bool validation)
    {
        return config.CreateMapByAttribute(typeof(T), null, validation);
    }

    public static MapperFactoryConfig CreateMapByAttribute<T>(this MapperFactoryConfig config, string profile, bool validation)
    {
        return config.CreateMapByAttribute(typeof(T), profile, validation);
    }

    public static MapperFactoryConfig CreateMapByAttribute(this MapperFactoryConfig config, Type type)
    {
        return config.CreateMapByAttribute(type, null, true);
    }

    public static MapperFactoryConfig CreateMapByAttribute(this MapperFactoryConfig config, Type type, string profile)
    {
        return config.CreateMapByAttribute(type, profile, true);
    }

    public static MapperFactoryConfig CreateMapByAttribute(this MapperFactoryConfig config, Type type, bool validation)
    {
        return config.CreateMapByAttribute(type, null, validation);
    }

    public static MapperFactoryConfig CreateMapByAttribute(this MapperFactoryConfig config, Type type, string profile, bool validation)
    {
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        var mapAttribute = type.GetCustomAttribute<MapAttribute>();
        if (mapAttribute is null)
        {
            throw new ArgumentException($"No MapAttribute. type=[{type.FullName}]", nameof(type));
        }

        config.AddMappingFactory(new AttributeMappingFactory(type, mapAttribute, profile, validation));

        return config;
    }

    //--------------------------------------------------------------------------------
    // Config.Multi
    //--------------------------------------------------------------------------------

    public static MapperFactoryConfig CreateMapByAttribute(this MapperFactoryConfig config, IEnumerable<Type> types)
    {
        return CreateMapByAttribute(config, types, null, true);
    }

    public static MapperFactoryConfig CreateMapByAttribute(this MapperFactoryConfig config, IEnumerable<Type> types, string profile)
    {
        return CreateMapByAttribute(config, types, profile, true);
    }

    public static MapperFactoryConfig CreateMapByAttribute(this MapperFactoryConfig config, IEnumerable<Type> types, bool validation)
    {
        return CreateMapByAttribute(config, types, null, validation);
    }

    public static MapperFactoryConfig CreateMapByAttribute(this MapperFactoryConfig config, IEnumerable<Type> types, string profile, bool validation)
    {
        if (types is null)
        {
            throw new ArgumentNullException(nameof(types));
        }

        var targets = types
            .Where(static x => x != null)
            .Select(static x => new
            {
                Type = x,
                Attribute = x.GetCustomAttribute<MapAttribute>()
            })
            .Where(static x => x.Attribute != null);
        foreach (var pair in targets)
        {
            config.AddMappingFactory(new AttributeMappingFactory(pair.Type, pair.Attribute, profile, validation));
        }

        return config;
    }

    //--------------------------------------------------------------------------------
    // Profile.Single
    //--------------------------------------------------------------------------------

    public static MapperProfile CreateMapByAttribute<T>(this MapperProfile profile)
    {
        return profile.CreateMapByAttribute(typeof(T), true);
    }

    public static MapperProfile CreateMapByAttribute<T>(this MapperProfile profile, bool validation)
    {
        return profile.CreateMapByAttribute(typeof(T), validation);
    }

    public static MapperProfile CreateMapByAttribute(this MapperProfile profile, Type type)
    {
        return profile.CreateMapByAttribute(type, true);
    }

    public static MapperProfile CreateMapByAttribute(this MapperProfile profile, Type type, bool validation)
    {
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        var mapAttribute = type.GetCustomAttribute<MapAttribute>();
        if (mapAttribute is null)
        {
            throw new ArgumentException($"No MapAttribute. type=[{type.FullName}]", nameof(type));
        }

        profile.AddMappingFactory(new AttributeMappingFactory(type, mapAttribute, profile.Name, validation));

        return profile;
    }

    //--------------------------------------------------------------------------------
    // Profile.Multi
    //--------------------------------------------------------------------------------

    public static MapperProfile CreateMapByAttribute(this MapperProfile profile, IEnumerable<Type> types)
    {
        return CreateMapByAttribute(profile, types, true);
    }

    public static MapperProfile CreateMapByAttribute(this MapperProfile profile, IEnumerable<Type> types, bool validation)
    {
        if (types is null)
        {
            throw new ArgumentNullException(nameof(types));
        }

        var targets = types
            .Where(static x => x != null)
            .Select(static x => new
            {
                Type = x,
                Attribute = x.GetCustomAttribute<MapAttribute>()
            })
            .Where(static x => x.Attribute != null);
        foreach (var pair in targets)
        {
            profile.AddMappingFactory(new AttributeMappingFactory(pair.Type, pair.Attribute, profile.Name, validation));
        }

        return profile;
    }
}
#pragma warning restore CA2263
#pragma warning restore CA1062
