namespace Smart.IO.ByteMapper.Helpers
{
    using System;
    using System.Text;

    using Xunit;

    public class BytesHelperDateTimeTest
    {
        [Fact]
        public void ParseDateTime()
        {
            // Default
            var buffer = Encoding.ASCII.GetBytes("21991231235959999");
            Assert.True(BytesHelper.TryParseDateTime(buffer, 0, "yyyyMMddHHmmssfff", out var value));
            Assert.Equal(new DateTime(2199, 12, 31, 23, 59, 59, 999), value);

            // Date only
            buffer = Encoding.ASCII.GetBytes("21991231");
            Assert.True(BytesHelper.TryParseDateTime(buffer, 0, "yyyyMMdd", out value));
            Assert.Equal(new DateTime(2199, 12, 31), value);

            // Short ms
            buffer = Encoding.ASCII.GetBytes("219912312359591");
            Assert.True(BytesHelper.TryParseDateTime(buffer, 0, "yyyyMMddHHmmssf", out value));
            Assert.Equal(new DateTime(2199, 12, 31, 23, 59, 59, 100), value);

            // Long ms
            buffer = Encoding.ASCII.GetBytes("219912312359591  ");
            Assert.True(BytesHelper.TryParseDateTime(buffer, 0, "yyyyMMddHHmmssfff", out value));
            Assert.Equal(new DateTime(2199, 12, 31, 23, 59, 59, 100), value);

            // Space
            buffer = Encoding.ASCII.GetBytes("   1 1 1 0 0 0000");
            Assert.True(BytesHelper.TryParseDateTime(buffer, 0, "yyyyMMddHHmmssfff", out value));
            Assert.Equal(new DateTime(1, 1, 1, 0, 0, 0, 0), value);

            // Format & trim
            buffer = Encoding.ASCII.GetBytes(" 2199/12/31 23:59:59.123 ");
            Assert.True(BytesHelper.TryParseDateTime(buffer, 1, "yyyy/MM/dd HH:mm:ss.fff", out value));
            Assert.Equal(new DateTime(2199, 12, 31, 23, 59, 59, 123), value);

            // Failed

            // Empty
            buffer = Encoding.ASCII.GetBytes("        ");
            Assert.False(BytesHelper.TryParseDateTime(buffer, 0, "yyyyMMddHH", out value));

            // Invalid format
            buffer = Encoding.ASCII.GetBytes("21991231");
            Assert.False(BytesHelper.TryParseDateTime(buffer, 0, "xxxxxxxx", out value));

            // Invalid value
            buffer = Encoding.ASCII.GetBytes("20009999");
            Assert.False(BytesHelper.TryParseDateTime(buffer, 0, "yyyyMMdd", out value));
        }

        [Fact]
        public void FormatDateTime()
        {
            // Default
            var format = "yyyyMMddHHmmssfff";
            var buffer = new byte[format.Length];
            BytesHelper.FormatDateTime(buffer, 0, format, new DateTime(2199, 12, 31, 23, 59, 59, 999));
            Assert.Equal("21991231235959999", Encoding.ASCII.GetString(buffer, 0, buffer.Length));

            // Short ms
            format = "yyyyMMddHHmmssf";
            buffer = new byte[format.Length];
            BytesHelper.FormatDateTime(buffer, 0, format, new DateTime(2199, 12, 31, 23, 59, 59, 999));
            Assert.Equal("219912312359599", Encoding.ASCII.GetString(buffer, 0, buffer.Length));

            // Format
            format = "yyyy/MM/dd HH:mm:ss.fff";
            buffer = new byte[format.Length];
            BytesHelper.FormatDateTime(buffer, 0, format, new DateTime(2199, 12, 31, 23, 59, 59, 123));
            Assert.Equal("2199/12/31 23:59:59.123", Encoding.ASCII.GetString(buffer, 0, buffer.Length));

            // Failed

            // Invalid format
            format = "yyyyMMddHHmmssffff";
            buffer = new byte[format.Length];
            Assert.Throws<FormatException>(() =>
                BytesHelper.FormatDateTime(buffer, 0, format, new DateTime(2199, 12, 31, 23, 59, 59, 999)));
        }
    }
}