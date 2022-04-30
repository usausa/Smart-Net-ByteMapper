namespace Smart.IO.ByteMapper.Attributes;

using System.Text;

using Xunit;

public class MapConstantAttributeTest
{
    //--------------------------------------------------------------------------------
    // Attribute
    //--------------------------------------------------------------------------------

    [Fact]
    public void MapByConstantAttribute()
    {
        var mapperFactory = new MapperFactoryConfig()
            .DefaultDelimiter(0x0D, 0x0A)
            .CreateMapByAttribute<ConstAttributeObject>()
            .ToMapperFactory();
        var mapper = mapperFactory.Create<ConstAttributeObject>();

        var buffer = new byte[mapper.Size];
        var obj = new ConstAttributeObject();

        // Write
        mapper.ToByte(buffer, 0, obj);

        Assert.Equal(Encoding.ASCII.GetBytes("12\r\n"), buffer);
    }

    //--------------------------------------------------------------------------------
    // Fix
    //--------------------------------------------------------------------------------

    [Fact]
    public void CoverageFix()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new MapConstantAttribute(-1, Array.Empty<byte>()));
        Assert.Throws<ArgumentNullException>(() => new MapConstantAttribute(0, null));
    }

    //--------------------------------------------------------------------------------
    // Helper
    //--------------------------------------------------------------------------------

    [Map(4, UseDelimiter = true)]
    [MapConstant(0, new byte[] { 0x31, 0x32 })]
    internal class ConstAttributeObject
    {
    }
}
