namespace Smart.IO.ByteMapper;

public class MapperProfile : IMapperProfile
{
    private readonly List<IMappingFactory> factories = [];

    public string Name { get; }

    public MapperProfile()
    {
    }

    public MapperProfile(string name)
    {
        Name = name;
    }

    public MapperProfile AddMappingFactory(IMappingFactory factory)
    {
        if (factory is null)
        {
            throw new ArgumentNullException(nameof(factory));
        }

        factories.Add(factory);
        return this;
    }

    //--------------------------------------------------------------------------------
    // IByteMapperProfile
    //--------------------------------------------------------------------------------

#pragma warning disable CA1033
    IEnumerable<IMappingFactory> IMapperProfile.ResolveMappingFactories() => factories;
#pragma warning restore CA1033
}
