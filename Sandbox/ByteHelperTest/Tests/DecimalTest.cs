namespace ByteHelperTest.Tests
{
    using System.Text;

    using Xunit;

    public class DecimalTest
    {
        [Fact]
        public void TryParseDecimal()
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
    }
}
