namespace Smart.IO.ByteMapper.Attributes;

using System.Globalization;
using System.Text;

public sealed class MapNumberTextAttributeTest
{
    //--------------------------------------------------------------------------------
    // Attribute
    //--------------------------------------------------------------------------------

    [Fact]
    public void MapByNumberTextAttribute()
    {
        var mapperFactory = new MapperFactoryConfig()
            .DefaultDelimiter(null)
            .DefaultTrim(true)
            .DefaultNumberTextEncoding(Encoding.ASCII)
            .DefaultNumberTextProvider(CultureInfo.InvariantCulture)
            .DefaultNumberTextNumberStyle(NumberStyles.Integer)
            .DefaultNumberTextDecimalStyle(NumberStyles.Any)
            .DefaultNumberTextPadding(Padding.Left)
            .DefaultNumberTextFiller(0x20)
            .CreateMapByAttribute<NumberTextAttributeObject>()
            .ToMapperFactory();
        var mapper = mapperFactory.Create<NumberTextAttributeObject>();

        var buffer = new byte[mapper.Size];
        var obj = new NumberTextAttributeObject
        {
            IntValue = 1,
            CustomIntValue = -1,
            LongValue = 1L,
            CustomLongValue = -1L,
            ShortValue = 1,
            CustomShortValue = -1,
            DecimalValue = 12.34m,
            CustomDecimalValue = -1.2m
        };

        // Write
        mapper.ToByte(buffer, 0, obj);
        Assert.Equal(
            Encoding.ASCII.GetBytes(
                "   1" +
                "    " +
                "-01_" +
                "     1" +
                "      " +
                "-01___" +
                " 1" +
                "  " +
                "-1" +
                " 12.34" +
                "      " +
                "-1.20_"),
            buffer);

        // Read
        mapper.FromByte(
            Encoding.ASCII.GetBytes(
                "   2" +
                "   2" +
                "-2__" +
                "     2" +
                "     2" +
                "-2____" +
                " 2" +
                " 2" +
                "-2" +
                "  23.4" +
                "  23.4" +
                " -23.4"),
            obj);

        Assert.Equal(2, obj.IntValue);
        Assert.Equal(2, obj.NullableIntValue);
        Assert.Equal(-2, obj.CustomIntValue);
        Assert.Equal(2L, obj.LongValue);
        Assert.Equal(2L, obj.NullableLongValue);
        Assert.Equal(-2L, obj.CustomLongValue);
        Assert.Equal((short)2, obj.ShortValue);
        Assert.Equal((short)2, obj.NullableShortValue);
        Assert.Equal((short)-2, obj.CustomShortValue);
        Assert.Equal(23.4m, obj.DecimalValue);
        Assert.Equal(23.4m, obj.NullableDecimalValue);
        Assert.Equal(-23.4m, obj.CustomDecimalValue);
    }

    //--------------------------------------------------------------------------------
    // Fix
    //--------------------------------------------------------------------------------

    [Fact]
    public void CoverageFix()
    {
        var attribute = new MapNumberTextAttribute(0, 0);

        Assert.Throws<NotSupportedException>(() => attribute.Format);
        Assert.Throws<NotSupportedException>(() => attribute.CodePage);
        Assert.Throws<NotSupportedException>(() => attribute.EncodingName);
        Assert.Throws<NotSupportedException>(() => attribute.Trim);
        Assert.Throws<NotSupportedException>(() => attribute.Padding);
        Assert.Throws<NotSupportedException>(() => attribute.Filler);
        Assert.Throws<NotSupportedException>(() => attribute.Style);
        Assert.Throws<NotSupportedException>(() => attribute.Culture);

        Assert.Throws<ArgumentOutOfRangeException>(() => new MapNumberTextAttribute(-1, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new MapNumberTextAttribute(0, -1));
    }

    //--------------------------------------------------------------------------------
    // Helper
    //--------------------------------------------------------------------------------

    [Map(54)]
    internal sealed class NumberTextAttributeObject
    {
        // int

        [MapNumberText(0, 4)]
        public int IntValue { get; set; }

        [MapNumberText(4, 4)]
        public int? NullableIntValue { get; set; }

        [MapNumberText(8, 4, Format = "D2", EncodingName = "ASCII", Trim = true, Padding = Padding.Right, Filler = (byte)'_', Style = NumberStyles.Any, Culture = Culture.Invariant)]
        public int CustomIntValue { get; set; }

        // long

        [MapNumberText(12, 6)]
        public long LongValue { get; set; }

        [MapNumberText(18, 6)]
        public long? NullableLongValue { get; set; }

        [MapNumberText(24, 6, Format = "D2", CodePage = 20127, Trim = true, Padding = Padding.Right, Filler = (byte)'_', Style = NumberStyles.Any, Culture = Culture.Invariant)]
        public long CustomLongValue { get; set; }

        // short

        [MapNumberText(30, 2)]
        public short ShortValue { get; set; }

        [MapNumberText(32, 2)]
        public short? NullableShortValue { get; set; }

        [MapNumberText(34, 2, Format = "D1", EncodingName = "ASCII", Trim = true, Padding = Padding.Right, Filler = (byte)'_', Style = NumberStyles.Any, Culture = Culture.Invariant)]
        public short CustomShortValue { get; set; }

        // decimal

        [MapNumberText(36, 6)]
        public decimal DecimalValue { get; set; }

        [MapNumberText(42, 6)]
        public decimal? NullableDecimalValue { get; set; }

        [MapNumberText(48, 6, Format = "0.00", CodePage = 20127, Trim = true, Padding = Padding.Right, Filler = (byte)'_', Style = NumberStyles.Any, Culture = Culture.Invariant)]
        public decimal CustomDecimalValue { get; set; }
    }
}
