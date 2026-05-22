namespace Smart.IO.ByteMapper.Converters;

using System.Globalization;
using System.Text;

public class NumberTextConverterTests
{
    // ---- int ----

    [Fact]
    public void WhenIntReadValidValueThenReturnsInt()
    {
        var converter = new NumberTextConverter<int>(8);
        var buffer = "    1234"u8.ToArray();
        Assert.Equal(1234, converter.Read(buffer));
    }

    [Fact]
    public void WhenIntReadAllFillerThenReturnsDefault()
    {
        var converter = new NumberTextConverter<int>(4);
        var buffer = "    "u8.ToArray();
        Assert.Equal(0, converter.Read(buffer));
    }

    [Fact]
    public void WhenIntWriteThenFormattedCorrectly()
    {
        var converter = new NumberTextConverter<int>(8, padding: Padding.Left);
        var buffer = new byte[8];
        converter.Write(buffer, 42);
        Assert.Equal("      42", Encoding.ASCII.GetString(buffer));
    }

    [Fact]
    public void WhenIntRoundTripThenSameValue()
    {
        var converter = new NumberTextConverter<int>(10);
        var buffer = new byte[10];
        converter.Write(buffer, -9999);
        Assert.Equal(-9999, converter.Read(buffer));
    }

    // ---- long ----

    [Fact]
    public void WhenLongReadValidValueThenReturnsLong()
    {
        var converter = new NumberTextConverter<long>(21);
        var buffer = "  1234567890123456789"u8.ToArray();
        Assert.Equal(1234567890123456789L, converter.Read(buffer));
    }

    [Fact]
    public void WhenLongRoundTripThenSameValue()
    {
        var converter = new NumberTextConverter<long>(21);
        var buffer = new byte[21];
        converter.Write(buffer, long.MaxValue);
        Assert.Equal(long.MaxValue, converter.Read(buffer));
    }

    // ---- short ----

    [Fact]
    public void WhenShortRoundTripThenSameValue()
    {
        var converter = new NumberTextConverter<short>(6);
        var buffer = new byte[6];
        converter.Write(buffer, short.MaxValue);
        Assert.Equal(short.MaxValue, converter.Read(buffer));
    }

    // ---- decimal ----

    [Fact]
    public void WhenDecimalReadValidValueThenReturnsDecimal()
    {
        var converter = new NumberTextConverter<decimal>(12, style: NumberStyles.Number);
        var buffer = "    1234.56 "u8.ToArray();
        Assert.Equal(1234.56m, converter.Read(buffer));
    }

    [Fact]
    public void WhenDecimalRoundTripThenSameValue()
    {
        var converter = new NumberTextConverter<decimal>(14, style: NumberStyles.Number);
        var buffer = new byte[14];
        converter.Write(buffer, 9999.99m);
        Assert.Equal(9999.99m, converter.Read(buffer));
    }

    // ---- right padding ----

    [Fact]
    public void WhenIntWriteRightPaddingThenFormattedCorrectly()
    {
        var converter = new NumberTextConverter<int>(8, padding: Padding.Right);
        var buffer = new byte[8];
        converter.Write(buffer, 100);
        Assert.Equal("100     ", Encoding.ASCII.GetString(buffer));
    }

    // ---- Size ----

    [Fact]
    public void WhenSizeMatchesLength()
    {
        var converter = new NumberTextConverter<int>(10);
        Assert.Equal(10, converter.Size);
    }
}
