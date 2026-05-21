namespace Smart.IO.ByteMapper.Converters;

using System;
using System.Text;

using Xunit;

public class DateTimeTextConverterTests
{
    // ---- DateTime ----

    [Fact]
    public void WhenDateTimeReadValidDateThenReturnsDateTime()
    {
        var converter = new DateTimeTextConverter<DateTime>(8, "yyyyMMdd");
        var buffer = Encoding.ASCII.GetBytes("20240315");
        Assert.Equal(new DateTime(2024, 3, 15), converter.Read(buffer));
    }

    [Fact]
    public void WhenDateTimeReadAllFillerThenReturnsDefault()
    {
        var converter = new DateTimeTextConverter<DateTime>(8, "yyyyMMdd");
        var buffer = new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 };
        Assert.Equal(default(DateTime), converter.Read(buffer));
    }

    [Fact]
    public void WhenDateTimeWriteNullFillsThenFillerFilled()
    {
        var converter = new DateTimeTextConverter<DateTime>(8, "yyyyMMdd");
        var buffer = new byte[8];
        converter.Write(buffer, default(DateTime));
        // default DateTime は "00010101" に書き込まれる（fillerではなく値として）
        Assert.Equal("00010101", Encoding.ASCII.GetString(buffer));
    }

    [Fact]
    public void WhenDateTimeRoundTripThenSameValue()
    {
        var converter = new DateTimeTextConverter<DateTime>(14, "yyyyMMddHHmmss");
        var dt = new DateTime(2024, 6, 1, 12, 30, 59);
        var buffer = new byte[14];
        converter.Write(buffer, dt);
        Assert.Equal(dt, converter.Read(buffer));
    }

    // ---- DateOnly ----

    [Fact]
    public void WhenDateOnlyRoundTripThenSameValue()
    {
        var converter = new DateTimeTextConverter<DateOnly>(8, "yyyyMMdd");
        var date = new DateOnly(2025, 12, 31);
        var buffer = new byte[8];
        converter.Write(buffer, date);
        Assert.Equal(date, converter.Read(buffer));
    }

    // ---- TimeOnly ----

    [Fact]
    public void WhenTimeOnlyRoundTripThenSameValue()
    {
        var converter = new DateTimeTextConverter<TimeOnly>(6, "HHmmss");
        var time = new TimeOnly(9, 15, 30);
        var buffer = new byte[6];
        converter.Write(buffer, time);
        Assert.Equal(time, converter.Read(buffer));
    }

    // ---- DateTimeOffset ----

    [Fact]
    public void WhenDateTimeOffsetReadAllFillerThenReturnsDefault()
    {
        var converter = new DateTimeTextConverter<DateTimeOffset>(8, "yyyyMMdd");
        var buffer = new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 };
        Assert.Equal(default(DateTimeOffset), converter.Read(buffer));
    }

    // ---- custom filler ----

    [Fact]
    public void WhenCustomFillerThenFillerFilled()
    {
        var converter = new DateTimeTextConverter<DateTime>(8, "yyyyMMdd", filler: 0x30);
        // フィラーで全体が埋まった場合はdefaultを返す
        var buffer = new byte[] { 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30 };
        Assert.Equal(default(DateTime), converter.Read(buffer));
    }

    // ---- Size ----

    [Fact]
    public void WhenSizeMatchesFormatLength()
    {
        var converter = new DateTimeTextConverter<DateTime>(14, "yyyyMMddHHmmss");
        Assert.Equal(14, converter.Size);
    }
}
