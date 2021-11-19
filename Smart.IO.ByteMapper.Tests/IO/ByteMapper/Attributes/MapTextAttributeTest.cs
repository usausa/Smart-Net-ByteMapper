namespace Smart.IO.ByteMapper.Attributes;

using System;
using System.Text;

using Xunit;

public class MapTextAttributeTest
{
    //--------------------------------------------------------------------------------
    // Attribute
    //--------------------------------------------------------------------------------

    [Fact]
    public void MapByStringAttribute()
    {
        var mapperFactory = new MapperFactoryConfig()
            .DefaultDelimiter(null)
            .DefaultEncoding(Encoding.ASCII)
            .DefaultTrim(true)
            .DefaultTextPadding(Padding.Right)
            .DefaultTextFiller(0x20)
            .CreateMapByAttribute<TextAttributeObject>()
            .ToMapperFactory();
        var mapper = mapperFactory.Create<TextAttributeObject>();

        var buffer = new byte[mapper.Size];
        var obj = new TextAttributeObject();

        // Write
        mapper.ToByte(buffer, 0, obj);

        Assert.Equal(Encoding.ASCII.GetBytes("    ______"), buffer);

        // Read
        mapper.FromByte(Encoding.ASCII.GetBytes("12  __AB*_"), 0, obj);

        Assert.Equal("12", obj.StringValue);
        Assert.Equal("__AB", obj.CustomStringValue);
        Assert.Equal("*", obj.CustomStringValue2);
    }

    //--------------------------------------------------------------------------------
    // Fix
    //--------------------------------------------------------------------------------

    [Fact]
    public void CoverageFix()
    {
        var attribute = new MapTextAttribute(0, 0);

        Assert.Throws<NotSupportedException>(() => attribute.CodePage);
        Assert.Throws<NotSupportedException>(() => attribute.EncodingName);
        Assert.Throws<NotSupportedException>(() => attribute.Trim);
        Assert.Throws<NotSupportedException>(() => attribute.Padding);
        Assert.Throws<NotSupportedException>(() => attribute.Filler);

        Assert.Throws<ArgumentOutOfRangeException>(() => new MapTextAttribute(-1, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new MapTextAttribute(0, -1));
    }

    //--------------------------------------------------------------------------------
    // Helper
    //--------------------------------------------------------------------------------

    [Map(10)]
    internal class TextAttributeObject
    {
        [MapText(0, 4)]
        public string StringValue { get; set; }

        [MapText(4, 4, EncodingName = "ASCII", Trim = false, Padding = Padding.Left, Filler = (byte)'_')]
        public string CustomStringValue { get; set; }

        [MapText(8, 2, CodePage = 20127, Filler = (byte)'_')]
        public string CustomStringValue2 { get; set; }
    }
}
