namespace Smart.IO.ByteMapper.Attributes;

public sealed class MapAsciiAttributeTest
{
    //--------------------------------------------------------------------------------
    // Attribute
    //--------------------------------------------------------------------------------

    [Fact]
    public void MapByAsciiAttribute()
    {
        var mapperFactory = new MapperFactoryConfig()
            .UseOptionsDefault()
            .DefaultDelimiter(null)
            .DefaultTrim(true)
            .DefaultTextPadding(Padding.Right)
            .DefaultTextFiller(0x20)
            .CreateMapByAttribute<AsciiAttributeObject>()
            .ToMapperFactory();
        var mapper = mapperFactory.Create<AsciiAttributeObject>();

        var buffer = new byte[mapper.Size];
        var obj = new AsciiAttributeObject();

        // Write
        mapper.ToByte(buffer, 0, obj);

        Assert.Equal("    ____"u8.ToArray(), buffer);

        // Read
        mapper.FromByte("12  __AB"u8.ToArray(), 0, obj);

        Assert.Equal("12", obj.StringValue);
        Assert.Equal("__AB", obj.CustomStringValue);
    }

    //--------------------------------------------------------------------------------
    // Fix
    //--------------------------------------------------------------------------------

    [Fact]
    public void CoverageFix()
    {
        var attribute = new MapAsciiAttribute(0, 0);

        Assert.Throws<NotSupportedException>(() => attribute.Trim);
        Assert.Throws<NotSupportedException>(() => attribute.Padding);
        Assert.Throws<NotSupportedException>(() => attribute.Filler);

        Assert.Throws<ArgumentOutOfRangeException>(() => new MapAsciiAttribute(-1, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new MapAsciiAttribute(0, -1));
    }

    //--------------------------------------------------------------------------------
    // Helper
    //--------------------------------------------------------------------------------

    [Map(8)]
    internal sealed class AsciiAttributeObject
    {
        [MapAscii(0, 4)]
        public string StringValue { get; set; }

        [MapAscii(4, 4, Trim = false, Padding = Padding.Left, Filler = (byte)'_')]
        public string CustomStringValue { get; set; }
    }
}
