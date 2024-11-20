namespace Smart.IO.ByteMapper.Attributes;

#pragma warning disable IDE0320
public sealed class MapAttributeTest
{
    //--------------------------------------------------------------------------------
    // NullFiller
    //--------------------------------------------------------------------------------

    [Fact]
    public void MapUseNullFiller()
    {
        var mapperFactory = new MapperFactoryConfig()
            .DefaultDelimiter(null)
            .DefaultFiller(0xCC)
            .CreateMapByAttribute<MapNullFillerObject>()
            .CreateMapByAttribute<DefaultNullFillerObject>()
            .ToMapperFactory();
        var mapMapper = mapperFactory.Create<MapNullFillerObject>();
        var defaultMapper = mapperFactory.Create<DefaultNullFillerObject>();

        // Write
        Assert.Equal([0xFF, 0xFF], mapMapper.ToByte(null));

        Assert.Equal([0xCC, 0xCC], defaultMapper.ToByte(null));
    }

    [Map(2, NullFiller = 0xFF)]
    internal sealed class MapNullFillerObject
    {
    }

    [Map(2)]
    internal sealed class DefaultNullFillerObject
    {
    }

    //--------------------------------------------------------------------------------
    // AutoFiller
    //--------------------------------------------------------------------------------

    [Fact]
    public void MapUseAutoFiller()
    {
        var mapperFactory = new MapperFactoryConfig()
            .DefaultDelimiter(null)
            .DefaultFiller(0xCC)
            .CreateMapByAttribute<TypeFillerObject>()
            .CreateMapByAttribute<DefaultFillerObject>()
            .CreateMapByAttribute<NoFillerObject>()
            .ToMapperFactory();
        var typeMapper = mapperFactory.Create<TypeFillerObject>();
        var defaultMapper = mapperFactory.Create<DefaultFillerObject>();
        var noMapper = mapperFactory.Create<NoFillerObject>();

        // Write
        Assert.Equal([0xFF, 0xFF], typeMapper.ToByte(new TypeFillerObject()));

        Assert.Equal([0xCC, 0xCC], defaultMapper.ToByte(new DefaultFillerObject()));

        var buffer = new byte[noMapper.Size].Also(x => x.AsSpan().Fill(0x11));
        noMapper.ToByte(buffer, 0, new NoFillerObject());
        Assert.Equal([0x11, 0x11], buffer);
    }

    [Map(2, AutoFiller = true)]
    [TypeFiller(0xFF)]
    internal sealed class TypeFillerObject
    {
    }

    [Map(2, AutoFiller = true)]
    internal sealed class DefaultFillerObject
    {
    }

    [Map(2, AutoFiller = false)]
    internal sealed class NoFillerObject
    {
    }

    //--------------------------------------------------------------------------------
    // Delimiter
    //--------------------------------------------------------------------------------

    [Fact]
    public void MapUseDelimiter()
    {
        var mapperFactory = new MapperFactoryConfig()
            .DefaultFiller(0x00)
            .DefaultDelimiter(0xCC)
            .CreateMapByAttribute<TypeDelimiterObject>()
            .CreateMapByAttribute<DefaultDelimiterObject>()
            .CreateMapByAttribute<NoDelimiterObject>()
            .ToMapperFactory();
        var typeMapper = mapperFactory.Create<TypeDelimiterObject>();
        var defaultMapper = mapperFactory.Create<DefaultDelimiterObject>();
        var noMapper = mapperFactory.Create<NoDelimiterObject>();

        // Write
        Assert.Equal([0x00, 0xFF], typeMapper.ToByte(new TypeDelimiterObject()));

        Assert.Equal([0x00, 0xCC], defaultMapper.ToByte(new DefaultDelimiterObject()));

        var buffer = new byte[noMapper.Size].Also(x => x.AsSpan().Fill(0x11));
        noMapper.ToByte(buffer, 0, new NoDelimiterObject());
        Assert.Equal([0x11, 0x11], buffer);
    }

    [Map(2, AutoFiller = false, UseDelimiter = true)]
    [TypeDelimiter(0xFF)]
    internal sealed class TypeDelimiterObject
    {
    }

    [Map(2, AutoFiller = false, UseDelimiter = true)]
    internal sealed class DefaultDelimiterObject
    {
    }

    [Map(2, AutoFiller = false, UseDelimiter = false)]
    internal sealed class NoDelimiterObject
    {
    }

    //--------------------------------------------------------------------------------
    // Fix
    //--------------------------------------------------------------------------------

    [Fact]
    public void CoverageFix()
    {
        var attribute = new MapAttribute(0);

        Assert.False(attribute.HasNullFiller);
        Assert.Equal(0, attribute.NullFiller);

        Assert.Throws<ArgumentOutOfRangeException>(() => new MapAttribute(-1));
    }
}
