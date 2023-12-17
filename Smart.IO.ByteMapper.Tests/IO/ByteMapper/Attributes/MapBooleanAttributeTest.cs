// ReSharper disable UseUtf8StringLiteral
namespace Smart.IO.ByteMapper.Attributes;

public sealed class MapBooleanAttributeTest
{
    private const byte Filler = 0x00;

    private const byte True = (byte)'1';

    private const byte False = (byte)'0';

    private const byte Yes = (byte)'Y';

    private const byte No = (byte)'N';

    //--------------------------------------------------------------------------------
    // Attribute
    //--------------------------------------------------------------------------------

    [Fact]
    public void MapByBooleanAttribute()
    {
        var mapperFactory = new MapperFactoryConfig()
            .DefaultDelimiter(null)
            .DefaultFiller(Filler)
            .DefaultTrueValue(True)
            .DefaultFalseValue(False)
            .CreateMapByAttribute<BooleanAttributeObject>()
            .ToMapperFactory();
        var mapper = mapperFactory.Create<BooleanAttributeObject>();

        var buffer = new byte[mapper.Size];
        var obj = new BooleanAttributeObject();

        // Write
        mapper.ToByte(buffer, 0, obj);

        Assert.Equal((byte[])[False, Filler, No, Filler], buffer);

        // Read
        mapper.FromByte([True, True, Yes, Yes], 0, obj);

        Assert.True(obj.BooleanValue);
        Assert.True(obj.NullableBooleanValue);
        Assert.True(obj.CustomBooleanValue);
        Assert.True(obj.CustomNullableBooleanValue);

        // Read default
        mapper.FromByte([Filler, Filler, Filler, Filler], 0, obj);

        Assert.False(obj.BooleanValue);
        Assert.Null(obj.NullableBooleanValue);
        Assert.False(obj.CustomBooleanValue);
        Assert.Null(obj.CustomNullableBooleanValue);
    }

    //--------------------------------------------------------------------------------
    // Fix
    //--------------------------------------------------------------------------------

    [Fact]
    public void CoverageFix()
    {
        var attribute = new MapBooleanAttribute(0);

        Assert.Throws<NotSupportedException>(() => attribute.TrueValue);
        Assert.Throws<NotSupportedException>(() => attribute.FalseValue);
        Assert.Throws<NotSupportedException>(() => attribute.NullValue);

        Assert.Throws<ArgumentOutOfRangeException>(() => new MapBooleanAttribute(-1));
    }

    //--------------------------------------------------------------------------------
    // Helper
    //--------------------------------------------------------------------------------

    [Map(4)]
    internal sealed class BooleanAttributeObject
    {
        [MapBoolean(0)]
        public bool BooleanValue { get; set; }

        [MapBoolean(1)]
        public bool? NullableBooleanValue { get; set; }

        [MapBoolean(2, TrueValue = Yes, FalseValue = No)]
        public bool CustomBooleanValue { get; set; }

        [MapBoolean(3, TrueValue = Yes, FalseValue = No, NullValue = Filler)]
        public bool? CustomNullableBooleanValue { get; set; }
    }
}
