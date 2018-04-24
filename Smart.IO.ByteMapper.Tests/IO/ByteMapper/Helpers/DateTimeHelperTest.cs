﻿namespace Smart.IO.ByteMapper.Helpers
{
    using System;
    using System.Text;

    using Xunit;

    public class DateTimeHelperTest
    {
        [Fact]
        public void ParseDateTime()
        {
            // Default
            var buffer = Encoding.ASCII.GetBytes("21991231235959999");
            Assert.True(DateTimeHelper.TryParseDateTime(buffer, 0, "yyyyMMddHHmmssfff", DateTimeKind.Unspecified, out var value));
            Assert.Equal(new DateTime(2199, 12, 31, 23, 59, 59, 999), value);

            // Date only
            buffer = Encoding.ASCII.GetBytes("21991231");
            Assert.True(DateTimeHelper.TryParseDateTime(buffer, 0, "yyyyMMdd", DateTimeKind.Unspecified, out value));
            Assert.Equal(new DateTime(2199, 12, 31), value);

            // Short ms
            buffer = Encoding.ASCII.GetBytes("219912312359591");
            Assert.True(DateTimeHelper.TryParseDateTime(buffer, 0, "yyyyMMddHHmmssf", DateTimeKind.Unspecified, out value));
            Assert.Equal(new DateTime(2199, 12, 31, 23, 59, 59, 100), value);

            // Long ms
            buffer = Encoding.ASCII.GetBytes("219912312359591  ");
            Assert.True(DateTimeHelper.TryParseDateTime(buffer, 0, "yyyyMMddHHmmssfff", DateTimeKind.Unspecified, out value));
            Assert.Equal(new DateTime(2199, 12, 31, 23, 59, 59, 100), value);

            // Space
            buffer = Encoding.ASCII.GetBytes("   1 1 1 0 0 0000");
            Assert.True(DateTimeHelper.TryParseDateTime(buffer, 0, "yyyyMMddHHmmssfff", DateTimeKind.Unspecified, out value));
            Assert.Equal(new DateTime(1, 1, 1, 0, 0, 0, 0), value);

            // Format & trim
            buffer = Encoding.ASCII.GetBytes(" 2199/12/31 23:59:59.123 ");
            Assert.True(DateTimeHelper.TryParseDateTime(buffer, 1, "yyyy/MM/dd HH:mm:ss.fff", DateTimeKind.Unspecified, out value));
            Assert.Equal(new DateTime(2199, 12, 31, 23, 59, 59, 123), value);

            // Failed

            // Empty
            buffer = Encoding.ASCII.GetBytes("        ");
            Assert.False(DateTimeHelper.TryParseDateTime(buffer, 0, "yyyyMMddHH", DateTimeKind.Unspecified, out value));

            // Invalid format
            buffer = Encoding.ASCII.GetBytes("21991231");
            Assert.False(DateTimeHelper.TryParseDateTime(buffer, 0, "xxxxxxxx", DateTimeKind.Unspecified, out value));

            // Invalid value
            buffer = Encoding.ASCII.GetBytes("20009999");
            Assert.False(DateTimeHelper.TryParseDateTime(buffer, 0, "yyyyMMdd", DateTimeKind.Unspecified, out value));
        }

        [Fact]
        public void FormatDateTime()
        {
            // Default
            var format = "yyyyMMddHHmmssfff";
            var buffer = new byte[format.Length];
            DateTimeHelper.FormatDateTime(buffer, 0, format, new DateTime(2199, 12, 31, 23, 59, 59, 999));
            Assert.Equal("21991231235959999", Encoding.ASCII.GetString(buffer, 0, buffer.Length));

            // Short ms
            format = "yyyyMMddHHmmssf";
            buffer = new byte[format.Length];
            DateTimeHelper.FormatDateTime(buffer, 0, format, new DateTime(2199, 12, 31, 23, 59, 59, 999));
            Assert.Equal("219912312359599", Encoding.ASCII.GetString(buffer, 0, buffer.Length));

            // Format
            format = "yyyy/MM/dd HH:mm:ss.fff";
            buffer = new byte[format.Length];
            DateTimeHelper.FormatDateTime(buffer, 0, format, new DateTime(2199, 12, 31, 23, 59, 59, 123));
            Assert.Equal("2199/12/31 23:59:59.123", Encoding.ASCII.GetString(buffer, 0, buffer.Length));

            // Failed

            // Invalid format
            format = "yyyyMMddHHmmssffff";
            buffer = new byte[format.Length];
            Assert.Throws<FormatException>(() =>
                DateTimeHelper.FormatDateTime(buffer, 0, format, new DateTime(2199, 12, 31, 23, 59, 59, 999)));
        }
    }
}