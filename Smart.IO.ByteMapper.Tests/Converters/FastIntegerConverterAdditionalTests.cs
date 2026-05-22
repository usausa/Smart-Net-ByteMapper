namespace Smart.IO.ByteMapper.Converters;

using System;
using System.Text;

using Xunit;

public class FastIntegerConverterAdditionalTests
{
    // ---- null書き込み ----

    [Fact]
    public void WhenIntWriteNullThenFillerFilled()
    {
        var converter = new FastIntegerConverter<int>(6, filler: 0x20);
        Span<byte> buffer = stackalloc byte[6];
        converter.Write(buffer, null);
        Assert.Equal("      ", Encoding.ASCII.GetString(buffer));
    }

    [Fact]
    public void WhenShortWriteNullThenFillerFilled()
    {
        var converter = new FastIntegerConverter<short>(4, filler: 0x20);
        Span<byte> buffer = stackalloc byte[4];
        converter.Write(buffer, null);
        Assert.Equal("    ", Encoding.ASCII.GetString(buffer));
    }

    [Fact]
    public void WhenLongWriteNullThenFillerFilled()
    {
        var converter = new FastIntegerConverter<long>(8, filler: 0x20);
        Span<byte> buffer = stackalloc byte[8];
        converter.Write(buffer, null);
        Assert.Equal("        ", Encoding.ASCII.GetString(buffer));
    }

    // ---- short ----

    [Fact]
    public void WhenShortReadValidValueThenReturnsShort()
    {
        var converter = new FastIntegerConverter<short>(5);
        var buffer = "  123"u8.ToArray().AsSpan();
        Assert.Equal((short)123, converter.Read(buffer));
    }

    [Fact]
    public void WhenShortReadEmptyThenReturnsNull()
    {
        var converter = new FastIntegerConverter<short>(4);
        var buffer = "    "u8.ToArray().AsSpan();
        Assert.Null(converter.Read(buffer));
    }

    [Fact]
    public void WhenShortRoundTripThenSameValue()
    {
        var converter = new FastIntegerConverter<short>(6);
        Span<byte> buffer = stackalloc byte[6];
        converter.Write(buffer, short.MaxValue);
        Assert.Equal(short.MaxValue, converter.Read(buffer));
    }

    // ---- long ----

    [Fact]
    public void WhenLongReadValidValueThenReturnsLong()
    {
        var converter = new FastIntegerConverter<long>(21);
        var buffer = "  1234567890123456789"u8.ToArray().AsSpan();
        Assert.Equal(1234567890123456789L, converter.Read(buffer));
    }

    [Fact]
    public void WhenLongReadEmptyThenReturnsNull()
    {
        var converter = new FastIntegerConverter<long>(6);
        var buffer = "      "u8.ToArray().AsSpan();
        Assert.Null(converter.Read(buffer));
    }

    [Fact]
    public void WhenLongRoundTripNegativeThenSameValue()
    {
        var converter = new FastIntegerConverter<long>(20);
        Span<byte> buffer = stackalloc byte[20];
        converter.Write(buffer, -1234567890123456789L);
        Assert.Equal(-1234567890123456789L, converter.Read(buffer));
    }

    // ---- 右パディング ----

    [Fact]
    public void WhenIntWriteRightPaddingThenFormattedCorrectly()
    {
        var converter = new FastIntegerConverter<int>(8, padding: Padding.Right);
        Span<byte> buffer = stackalloc byte[8];
        converter.Write(buffer, 123);
        Assert.Equal("123     ", Encoding.ASCII.GetString(buffer));
    }

    // ---- zerofill + 負数 ----

    [Fact]
    public void WhenIntWriteZerofillNegativeThenFormattedCorrectly()
    {
        var converter = new FastIntegerConverter<int>(8, zerofill: true);
        Span<byte> buffer = stackalloc byte[8];
        converter.Write(buffer, -42);
        Assert.Equal("-0000042", Encoding.ASCII.GetString(buffer));
    }

    // ---- Size ----

    [Fact]
    public void WhenSizeMatchesLength()
    {
        var converter = new FastIntegerConverter<int>(10);
        Assert.Equal(10, converter.Size);
    }
}
