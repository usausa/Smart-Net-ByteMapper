namespace Smart.IO.ByteMapper.Helpers
{
    using System;
    using System.Text;

    using Xunit;

    public class DateTimeByteHelperTest
    {
        [Fact]
        public void ParseDateTime()
        {
            // Default
            var buffer = Encoding.ASCII.GetBytes("21991231235959999");
            Assert.True(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyyyMMddHHmmssfff", out _), DateTimeKind.Unspecified, out var value));
            Assert.Equal(new DateTime(2199, 12, 31, 23, 59, 59, 999), value);

            // Date only
            buffer = Encoding.ASCII.GetBytes("21991231");
            Assert.True(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyyyMMdd", out _), DateTimeKind.Unspecified, out value));
            Assert.Equal(new DateTime(2199, 12, 31), value);

            // Short year
            buffer = Encoding.ASCII.GetBytes("991231");
            Assert.True(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyMMdd", out _), DateTimeKind.Unspecified, out value));
            Assert.Equal(new DateTime(2099, 12, 31), value);

            // Current year
            buffer = Encoding.ASCII.GetBytes("1231");
            Assert.True(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("MMdd", out _), DateTimeKind.Unspecified, out value));
            Assert.Equal(new DateTime(DateTime.Now.Year, 12, 31), value);

            // Default date
            buffer = Encoding.ASCII.GetBytes("2199");
            Assert.True(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyyy", out _), DateTimeKind.Unspecified, out value));
            Assert.Equal(new DateTime(2199, 1, 1), value);

            // Short
            buffer = Encoding.ASCII.GetBytes("219912312359591");
            Assert.True(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyyyMMddHHmmssf", out _), DateTimeKind.Unspecified, out value));
            Assert.Equal(new DateTime(2199, 12, 31, 23, 59, 59, 100), value);

            // 2ms
            buffer = Encoding.ASCII.GetBytes("2199123123595912");
            Assert.True(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyyyMMddHHmmssff", out _), DateTimeKind.Unspecified, out value));
            Assert.Equal(new DateTime(2199, 12, 31, 23, 59, 59, 120), value);

            // Space
            buffer = Encoding.ASCII.GetBytes("   1 1 1 0 0 0000");
            Assert.True(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyyyMMddHHmmssfff", out _), DateTimeKind.Unspecified, out value));
            Assert.Equal(new DateTime(1, 1, 1, 0, 0, 0, 0), value);

            // Format & trim
            buffer = Encoding.ASCII.GetBytes(" 2199/12/31 23:59:59.123 ");
            Assert.True(DateTimeByteHelper.TryParseDateTime(buffer, 1, DateTimeByteHelper.ParseDateTimeFormat("yyyy/MM/dd HH:mm:ss.fff", out _), DateTimeKind.Unspecified, out value));
            Assert.Equal(new DateTime(2199, 12, 31, 23, 59, 59, 123), value);

            // Failed

            // Empty
            buffer = Encoding.ASCII.GetBytes("        ");
            Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyyyMMddHH", out _), DateTimeKind.Unspecified, out value));

            // Invalid Year
            buffer = Encoding.ASCII.GetBytes("****0101000000000");
            Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyyyMMddHHmmssfff", out _), DateTimeKind.Unspecified, out value));

            buffer = Encoding.ASCII.GetBytes(" 0*00101000000000");
            Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyyyMMddHHmmssfff", out _), DateTimeKind.Unspecified, out value));

            buffer = Encoding.ASCII.GetBytes("999990101000000000");
            Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyyyyMMddHHmmssfff", out _), DateTimeKind.Unspecified, out value));

            // Invalid Month
            buffer = Encoding.ASCII.GetBytes("2000**01000000000");
            Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyyyMMddHHmmssfff", out _), DateTimeKind.Unspecified, out value));

            buffer = Encoding.ASCII.GetBytes("20001301000000000");
            Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyyyMMddHHmmssfff", out _), DateTimeKind.Unspecified, out value));

            // Invalid Day
            buffer = Encoding.ASCII.GetBytes("200001**000000000");
            Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyyyMMddHHmmssfff", out _), DateTimeKind.Unspecified, out value));

            buffer = Encoding.ASCII.GetBytes("20000132000000000");
            Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyyyMMddHHmmssfff", out _), DateTimeKind.Unspecified, out value));

            // Invalid Hour
            buffer = Encoding.ASCII.GetBytes("20000101**0000000");
            Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyyyMMddHHmmssfff", out _), DateTimeKind.Unspecified, out value));

            buffer = Encoding.ASCII.GetBytes("20000101250000000");
            Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyyyMMddHHmmssfff", out _), DateTimeKind.Unspecified, out value));

            // Invalid Minute
            buffer = Encoding.ASCII.GetBytes("2000010100**00000");
            Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyyyMMddHHmmssfff", out _), DateTimeKind.Unspecified, out value));

            buffer = Encoding.ASCII.GetBytes("20000101006000000");
            Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyyyMMddHHmmssfff", out _), DateTimeKind.Unspecified, out value));

            // Invalid Second
            buffer = Encoding.ASCII.GetBytes("200001010000**000");
            Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyyyMMddHHmmssfff", out _), DateTimeKind.Unspecified, out value));

            buffer = Encoding.ASCII.GetBytes("20000101000060000");
            Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyyyMMddHHmmssfff", out _), DateTimeKind.Unspecified, out value));

            // Invalid Milisecond
            buffer = Encoding.ASCII.GetBytes("20000101000000***");
            Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyyyMMddHHmmssfff", out _), DateTimeKind.Unspecified, out value));

            buffer = Encoding.ASCII.GetBytes("20000101000000**");
            Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyyyMMddHHmmssff", out _), DateTimeKind.Unspecified, out value));

            buffer = Encoding.ASCII.GetBytes("20000101000000*");
            Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, DateTimeByteHelper.ParseDateTimeFormat("yyyyMMddHHmmssf", out _), DateTimeKind.Unspecified, out value));
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
}