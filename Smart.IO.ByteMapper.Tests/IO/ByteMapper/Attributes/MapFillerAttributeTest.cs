namespace Smart.IO.ByteMapper.Attributes;

using Xunit;

public class MapFillerAttributeTest
{
    //--------------------------------------------------------------------------------
    // Attribute
    //--------------------------------------------------------------------------------

    [Fact]
    public void MapByFillerAttribute()
    {
        var mapperFactory = new MapperFactoryConfig()
            .DefaultDelimiter(null)
            .DefaultFiller((byte)' ')
            .CreateMapByAttribute<FillerAttributeObject>()
            .ToMapperFactory();
        var mapper = mapperFactory.Create<FillerAttributeObject>();

        var buffer = new byte[mapper.Size];
        var obj = new FillerAttributeObject();

        // Write
        mapper.ToByte(buffer, 0, obj);

        Assert.Equal("    00"u8.ToArray(), buffer);
    }

    //--------------------------------------------------------------------------------
    // Fix
    //--------------------------------------------------------------------------------

    [Fact]
    public void CoverageFix()
    {
        var attribute = new MapFillerAttribute(0, 0);

        Assert.Throws<NotSupportedException>(() => attribute.Filler);

        Assert.Throws<ArgumentOutOfRangeException>(() => new MapFillerAttribute(-1, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new MapFillerAttribute(0, -1));
    }

    //--------------------------------------------------------------------------------
    // Helper
    //--------------------------------------------------------------------------------

    [Map(6, AutoFiller = true)]
    [MapFiller(0, 2)]
    [MapFiller(4, 2, Filler = 0x30)]
    internal sealed class FillerAttributeObject
    {
    }
}
