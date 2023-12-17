namespace Smart.IO.ByteMapper.Helpers;

using System.Text;

// ReSharper disable StringLiteralTypo
public sealed class DateTimeByteHelperTest
{
    [Fact]
    public void ParseDateTime()
    {
        // Default
        var buffer = "21991231235959999"u8.ToArray();
        Assert.True(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyyyMMddHHmmssfff", out _), DateTimeKind.Unspecified, out var value));
        Assert.Equal(new DateTime(2199, 12, 31, 23, 59, 59, 999), value);

        // Date only
        buffer = "21991231"u8.ToArray();
        Assert.True(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyyyMMdd", out _), DateTimeKind.Unspecified, out value));
        Assert.Equal(new DateTime(2199, 12, 31), value);

        // Short year
        buffer = "991231"u8.ToArray();
        Assert.True(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyMMdd", out _), DateTimeKind.Unspecified, out value));
        Assert.Equal(new DateTime(2099, 12, 31), value);

        // Current year
        buffer = "1231"u8.ToArray();
        Assert.True(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("MMdd", out _), DateTimeKind.Unspecified, out value));
        Assert.Equal(new DateTime(DateTime.Now.Year, 12, 31), value);

        // Default date
        buffer = "2199"u8.ToArray();
        Assert.True(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyyy", out _), DateTimeKind.Unspecified, out value));
        Assert.Equal(new DateTime(2199, 1, 1), value);

        // Short
        buffer = "219912312359591"u8.ToArray();
        Assert.True(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyyyMMddHHmmssf", out _), DateTimeKind.Unspecified, out value));
        Assert.Equal(new DateTime(2199, 12, 31, 23, 59, 59, 100), value);

        // 2ms
        buffer = "2199123123595912"u8.ToArray();
        Assert.True(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyyyMMddHHmmssff", out _), DateTimeKind.Unspecified, out value));
        Assert.Equal(new DateTime(2199, 12, 31, 23, 59, 59, 120), value);

        // Space
        buffer = "   1 1 1 0 0 0000"u8.ToArray();
        Assert.True(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyyyMMddHHmmssfff", out _), DateTimeKind.Unspecified, out value));
        Assert.Equal(new DateTime(1, 1, 1, 0, 0, 0, 0), value);

        // Format & trim
        buffer = " 2199/12/31 23:59:59.123 "u8.ToArray();
        Assert.True(DateTimeByteHelper.TryParseDateTime(buffer, 1, DateTimeByteHelper.ParseDateTimeFormat("yyyy/MM/dd HH:mm:ss.fff", out _), DateTimeKind.Unspecified, out value));
        Assert.Equal(new DateTime(2199, 12, 31, 23, 59, 59, 123), value);

        // Failed

        // Empty
        buffer = "        "u8.ToArray();
        Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyyyMMddHH", out _), DateTimeKind.Unspecified, out _));

        // Invalid Year
        buffer = "****0101000000000"u8.ToArray();
        Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyyyMMddHHmmssfff", out _), DateTimeKind.Unspecified, out _));

        buffer = " 0*00101000000000"u8.ToArray();
        Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyyyMMddHHmmssfff", out _), DateTimeKind.Unspecified, out _));

        buffer = "999990101000000000"u8.ToArray();
        Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyyyyMMddHHmmssfff", out _), DateTimeKind.Unspecified, out _));

        // Invalid Month
        buffer = "2000**01000000000"u8.ToArray();
        Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyyyMMddHHmmssfff", out _), DateTimeKind.Unspecified, out _));

        buffer = "20001301000000000"u8.ToArray();
        Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyyyMMddHHmmssfff", out _), DateTimeKind.Unspecified, out _));

        // Invalid Day
        buffer = "200001**000000000"u8.ToArray();
        Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyyyMMddHHmmssfff", out _), DateTimeKind.Unspecified, out _));

        buffer = "20000132000000000"u8.ToArray();
        Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyyyMMddHHmmssfff", out _), DateTimeKind.Unspecified, out _));

        // Invalid Hour
        buffer = "20000101**0000000"u8.ToArray();
        Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyyyMMddHHmmssfff", out _), DateTimeKind.Unspecified, out _));

        buffer = "20000101250000000"u8.ToArray();
        Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyyyMMddHHmmssfff", out _), DateTimeKind.Unspecified, out _));

        // Invalid Minute
        buffer = "2000010100**00000"u8.ToArray();
        Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyyyMMddHHmmssfff", out _), DateTimeKind.Unspecified, out _));

        buffer = "20000101006000000"u8.ToArray();
        Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyyyMMddHHmmssfff", out _), DateTimeKind.Unspecified, out _));

        // Invalid Second
        buffer = "200001010000**000"u8.ToArray();
        Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyyyMMddHHmmssfff", out _), DateTimeKind.Unspecified, out _));

        buffer = "20000101000060000"u8.ToArray();
        Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyyyMMddHHmmssfff", out _), DateTimeKind.Unspecified, out _));

        // Invalid Millisecond
        buffer = "20000101000000***"u8.ToArray();
        Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyyyMMddHHmmssfff", out _), DateTimeKind.Unspecified, out _));

        buffer = "20000101000000**"u8.ToArray();
        Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyyyMMddHHmmssff", out _), DateTimeKind.Unspecified, out _));

        buffer = "20000101000000*"u8.ToArray();
        Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyyyMMddHHmmssf", out _), DateTimeKind.Unspecified, out _));
    }

    [Fact]
    public void FormatDateTime()
    {
        // Default
        var format = "yyyyMMddHHmmssfff";
        var buffer = new byte[format.Length];
        var entries = DateTimeByteHelper.ParseDateTimeFormat(format, out var hasDatePart);
        DateTimeByteHelper.FormatDateTime(buffer, 0, hasDatePart, entries, new DateTime(2199, 12, 31, 23, 59, 59, 789));
        Assert.Equal("21991231235959789", Encoding.ASCII.GetString(buffer, 0, buffer.Length));

        // Short
        format = "yMdHmsf";
        buffer = new byte[format.Length];
        entries = DateTimeByteHelper.ParseDateTimeFormat(format, out hasDatePart);
        DateTimeByteHelper.FormatDateTime(buffer, 0, hasDatePart, entries, new DateTime(2199, 12, 31, 23, 59, 59, 789));
        Assert.Equal("9213997", Encoding.ASCII.GetString(buffer, 0, buffer.Length));

        // 2year
        format = "yy";
        buffer = new byte[format.Length];
        entries = DateTimeByteHelper.ParseDateTimeFormat(format, out hasDatePart);
        DateTimeByteHelper.FormatDateTime(buffer, 0, hasDatePart, entries, new DateTime(2199, 12, 31, 23, 59, 59, 789));
        Assert.Equal("99", Encoding.ASCII.GetString(buffer, 0, buffer.Length));

        // 3year
        format = "yyy";
        buffer = new byte[format.Length];
        entries = DateTimeByteHelper.ParseDateTimeFormat(format, out hasDatePart);
        DateTimeByteHelper.FormatDateTime(buffer, 0, hasDatePart, entries, new DateTime(2199, 12, 31, 23, 59, 59, 789));
        Assert.Equal("199", Encoding.ASCII.GetString(buffer, 0, buffer.Length));

        // 2ms
        format = "ff";
        buffer = new byte[format.Length];
        entries = DateTimeByteHelper.ParseDateTimeFormat(format, out hasDatePart);
        DateTimeByteHelper.FormatDateTime(buffer, 0, hasDatePart, entries, new DateTime(2199, 12, 31, 23, 59, 59, 789));
        Assert.Equal("78", Encoding.ASCII.GetString(buffer, 0, buffer.Length));

        // Default
        format = "yyyyyMMMdddHHHmmmsss";
        buffer = new byte[format.Length];
        entries = DateTimeByteHelper.ParseDateTimeFormat(format, out hasDatePart);
        DateTimeByteHelper.FormatDateTime(buffer, 0, hasDatePart, entries, new DateTime(2199, 12, 31, 23, 59, 59, 789));
        Assert.Equal("02199012031023059059", Encoding.ASCII.GetString(buffer, 0, buffer.Length));

        // Format
        format = "[yyyy/MM/dd - HH:mm:ss.fff]";
        buffer = new byte[format.Length];
        entries = DateTimeByteHelper.ParseDateTimeFormat(format, out hasDatePart);
        DateTimeByteHelper.FormatDateTime(buffer, 0, hasDatePart, entries, new DateTime(2199, 12, 31, 23, 59, 59, 789));
        Assert.Equal("[2199/12/31 - 23:59:59.789]", Encoding.ASCII.GetString(buffer, 0, buffer.Length));

        // Leap
        format = "yyyyMMdd";
        buffer = new byte[format.Length];
        entries = DateTimeByteHelper.ParseDateTimeFormat(format, out hasDatePart);
        DateTimeByteHelper.FormatDateTime(buffer, 0, hasDatePart, entries, new DateTime(2000, 2, 29));
        Assert.Equal("20000229", Encoding.ASCII.GetString(buffer, 0, buffer.Length));

        format = "yyyyMMdd";
        buffer = new byte[format.Length];
        entries = DateTimeByteHelper.ParseDateTimeFormat(format, out hasDatePart);
        DateTimeByteHelper.FormatDateTime(buffer, 0, hasDatePart, entries, new DateTime(2004, 2, 29));
        Assert.Equal("20040229", Encoding.ASCII.GetString(buffer, 0, buffer.Length));

        // DatePart
        format = "yyyyMMdd";
        buffer = new byte[format.Length];
        entries = DateTimeByteHelper.ParseDateTimeFormat(format, out hasDatePart);
        DateTimeByteHelper.FormatDateTime(buffer, 0, hasDatePart, entries, new DateTime(2000, 2, 1));
        Assert.Equal("20000201", Encoding.ASCII.GetString(buffer, 0, buffer.Length));

        // Failed

        // Invalid format
        format = "yyyyMMddHHmmssffff";
        Assert.Throws<FormatException>(() => DateTimeByteHelper.ParseDateTimeFormat(format, out _));
    }
}
