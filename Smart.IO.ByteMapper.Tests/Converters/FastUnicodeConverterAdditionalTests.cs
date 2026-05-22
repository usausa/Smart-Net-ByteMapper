namespace Smart.IO.ByteMapper.Fast.Converters;

public class FastUnicodeConverterAdditionalTests
{
    // ---- trim=false ----

    [Fact]
    public void WhenReadWithTrimFalseThenNotStripped()
    {
        var converter = new FastUnicodeConverter(20, trim: false);
        Span<byte> buffer = stackalloc byte[20];
        converter.Write(buffer, "Hi");
        // trim=falseなので右のスペースも含まれる
        var result = converter.Read(buffer);
        Assert.StartsWith("Hi", result);
        Assert.Equal(10, result.Length); // 20バイト / 2 = 10文字
    }

    // ---- 左パディング ----

    [Fact]
    public void WhenWriteLeftPaddingThenPaddedLeft()
    {
        var converter = new FastUnicodeConverter(10, padding: Padding.Left);
        Span<byte> buffer = stackalloc byte[10];
        converter.Write(buffer, "AB");
        var result = converter.Read(buffer);
        Assert.Equal("AB", result);
    }

    [Fact]
    public void WhenReadLeftPaddingWithTrimThenStripped()
    {
        var converter = new FastUnicodeConverter(10, trim: true, padding: Padding.Left);
        Span<byte> buffer = stackalloc byte[10];
        converter.Write(buffer, "AB");
        Assert.Equal("AB", converter.Read(buffer));
    }

    // ---- 文字列が領域より長い場合は切り捨て ----

    [Fact]
    public void WhenWriteLongerThanSizeThenTruncated()
    {
        var converter = new FastUnicodeConverter(4); // 4バイト = 2文字
        Span<byte> buffer = stackalloc byte[4];
        converter.Write(buffer, "ABCDE");
        var result = converter.Read(buffer);
        Assert.Equal("AB", result);
    }

    // ---- 空文字列読み取り ----

    [Fact]
    public void WhenReadAllFillerThenReturnsEmptyString()
    {
        var converter = new FastUnicodeConverter(10, trim: true);
        Span<byte> buffer = stackalloc byte[10];
        converter.Write(buffer, null);
        Assert.Equal(string.Empty, converter.Read(buffer));
    }

    // ---- カスタムfiller ----

    [Fact]
    public void WhenWriteNullWithCustomFillerThenFillerFilled()
    {
        var converter = new FastUnicodeConverter(4, filler: '*');
        Span<byte> buffer = stackalloc byte[4];
        converter.Write(buffer, null);
        // 4バイト = 2文字分の '*' (U+002A, LE: 0x2A 0x00)
        Assert.Equal((byte)'*', buffer[0]);
        Assert.Equal(0x00, buffer[1]);
        Assert.Equal((byte)'*', buffer[2]);
        Assert.Equal(0x00, buffer[3]);
    }

    // ---- Size ----

    [Fact]
    public void WhenSizeMatchesLength()
    {
        var converter = new FastUnicodeConverter(20);
        Assert.Equal(20, converter.Size);
    }
}
