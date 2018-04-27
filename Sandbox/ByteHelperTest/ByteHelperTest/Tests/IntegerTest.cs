namespace ByteHelperTest.Tests
{
    using System;
    using System.Text;

    using Xunit;

    public class IntegerTest
    {
        [Fact]
        public void TryParseInt32()
        {
            var buffer = Encoding.ASCII.GetBytes("12345678");
            var ret = ByteHelper.TryParseInt32(buffer, 0, buffer.Length, out var value);
            Assert.True(ret);
            Assert.Equal(12345678, value);

            buffer = Encoding.ASCII.GetBytes("-12345678");
            ret = ByteHelper.TryParseInt32(buffer, 0, buffer.Length, out value);
            Assert.True(ret);
            Assert.Equal(-12345678, value);

            buffer = Encoding.ASCII.GetBytes("0");
            ret = ByteHelper.TryParseInt32(buffer, 0, buffer.Length, out value);
            Assert.True(ret);
            Assert.Equal(0, value);

            buffer = Encoding.ASCII.GetBytes("-0");
            ret = ByteHelper.TryParseInt32(buffer, 0, buffer.Length, out value);
            Assert.True(ret);
            Assert.Equal(0, value);

            buffer = Encoding.ASCII.GetBytes(" 12345678 ");
            ret = ByteHelper.TryParseInt32(buffer, 0, buffer.Length, out value);
            Assert.True(ret);
            Assert.Equal(12345678, value);

            buffer = Encoding.ASCII.GetBytes("12 34");
            ret = ByteHelper.TryParseInt32(buffer, 0, buffer.Length, out value);
            Assert.False(ret);

            buffer = Encoding.ASCII.GetBytes("a1234");
            ret = ByteHelper.TryParseInt32(buffer, 0, buffer.Length, out value);
            Assert.False(ret);

            buffer = Encoding.ASCII.GetBytes("1234a");
            ret = ByteHelper.TryParseInt32(buffer, 0, buffer.Length, out value);
            Assert.False(ret);
        }

        [Fact]
        public void FormatInt32()
        {
            var buffer = new byte[32];

            // 0
            buffer.Fill(0, buffer.Length, 0);
            ByteHelper.FormatInt32(buffer, 0, 10, 0, Padding.Left, false);
            Assert.Equal("         0", Encoding.ASCII.GetString(buffer, 0, 10));

            buffer.Fill(0, buffer.Length, 0);
            ByteHelper.FormatInt32(buffer, 0, 10, 0, Padding.Left, true);
            Assert.Equal("0000000000", Encoding.ASCII.GetString(buffer, 0, 10));

            buffer.Fill(0, buffer.Length, 0);
            ByteHelper.FormatInt32(buffer, 0, 10, 0, Padding.Right, false);
            Assert.Equal("0         ", Encoding.ASCII.GetString(buffer, 0, 10));

            // 10
            buffer.Fill(0, buffer.Length, 0);
            ByteHelper.FormatInt32(buffer, 0, 10, 10, Padding.Left, false);
            Assert.Equal("        10", Encoding.ASCII.GetString(buffer, 0, 10));

            buffer.Fill(0, buffer.Length, 0);
            ByteHelper.FormatInt32(buffer, 0, 10, 10, Padding.Left, true);
            Assert.Equal("0000000010", Encoding.ASCII.GetString(buffer, 0, 10));

            buffer.Fill(0, buffer.Length, 0);
            ByteHelper.FormatInt32(buffer, 0, 10, 10, Padding.Right, false);
            Assert.Equal("10        ", Encoding.ASCII.GetString(buffer, 0, 10));

            // -10
            buffer.Fill(0, buffer.Length, 0);
            ByteHelper.FormatInt32(buffer, 0, 10, -10, Padding.Left, false);
            Assert.Equal("       -10", Encoding.ASCII.GetString(buffer, 0, 10));

            buffer.Fill(0, buffer.Length, 0);
            ByteHelper.FormatInt32(buffer, 0, 10, -10, Padding.Left, true);
            Assert.Equal("-000000010", Encoding.ASCII.GetString(buffer, 0, 10));

            buffer.Fill(0, buffer.Length, 0);
            ByteHelper.FormatInt32(buffer, 0, 10, -10, Padding.Right, false);
            Assert.Equal("-10       ", Encoding.ASCII.GetString(buffer, 0, 10));

            // Int32.MaxValue
            buffer.Fill(0, buffer.Length, 0);
            ByteHelper.FormatInt32(buffer, 0, 12, Int32.MaxValue, Padding.Left, false);
            Assert.Equal("  2147483647", Encoding.ASCII.GetString(buffer, 0, 12));

            buffer.Fill(0, buffer.Length, 0);
            ByteHelper.FormatInt32(buffer, 0, 12, Int32.MaxValue, Padding.Left, true);
            Assert.Equal("002147483647", Encoding.ASCII.GetString(buffer, 0, 12));

            buffer.Fill(0, buffer.Length, 0);
            ByteHelper.FormatInt32(buffer, 0, 12, Int32.MaxValue, Padding.Right, false);
            Assert.Equal("2147483647  ", Encoding.ASCII.GetString(buffer, 0, 12));

            // Int32.MinValue
            buffer.Fill(0, buffer.Length, 0);
            ByteHelper.FormatInt32(buffer, 0, 12, Int32.MinValue, Padding.Left, false);
            Assert.Equal(" -2147483648", Encoding.ASCII.GetString(buffer, 0, 12));

            buffer.Fill(0, buffer.Length, 0);
            ByteHelper.FormatInt32(buffer, 0, 12, Int32.MinValue, Padding.Left, true);
            Assert.Equal("-02147483648", Encoding.ASCII.GetString(buffer, 0, 12));

            buffer.Fill(0, buffer.Length, 0);
            ByteHelper.FormatInt32(buffer, 0, 12, Int32.MinValue, Padding.Right, false);
            Assert.Equal("-2147483648 ", Encoding.ASCII.GetString(buffer, 0, 12));

            // Overflow
            buffer.Fill(0, buffer.Length, 0);
            ByteHelper.FormatInt32(buffer, 0, 4, 10000, Padding.Left, false);
            Assert.Equal("0000", Encoding.ASCII.GetString(buffer, 0, 4));

            buffer.Fill(0, buffer.Length, 0);
            ByteHelper.FormatInt32(buffer, 0, 4, 10000, Padding.Left, true);
            Assert.Equal("0000", Encoding.ASCII.GetString(buffer, 0, 4));

            buffer.Fill(0, buffer.Length, 0);
            ByteHelper.FormatInt32(buffer, 0, 4, 10000, Padding.Right, false);
            Assert.Equal("0000", Encoding.ASCII.GetString(buffer, 0, 4));

            // Overflow
            buffer.Fill(0, buffer.Length, 0);
            ByteHelper.FormatInt32(buffer, 0, 4, -10000, Padding.Left, false);
            Assert.Equal("0000", Encoding.ASCII.GetString(buffer, 0, 4));

            buffer.Fill(0, buffer.Length, 0);
            ByteHelper.FormatInt32(buffer, 0, 4, -10000, Padding.Left, true);
            Assert.Equal("0000", Encoding.ASCII.GetString(buffer, 0, 4));

            buffer.Fill(0, buffer.Length, 0);
            ByteHelper.FormatInt32(buffer, 0, 4, -10000, Padding.Right, false);
            Assert.Equal("0000", Encoding.ASCII.GetString(buffer, 0, 4));
        }
    }
}
