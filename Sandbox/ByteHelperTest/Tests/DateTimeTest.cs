namespace ByteHelperTest.Tests
{
    using System;
    using System.Text;

    using Xunit;

    public class DateTimeTest
    {
        [Fact]
        public void TryParaseDateTime()
        {
            var buffer = Encoding.ASCII.GetBytes("21991231235959999");
            var ret = ByteHelper.TryParseDateTime(buffer, 0, "yyyyMMddHHmmssfff", out var value);
            Assert.True(ret);
            Assert.Equal(new DateTime(2199, 12, 31, 23, 59, 59, 999), value);

            buffer = Encoding.ASCII.GetBytes("219912312359591");
            ret = ByteHelper.TryParseDateTime(buffer, 0, "yyyyMMddHHmmssf", out value);
            Assert.True(ret);
            Assert.Equal(new DateTime(2199, 12, 31, 23, 59, 59, 100), value);

            buffer = Encoding.ASCII.GetBytes("   1 1 1 0 0 0000");
            ret = ByteHelper.TryParseDateTime(buffer, 0, "yyyyMMddHHmmssfff", out value);
            Assert.True(ret);
            Assert.Equal(new DateTime(1, 1, 1, 0, 0, 0, 0), value);

            buffer = Encoding.ASCII.GetBytes(" 2199/12/31 23:59:59.123 ");
            ret = ByteHelper.TryParseDateTime(buffer, 1, "yyyy/MM/dd HH:mm:ss.fff", out value);
            Assert.True(ret);
            Assert.Equal(new DateTime(2199, 12, 31, 23, 59, 59, 123), value);
        }

        [Fact]
        public void FormatDateTime()
        {
            var buffer = new byte[32];

            var format = "yyyyMMddHHmmssfff";
            ByteHelper.FormatDateTime(buffer, 0, format, new DateTime(2199, 12, 31, 23, 59, 59, 999));
            var str = Encoding.ASCII.GetString(buffer, 0, format.Length);
            Assert.Equal("21991231235959999", str);

            format = "yyyyMMddHHmmssf";
            ByteHelper.FormatDateTime(buffer, 0, format, new DateTime(2199, 12, 31, 23, 59, 59, 100));
            str = Encoding.ASCII.GetString(buffer, 0, format.Length);
            Assert.Equal("219912312359591", str);

            format = "yyyyMMddHHmmssfff";
            ByteHelper.FormatDateTime(buffer, 0, format, new DateTime(1, 1, 1, 0, 0, 0, 0));
            str = Encoding.ASCII.GetString(buffer, 0, format.Length);
            Assert.Equal("00010101000000000", str);

            format = "yyyy/MM/dd HH:mm:ss.fff";
            ByteHelper.FormatDateTime(buffer, 1, format, new DateTime(2199, 12, 31, 23, 59, 59, 123));
            str = Encoding.ASCII.GetString(buffer, 1, format.Length);
            Assert.Equal("2199/12/31 23:59:59.123", str);
        }
    }
}
