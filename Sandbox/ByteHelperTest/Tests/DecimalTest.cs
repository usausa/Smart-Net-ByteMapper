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
        public void FormatDecimal()
        {
            var buffer = new byte[32];

            // TODO
            buffer.Fill(0, buffer.Length, 0);
            ByteHelper.FormatDecimal2(buffer, 0, 10, 12345678m, 0, Padding.Left, false);
            Assert.Equal("  12345678", Encoding.ASCII.GetString(buffer, 0, 10));

            buffer.Fill(0, buffer.Length, 0);
            ByteHelper.FormatDecimal2(buffer, 0, 10, 123456.78m, 2, Padding.Left, false);
            Assert.Equal(" 123456.78", Encoding.ASCII.GetString(buffer, 0, 10));

            buffer.Fill(0, buffer.Length, 0);
            ByteHelper.FormatDecimal2(buffer, 0, 10, 123456.78m, 3, Padding.Left, false);
            Assert.Equal("123456.780", Encoding.ASCII.GetString(buffer, 0, 10));

            buffer.Fill(0, buffer.Length, 0);
            ByteHelper.FormatDecimal2(buffer, 0, 10, 123456.78m, 1, Padding.Left, false);
            Assert.Equal("  123456.7", Encoding.ASCII.GetString(buffer, 0, 10));

            // zero
            buffer.Fill(0, buffer.Length, 0);
            ByteHelper.FormatDecimal2(buffer, 0, 10, 12345678m, 0, Padding.Left, true);
            Assert.Equal("0012345678", Encoding.ASCII.GetString(buffer, 0, 10));

            buffer.Fill(0, buffer.Length, 0);
            ByteHelper.FormatDecimal2(buffer, 0, 10, 123456.78m, 2, Padding.Left, true);
            Assert.Equal("0123456.78", Encoding.ASCII.GetString(buffer, 0, 10));

            buffer.Fill(0, buffer.Length, 0);
            ByteHelper.FormatDecimal2(buffer, 0, 10, 123456.78m, 3, Padding.Left, true);
            Assert.Equal("123456.780", Encoding.ASCII.GetString(buffer, 0, 10));

            buffer.Fill(0, buffer.Length, 0);
            ByteHelper.FormatDecimal2(buffer, 0, 10, 123456.78m, 1, Padding.Left, true);
            Assert.Equal("00123456.7", Encoding.ASCII.GetString(buffer, 0, 10));
        }
    }
}
