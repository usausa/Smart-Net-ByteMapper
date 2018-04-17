namespace ByteHelperTest.Tests
{
    using System.Text;

    using Xunit;

    public class DecimalTest
    {
        [Fact]
        public void TryParseDecimal28()
        {
            var buffer = Encoding.ASCII.GetBytes("12345678901234567890123456.78");
            var ret = ByteHelper.TryParseDecimal(buffer, 0, buffer.Length, out var value);
            Assert.True(ret);
            Assert.Equal(12345678901234567890123456.78m, value);

            buffer = Encoding.ASCII.GetBytes("-12345678901234567890123456.78");
            ret = ByteHelper.TryParseDecimal(buffer, 0, buffer.Length, out value);
            Assert.True(ret);
            Assert.Equal(-12345678901234567890123456.78m, value);

            buffer = Encoding.ASCII.GetBytes("0");
            ret = ByteHelper.TryParseDecimal(buffer, 0, buffer.Length, out value);
            Assert.True(ret);
            Assert.Equal(0m, value);

            buffer = Encoding.ASCII.GetBytes("-0");
            ret = ByteHelper.TryParseDecimal(buffer, 0, buffer.Length, out value);
            Assert.True(ret);
            Assert.Equal(0m, value);

            buffer = Encoding.ASCII.GetBytes(" 12345678901234567890123456.78 ");
            ret = ByteHelper.TryParseDecimal(buffer, 0, buffer.Length, out value);
            Assert.True(ret);
            Assert.Equal(12345678901234567890123456.78m, value);

            buffer = Encoding.ASCII.GetBytes("1234567890 1234567890123456.78");
            ret = ByteHelper.TryParseDecimal(buffer, 0, buffer.Length, out value);
            Assert.False(ret);

            buffer = Encoding.ASCII.GetBytes("a12345678901234567890123456.78");
            ret = ByteHelper.TryParseDecimal(buffer, 0, buffer.Length, out value);
            Assert.False(ret);

            buffer = Encoding.ASCII.GetBytes("12345678901234567890123456.78a");
            ret = ByteHelper.TryParseDecimal(buffer, 0, buffer.Length, out value);
            Assert.False(ret);
        }

        [Fact]
        public void TryParseDecimalB19()
        {
            var buffer = Encoding.ASCII.GetBytes("12345678901234567.89");
            var ret = ByteHelper.TryParseDecimal2(buffer, 0, buffer.Length, out var value);
            Assert.True(ret);
            Assert.Equal(12345678901234567.89m, value);

            buffer = Encoding.ASCII.GetBytes("-12345678901234567.89");
            ret = ByteHelper.TryParseDecimal2(buffer, 0, buffer.Length, out value);
            Assert.True(ret);
            Assert.Equal(-12345678901234567.89m, value);

            buffer = Encoding.ASCII.GetBytes("0");
            ret = ByteHelper.TryParseDecimal2(buffer, 0, buffer.Length, out value);
            Assert.True(ret);
            Assert.Equal(0m, value);

            buffer = Encoding.ASCII.GetBytes("-0");
            ret = ByteHelper.TryParseDecimal2(buffer, 0, buffer.Length, out value);
            Assert.True(ret);
            Assert.Equal(0m, value);

            buffer = Encoding.ASCII.GetBytes(" 12345678901234567.89 ");
            ret = ByteHelper.TryParseDecimal2(buffer, 0, buffer.Length, out value);
            Assert.True(ret);
            Assert.Equal(12345678901234567.89m, value);

            buffer = Encoding.ASCII.GetBytes("1234567890 1234567.89");
            ret = ByteHelper.TryParseDecimal2(buffer, 0, buffer.Length, out value);
            Assert.False(ret);

            buffer = Encoding.ASCII.GetBytes("a12345678901234567.89");
            ret = ByteHelper.TryParseDecimal2(buffer, 0, buffer.Length, out value);
            Assert.False(ret);

            buffer = Encoding.ASCII.GetBytes("12345678901234567.89a");
            ret = ByteHelper.TryParseDecimal2(buffer, 0, buffer.Length, out value);
            Assert.False(ret);
        }

        [Fact]
        public void FormatDecimal2()
        {
            var buffer = new byte[32];

            // TODO

            // Left

            buffer.Fill(0, buffer.Length, 0);
            ByteHelper.FormatDecimal2(buffer, 0, 10, 1234567m, 0, Padding.Left, false);
            Assert.Equal("   1234567", Encoding.ASCII.GetString(buffer, 0, 10));

            buffer.Fill(0, buffer.Length, 0);
            ByteHelper.FormatDecimal2(buffer, 0, 10, 12345.67m, 2, Padding.Left, false);
            Assert.Equal("  12345.67", Encoding.ASCII.GetString(buffer, 0, 10));

            buffer.Fill(0, buffer.Length, 0);
            ByteHelper.FormatDecimal2(buffer, 0, 10, -12345.67m, 2, Padding.Left, false);
            Assert.Equal(" -12345.67", Encoding.ASCII.GetString(buffer, 0, 10));

            buffer.Fill(0, buffer.Length, 0);
            ByteHelper.FormatDecimal2(buffer, 0, 10, 12345.67m, 3, Padding.Left, false);
            Assert.Equal(" 12345.670", Encoding.ASCII.GetString(buffer, 0, 10));

            buffer.Fill(0, buffer.Length, 0);
            ByteHelper.FormatDecimal2(buffer, 0, 10, 12345.67m, 1, Padding.Left, false);
            Assert.Equal("   12345.6", Encoding.ASCII.GetString(buffer, 0, 10));

            // Zero

            buffer.Fill(0, buffer.Length, 0);
            ByteHelper.FormatDecimal2(buffer, 0, 10, 1234567m, 0, Padding.Left, true);
            Assert.Equal("0001234567", Encoding.ASCII.GetString(buffer, 0, 10));

            buffer.Fill(0, buffer.Length, 0);
            ByteHelper.FormatDecimal2(buffer, 0, 10, 12345.67m, 2, Padding.Left, true);
            Assert.Equal("0012345.67", Encoding.ASCII.GetString(buffer, 0, 10));

            buffer.Fill(0, buffer.Length, 0);
            ByteHelper.FormatDecimal2(buffer, 0, 10, -12345.67m, 2, Padding.Left, true);
            Assert.Equal("-012345.67", Encoding.ASCII.GetString(buffer, 0, 10));

            buffer.Fill(0, buffer.Length, 0);
            ByteHelper.FormatDecimal2(buffer, 0, 10, 12345.67m, 3, Padding.Left, true);
            Assert.Equal("012345.670", Encoding.ASCII.GetString(buffer, 0, 10));

            buffer.Fill(0, buffer.Length, 0);
            ByteHelper.FormatDecimal2(buffer, 0, 10, 12345.67m, 1, Padding.Left, true);
            Assert.Equal("00012345.6", Encoding.ASCII.GetString(buffer, 0, 10));

            // Right

            buffer.Fill(0, buffer.Length, 0);
            ByteHelper.FormatDecimal2(buffer, 0, 10, 1234567m, 0, Padding.Right, false);
            Assert.Equal("1234567   ", Encoding.ASCII.GetString(buffer, 0, 10));

            buffer.Fill(0, buffer.Length, 0);
            ByteHelper.FormatDecimal2(buffer, 0, 10, 12345.67m, 2, Padding.Right, false);
            Assert.Equal("12345.67  ", Encoding.ASCII.GetString(buffer, 0, 10));

            buffer.Fill(0, buffer.Length, 0);
            ByteHelper.FormatDecimal2(buffer, 0, 10, -12345.67m, 2, Padding.Right, false);
            Assert.Equal("-12345.67 ", Encoding.ASCII.GetString(buffer, 0, 10));

            buffer.Fill(0, buffer.Length, 0);
            ByteHelper.FormatDecimal2(buffer, 0, 10, 12345.67m, 3, Padding.Right, false);
            Assert.Equal("12345.670 ", Encoding.ASCII.GetString(buffer, 0, 10));

            buffer.Fill(0, buffer.Length, 0);
            ByteHelper.FormatDecimal2(buffer, 0, 10, 12345.67m, 1, Padding.Right, false);
            Assert.Equal("12345.6   ", Encoding.ASCII.GetString(buffer, 0, 10));
        }
    }
}
