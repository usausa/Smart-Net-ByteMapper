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
            Assert.True(DateTimeByteHelper.TryParseDateTime(buffer, 0, "yyyyMMddHHmmssfff", DateTimeKind.Unspecified, out var value));
            Assert.Equal(new DateTime(2199, 12, 31, 23, 59, 59, 999), value);

            // Date only
            buffer = Encoding.ASCII.GetBytes("21991231");
            Assert.True(DateTimeByteHelper.TryParseDateTime(buffer, 0, "yyyyMMdd", DateTimeKind.Unspecified, out value));
            Assert.Equal(new DateTime(2199, 12, 31), value);

            // Short year
            buffer = Encoding.ASCII.GetBytes("991231");
            Assert.True(DateTimeByteHelper.TryParseDateTime(buffer, 0, "yyMMdd", DateTimeKind.Unspecified, out value));
            Assert.Equal(new DateTime(2099, 12, 31), value);

            // Current year
            buffer = Encoding.ASCII.GetBytes("1231");
            Assert.True(DateTimeByteHelper.TryParseDateTime(buffer, 0, "MMdd", DateTimeKind.Unspecified, out value));
            Assert.Equal(new DateTime(DateTime.Now.Year, 12, 31), value);

            // Default date
            buffer = Encoding.ASCII.GetBytes("2199");
            Assert.True(DateTimeByteHelper.TryParseDateTime(buffer, 0, "yyyy", DateTimeKind.Unspecified, out value));
            Assert.Equal(new DateTime(2199, 1, 1), value);

            // Short ms
            buffer = Encoding.ASCII.GetBytes("219912312359591");
            Assert.True(DateTimeByteHelper.TryParseDateTime(buffer, 0, "yyyyMMddHHmmssf", DateTimeKind.Unspecified, out value));
            Assert.Equal(new DateTime(2199, 12, 31, 23, 59, 59, 100), value);

            // Long ms
            buffer = Encoding.ASCII.GetBytes("219912312359591  ");
            Assert.True(DateTimeByteHelper.TryParseDateTime(buffer, 0, "yyyyMMddHHmmssfff", DateTimeKind.Unspecified, out value));
            Assert.Equal(new DateTime(2199, 12, 31, 23, 59, 59, 100), value);

            // Space
            buffer = Encoding.ASCII.GetBytes("   1 1 1 0 0 0000");
            Assert.True(DateTimeByteHelper.TryParseDateTime(buffer, 0, "yyyyMMddHHmmssfff", DateTimeKind.Unspecified, out value));
            Assert.Equal(new DateTime(1, 1, 1, 0, 0, 0, 0), value);

            // Format & trim
            buffer = Encoding.ASCII.GetBytes(" 2199/12/31 23:59:59.123 ");
            Assert.True(DateTimeByteHelper.TryParseDateTime(buffer, 1, "yyyy/MM/dd HH:mm:ss.fff", DateTimeKind.Unspecified, out value));
            Assert.Equal(new DateTime(2199, 12, 31, 23, 59, 59, 123), value);

            // Failed

            // Empty
            buffer = Encoding.ASCII.GetBytes("        ");
            Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, "yyyyMMddHH", DateTimeKind.Unspecified, out value));

            // Invalid format
            buffer = Encoding.ASCII.GetBytes("21991231");
            Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, "xxxxxxxx", DateTimeKind.Unspecified, out value));

            // Invalid Year
            buffer = Encoding.ASCII.GetBytes("****0101000000000");
            Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, "yyyyMMddHHmmssfff", DateTimeKind.Unspecified, out value));

            buffer = Encoding.ASCII.GetBytes("999990101000000000");
            Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, "yyyyyMMddHHmmssfff", DateTimeKind.Unspecified, out value));

            // Invalid Month
            buffer = Encoding.ASCII.GetBytes("2000**01000000000");
            Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, "yyyyMMddHHmmssfff", DateTimeKind.Unspecified, out value));

            buffer = Encoding.ASCII.GetBytes("20001301000000000");
            Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, "yyyyMMddHHmmssfff", DateTimeKind.Unspecified, out value));

            // Invalid Day
            buffer = Encoding.ASCII.GetBytes("200001**000000000");
            Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, "yyyyMMddHHmmssfff", DateTimeKind.Unspecified, out value));

            buffer = Encoding.ASCII.GetBytes("20000132000000000");
            Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, "yyyyMMddHHmmssfff", DateTimeKind.Unspecified, out value));

            // Invalid Hour
            buffer = Encoding.ASCII.GetBytes("20000101**0000000");
            Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, "yyyyMMddHHmmssfff", DateTimeKind.Unspecified, out value));

            buffer = Encoding.ASCII.GetBytes("20000101250000000");
            Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, "yyyyMMddHHmmssfff", DateTimeKind.Unspecified, out value));

            // Invalid Minute
            buffer = Encoding.ASCII.GetBytes("2000010100**00000");
            Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, "yyyyMMddHHmmssfff", DateTimeKind.Unspecified, out value));

            buffer = Encoding.ASCII.GetBytes("20000101006000000");
            Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, "yyyyMMddHHmmssfff", DateTimeKind.Unspecified, out value));

            // Invalid Second
            buffer = Encoding.ASCII.GetBytes("200001010000**000");
            Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, "yyyyMMddHHmmssfff", DateTimeKind.Unspecified, out value));

            buffer = Encoding.ASCII.GetBytes("20000101000060000");
            Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, "yyyyMMddHHmmssfff", DateTimeKind.Unspecified, out value));

            // Invalid Milisecond
            buffer = Encoding.ASCII.GetBytes("20000101000000***");
            Assert.False(DateTimeByteHelper.TryParseDateTime(buffer, 0, "yyyyMMddHHmmssfff", DateTimeKind.Unspecified, out value));
        }

        [Fact]
        public void FormatDateTime()
        {
            // Default
            var format = "yyyyMMddHHmmssfff";
            var buffer = new byte[format.Length];
            DateTimeByteHelper.FormatDateTime(buffer, 0, format, new DateTime(2199, 12, 31, 23, 59, 59, 999));
            Assert.Equal("21991231235959999", Encoding.ASCII.GetString(buffer, 0, buffer.Length));

            // Short ms
            format = "yyyyMMddHHmmssf";
            buffer = new byte[format.Length];
            DateTimeByteHelper.FormatDateTime(buffer, 0, format, new DateTime(2199, 12, 31, 23, 59, 59, 999));
            Assert.Equal("219912312359599", Encoding.ASCII.GetString(buffer, 0, buffer.Length));

            // Format
            format = "yyyy/MM/dd HH:mm:ss.fff";
            buffer = new byte[format.Length];
            DateTimeByteHelper.FormatDateTime(buffer, 0, format, new DateTime(2199, 12, 31, 23, 59, 59, 123));
            Assert.Equal("2199/12/31 23:59:59.123", Encoding.ASCII.GetString(buffer, 0, buffer.Length));

            // Failed

            // Invalid format
            format = "yyyyMMddHHmmssffff";
            buffer = new byte[format.Length];
            Assert.Throws<FormatException>(() =>
                DateTimeByteHelper.FormatDateTime(buffer, 0, format, new DateTime(2199, 12, 31, 23, 59, 59, 999)));
        }
    }
}