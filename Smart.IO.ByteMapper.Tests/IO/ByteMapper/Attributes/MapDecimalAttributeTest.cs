namespace Smart.IO.ByteMapper.Attributes;

using System.Text;

using Xunit;

public class MapDecimalAttributeTest
{
    //--------------------------------------------------------------------------------
    // Attribute
    //--------------------------------------------------------------------------------

    [Fact]
    public void MapByDecimalAttribute()
    {
        var mapperFactory = new MapperFactoryConfig()
            .UseOptionsDefault()
            .DefaultDelimiter(null)
            .DefaultZeroFill(false)
            .DefaultNumberPadding(Padding.Left)
            .DefaultNumberFiller(0x20)
            .CreateMapByAttribute<DecimalAttributeObject>()
            .ToMapperFactory();
        var mapper = mapperFactory.Create<DecimalAttributeObject>();

        var buffer = new byte[mapper.Size];
        var obj = new DecimalAttributeObject
        {
            DecimalValue = 1234.5m,
            PaddingRightDecimalValue = -1m,
            ZeroFillDecimalValue = 1m
        };

        // Write
        mapper.ToByte(buffer, 0, obj);
        Assert.Equal(
            Encoding.ASCII.GetBytes(
                "  1,234.50" +
                "______" +
                "-1    " +
                "000001"),
            buffer);

        // Read
        mapper.FromByte(
            Encoding.ASCII.GetBytes(
                "   2,345.6" +
                "_____2" +
                "-2    " +
                "000002"),
            obj);

        Assert.Equal(2345.60m, obj.DecimalValue);
        Assert.Equal(2m, obj.NullableDecimalValue);
        Assert.Equal(-2m, obj.PaddingRightDecimalValue);
        Assert.Equal(2m, obj.ZeroFillDecimalValue);

        mapper.FromByte(
            Encoding.ASCII.GetBytes(
                "          " +
                "______" +
                "      " +
                "000000"),
            obj);

        Assert.Equal(0m, obj.DecimalValue);
        Assert.Null(obj.NullableDecimalValue);
        Assert.Equal(0m, obj.PaddingRightDecimalValue);
        Assert.Equal(0m, obj.ZeroFillDecimalValue);
    }

    //--------------------------------------------------------------------------------
    // Fix
    //--------------------------------------------------------------------------------

    [Fact]
    public void CoverageFix()
    {
        var attribute = new MapDecimalAttribute(0, 0);

        Assert.Throws<NotSupportedException>(() => attribute.UseGrouping);
        Assert.Throws<NotSupportedException>(() => attribute.GroupingSize);
        Assert.Throws<NotSupportedException>(() => attribute.Padding);
        Assert.Throws<NotSupportedException>(() => attribute.ZeroFill);
        Assert.Throws<NotSupportedException>(() => attribute.Filler);

        Assert.Throws<ArgumentOutOfRangeException>(() => new MapDecimalAttribute(0, -1, 0));
        Assert.Throws<ArgumentException>(() => new MapDecimalAttribute(0, 0, 1));
    }

    //--------------------------------------------------------------------------------
    // Attribute
    //--------------------------------------------------------------------------------

    [Map(28)]
    internal sealed class DecimalAttributeObject
    {
        [MapDecimal(0, 10, 2, UseGrouping = true, GroupingSize = 3)]
        public decimal DecimalValue { get; set; }

        [MapDecimal(10, 6, Filler = (byte)'_')]
        public decimal? NullableDecimalValue { get; set; }

        [MapDecimal(16, 6, Padding = Padding.Right)]
        public decimal PaddingRightDecimalValue { get; set; }

        [MapDecimal(22, 6, ZeroFill = true)]
        public decimal ZeroFillDecimalValue { get; set; }
    }
}
