namespace Smart.IO.ByteMapper.Converters;

using System.Text;

public class FastDateTimeOffsetConverterTests
{
    [Fact]
    public void WhenReadValidDateThenReturnsDateTimeOffset()
    {
        var converter = new FastDateTimeOffsetConverter("yyyyMMdd");
        var buffer = "20240315"u8.ToArray().AsSpan();
        var result = converter.Read(buffer);
        Assert.NotNull(result);
        Assert.Equal(2024, result.Value.Year);
        Assert.Equal(3, result.Value.Month);
        Assert.Equal(15, result.Value.Day);
    }

    [Fact]
    public void WhenReadEmptyThenReturnsNull()
    {
        var converter = new FastDateTimeOffsetConverter("yyyyMMdd");
        var buffer = "        "u8.ToArray().AsSpan();
        Assert.Null(converter.Read(buffer));
    }

    [Fact]
    public void WhenWriteNullThenFillsFiller()
    {
        var converter = new FastDateTimeOffsetConverter("yyyyMMdd");
        Span<byte> buffer = stackalloc byte[8];
        converter.Write(buffer, null);
        Assert.Equal("        ", Encoding.ASCII.GetString(buffer));
    }

    [Fact]
    public void WhenWriteWithUtcKindThenFormattedCorrectly()
    {
        var converter = new FastDateTimeOffsetConverter("yyyyMMddHHmmss", DateTimeKind.Utc);
        var dto = new DateTimeOffset(2024, 6, 1, 12, 0, 0, TimeSpan.Zero);
        Span<byte> buffer = stackalloc byte[14];
        converter.Write(buffer, dto);
        Assert.Equal("20240601120000", Encoding.ASCII.GetString(buffer));
    }

    [Fact]
    public void WhenReadWithUtcKindThenOffsetIsZero()
    {
        var converter = new FastDateTimeOffsetConverter("yyyyMMddHHmmss", DateTimeKind.Utc);
        var buffer = "20240601120000"u8.ToArray().AsSpan();
        var result = converter.Read(buffer);
        Assert.NotNull(result);
        Assert.Equal(TimeSpan.Zero, result.Value.Offset);
    }

    [Fact]
    public void WhenSizeMatchesFormatLength()
    {
        var converter = new FastDateTimeOffsetConverter("yyyyMMddHHmmss");
        Assert.Equal(14, converter.Size);
    }
}
