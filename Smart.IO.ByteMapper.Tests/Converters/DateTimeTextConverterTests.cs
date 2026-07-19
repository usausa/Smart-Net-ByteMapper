namespace Smart.IO.ByteMapper.Converters;

using System.Text;

public class DateTimeTextConverterTests
{
    // ---- DateTime ----

    [Fact]
    public void WhenDateTimeReadValidDateThenReturnsDateTime()
    {
        var converter = new DateTimeTextConverter<DateTime>(8, "yyyyMMdd");
        var buffer = "20240315"u8.ToArray();
        Assert.Equal(new DateTime(2024, 3, 15), converter.Read(buffer));
    }

    [Fact]
    public void WhenDateTimeReadAllFillerThenReturnsNull()
    {
        var converter = new DateTimeTextConverter<DateTime>(8, "yyyyMMdd");
        var buffer = "        "u8.ToArray();
        Assert.Null(converter.Read(buffer));
    }

    [Fact]
    public void WhenDateTimeWriteNullThenFillerFilled()
    {
        var converter = new DateTimeTextConverter<DateTime>(8, "yyyyMMdd");
        var buffer = new byte[8];
        converter.Write(buffer, null);
        Assert.Equal("        ", Encoding.ASCII.GetString(buffer));
    }

    [Fact]
    public void WhenDateTimeWriteDefaultValueThenValueWritten()
    {
        var converter = new DateTimeTextConverter<DateTime>(8, "yyyyMMdd");
        var buffer = new byte[8];
        converter.Write(buffer, default(DateTime));
        // default DateTime は null ではなく値 "00010101" として書き込まれる
        Assert.Equal("00010101", Encoding.ASCII.GetString(buffer));
    }

    [Fact]
    public void WhenDateTimeNullRoundTripThenNull()
    {
        var converter = new DateTimeTextConverter<DateTime>(8, "yyyyMMdd");
        var buffer = new byte[8];
        converter.Write(buffer, null);
        Assert.Null(converter.Read(buffer));
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

    [Fact]
    public void WhenDateOnlyReadAllFillerThenReturnsNull()
    {
        var converter = new DateTimeTextConverter<DateOnly>(8, "yyyyMMdd");
        var buffer = "        "u8.ToArray();
        Assert.Null(converter.Read(buffer));
    }

    [Fact]
    public void WhenDateOnlyWriteNullThenFillerFilled()
    {
        var converter = new DateTimeTextConverter<DateOnly>(8, "yyyyMMdd");
        var buffer = new byte[8];
        converter.Write(buffer, null);
        Assert.Equal("        ", Encoding.ASCII.GetString(buffer));
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

    [Fact]
    public void WhenTimeOnlyReadAllFillerThenReturnsNull()
    {
        var converter = new DateTimeTextConverter<TimeOnly>(6, "HHmmss");
        var buffer = "      "u8.ToArray();
        Assert.Null(converter.Read(buffer));
    }

    [Fact]
    public void WhenTimeOnlyWriteNullThenFillerFilled()
    {
        var converter = new DateTimeTextConverter<TimeOnly>(6, "HHmmss");
        var buffer = new byte[6];
        converter.Write(buffer, null);
        Assert.Equal("      ", Encoding.ASCII.GetString(buffer));
    }

    // ---- DateTimeOffset ----

    [Fact]
    public void WhenDateTimeOffsetReadAllFillerThenReturnsNull()
    {
        var converter = new DateTimeTextConverter<DateTimeOffset>(8, "yyyyMMdd");
        var buffer = "        "u8.ToArray();
        Assert.Null(converter.Read(buffer));
    }

    [Fact]
    public void WhenDateTimeOffsetWriteNullThenFillerFilled()
    {
        var converter = new DateTimeTextConverter<DateTimeOffset>(8, "yyyyMMdd");
        var buffer = new byte[8];
        converter.Write(buffer, null);
        Assert.Equal("        ", Encoding.ASCII.GetString(buffer));
    }

    // ---- custom filler ----

    [Fact]
    public void WhenCustomFillerAllFilledThenReturnsNull()
    {
        var converter = new DateTimeTextConverter<DateTime>(8, "yyyyMMdd", filler: 0x30);
        // フィラーで全体が埋まった場合は null を返す
        var buffer = "00000000"u8.ToArray();
        Assert.Null(converter.Read(buffer));
    }

    // ---- 大きいフィールド（ArrayPool経路） ----

    [Fact]
    public void WhenLargeFieldDateTimeRoundTripThenSameValue()
    {
        var converter = new DateTimeTextConverter<DateTime>(300, "yyyyMMddHHmmss");
        var dt = new DateTime(2024, 6, 1, 12, 30, 59);
        var buffer = new byte[300];
        converter.Write(buffer, dt);
        Assert.Equal(dt, converter.Read(buffer));
    }

    // ---- Size ----

    [Fact]
    public void WhenSizeMatchesFormatLength()
    {
        var converter = new DateTimeTextConverter<DateTime>(14, "yyyyMMddHHmmss");
        Assert.Equal(14, converter.Size);
    }
}
