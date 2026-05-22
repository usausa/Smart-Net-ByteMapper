namespace Smart.IO.ByteMapper.Fast.Converters;

using System.Text;

public class FastDecimalConverterAdditionalTests
{
    // ---- null書き込み ----

    [Fact]
    public void WhenWriteNullThenFillerFilled()
    {
        var converter = new FastDecimalConverter(10, filler: 0x20);
        Span<byte> buffer = stackalloc byte[10];
        converter.Write(buffer, null);
        Assert.Equal("          ", Encoding.ASCII.GetString(buffer));
    }

    // ---- 右パディング ----

    [Fact]
    public void WhenWriteRightPaddingThenFormattedCorrectly()
    {
        var converter = new FastDecimalConverter(10, scale: 2, padding: Padding.Right);
        Span<byte> buffer = stackalloc byte[10];
        converter.Write(buffer, 12.34m);
        Assert.Equal("12.34     ", Encoding.ASCII.GetString(buffer));
    }

    // ---- groupingSize ----

    [Fact]
    public void WhenWriteWithGroupingSizeThenFormattedCorrectly()
    {
        var converter = new FastDecimalConverter(14, scale: 2, groupingSize: 3, padding: Padding.Left);
        Span<byte> buffer = stackalloc byte[14];
        converter.Write(buffer, 1234567.89m);
        Assert.Equal("  1,234,567.89", Encoding.ASCII.GetString(buffer));
    }

    // ---- 負数 ----

    [Fact]
    public void WhenReadNegativeThenReturnsNegativeDecimal()
    {
        var converter = new FastDecimalConverter(12);
        var buffer = "     -999.99"u8.ToArray().AsSpan();
        Assert.Equal(-999.99m, converter.Read(buffer));
    }

    [Fact]
    public void WhenWriteNegativeThenFormattedCorrectly()
    {
        var converter = new FastDecimalConverter(10, scale: 2, padding: Padding.Left);
        Span<byte> buffer = stackalloc byte[10];
        converter.Write(buffer, -12.34m);
        Assert.Equal("    -12.34", Encoding.ASCII.GetString(buffer));
    }

    // ---- zerofill ----

    [Fact]
    public void WhenReadWithZerofillThenCorrectValue()
    {
        var converter = new FastDecimalConverter(10, scale: 2);
        var buffer = "0000012.34"u8.ToArray().AsSpan();
        Assert.Equal(12.34m, converter.Read(buffer));
    }

    // ---- Size ----

    [Fact]
    public void WhenSizeMatchesLength()
    {
        var converter = new FastDecimalConverter(12);
        Assert.Equal(12, converter.Size);
    }
}
