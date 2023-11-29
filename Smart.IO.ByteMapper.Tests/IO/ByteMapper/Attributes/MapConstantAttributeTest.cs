// ReSharper disable UseUtf8StringLiteral
namespace Smart.IO.ByteMapper.Attributes;

using Xunit;

public class MapConstantAttributeTest
{
    //--------------------------------------------------------------------------------
    // Attribute
    //--------------------------------------------------------------------------------

    [Fact]
    public void MapByConstantAttribute()
    {
#pragma warning disable IDE0230
        var mapperFactory = new MapperFactoryConfig()
            .DefaultDelimiter(0x0D, 0x0A)
            .CreateMapByAttribute<ConstAttributeObject>()
            .ToMapperFactory();
#pragma warning restore IDE0230
        var mapper = mapperFactory.Create<ConstAttributeObject>();

        var buffer = new byte[mapper.Size];
        var obj = new ConstAttributeObject();

        // Write
        mapper.ToByte(buffer, 0, obj);

        Assert.Equal("12\r\n"u8.ToArray(), buffer);
    }

    //--------------------------------------------------------------------------------
    // Fix
    //--------------------------------------------------------------------------------

    [Fact]
    public void CoverageFix()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new MapConstantAttribute(-1, []));
        Assert.Throws<ArgumentNullException>(() => new MapConstantAttribute(0, null));
    }

    //--------------------------------------------------------------------------------
    // Helper
    //--------------------------------------------------------------------------------

#pragma warning disable IDE0300
    [Map(4, UseDelimiter = true)]
    [MapConstant(0, new byte[] { 0x31, 0x32 })]
    internal sealed class ConstAttributeObject
    {
    }
#pragma warning restore IDE0300
}
