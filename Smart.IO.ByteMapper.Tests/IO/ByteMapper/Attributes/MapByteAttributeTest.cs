namespace Smart.IO.ByteMapper.Attributes;

using Xunit;

public class MapByteAttributeTest
{
    //--------------------------------------------------------------------------------
    // Attribute
    //--------------------------------------------------------------------------------

    [Fact]
    public void MapByByteAttribute()
    {
        var mapperFactory = new MapperFactoryConfig()
            .DefaultDelimiter(null)
            .CreateMapByAttribute<ByteAttributeObject>()
            .ToMapperFactory();
        var mapper = mapperFactory.Create<ByteAttributeObject>();

        var buffer = new byte[mapper.Size];
        var obj = new ByteAttributeObject
        {
            ByteValue = 1
        };

        // Write
        mapper.ToByte(buffer, 0, obj);

        Assert.Equal(new byte[] { 0x01 }, buffer);

        // Read
        buffer[0] = 0x02;

        mapper.FromByte(buffer, 0, obj);

        Assert.Equal(2, obj.ByteValue);
    }

    //--------------------------------------------------------------------------------
    // Fix
    //--------------------------------------------------------------------------------

    [Fact]
    public void CoverageFix()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new MapByteAttribute(-1));
    }

    //--------------------------------------------------------------------------------
    // Helper
    //--------------------------------------------------------------------------------

    [Map(1)]
    internal sealed class ByteAttributeObject
    {
        [MapByte(0)]
        public byte ByteValue { get; set; }
    }
}
