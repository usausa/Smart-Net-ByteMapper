namespace Smart.IO.ByteMapper.Converters;

using System.Text;

public class AsciiConverterAdditionalTests
{
    [Fact]
    public void WhenReadWithTrimFalseThenNotStripped()
    {
        var converter = new AsciiConverter(8, trim: false);
        var buffer = "Hi      "u8.ToArray();
        Assert.Equal("Hi      ", converter.Read(buffer));
    }

    [Fact]
    public void WhenReadLeftPaddingWithTrimThenStripped()
    {
        var converter = new AsciiConverter(8, trim: true, padding: Padding.Left);
        var buffer = "      Hi"u8.ToArray();
        Assert.Equal("Hi", converter.Read(buffer));
    }

    [Fact]
    public void WhenReadAllFillerWithTrimThenEmptyString()
    {
        var converter = new AsciiConverter(4, trim: true);
        var buffer = "    "u8.ToArray();
        Assert.Equal(string.Empty, converter.Read(buffer));
    }

    [Fact]
    public void WhenWriteExactLengthThenNoTruncation()
    {
        var converter = new AsciiConverter(4);
        Span<byte> buffer = stackalloc byte[4];
        converter.Write(buffer, "ABCD");
        Assert.Equal("ABCD", Encoding.ASCII.GetString(buffer));
    }

    [Fact]
    public void WhenWriteLongerThanSizeThenTruncated()
    {
        var converter = new AsciiConverter(4);
        Span<byte> buffer = stackalloc byte[4];
        converter.Write(buffer, "ABCDEFGH");
        Assert.Equal("ABCD", Encoding.ASCII.GetString(buffer));
    }

    [Fact]
    public void WhenWriteEmptyStringThenFillerFilled()
    {
        var converter = new AsciiConverter(4, filler: 0x20);
        Span<byte> buffer = stackalloc byte[4];
        converter.Write(buffer, string.Empty);
        Assert.All(buffer.ToArray(), b => Assert.Equal(0x20, b));
    }

    [Fact]
    public void WhenWriteLeftPaddingThenPaddedCorrectly()
    {
        var converter = new AsciiConverter(6, padding: Padding.Left);
        Span<byte> buffer = stackalloc byte[6];
        converter.Write(buffer, "AB");
        Assert.Equal("    AB", Encoding.ASCII.GetString(buffer));
    }

    [Fact]
    public void WhenSizeMatchesLength()
    {
        var converter = new AsciiConverter(12);
        Assert.Equal(12, converter.Size);
    }
}
