namespace ByteHelperTest.Tests
{
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
    }
}
