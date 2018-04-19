namespace Smart.IO.ByteMapper.Helpers
{
    using System.Globalization;
    using System.Text;

    using Xunit;

    public class BytesHelperDecimalTest
    {
        // TODO Applicable

        [Fact]
        public void ParseDecimal()
        {
            // Default
            var buffer = Encoding.ASCII.GetBytes("1,234,567,890,123,456.78");
            Assert.True(BytesHelper.TryParseDecimalLimited64(buffer, 0, buffer.Length, out var value));
            Assert.Equal(1234567890123456.78m, value);

            // Negative
            buffer = Encoding.ASCII.GetBytes("-1,234,567,890,123,456.78");
            Assert.True(BytesHelper.TryParseDecimalLimited64(buffer, 0, buffer.Length, out value));
            Assert.Equal(-1234567890123456.78m, value);

            // Max
            buffer = Encoding.ASCII.GetBytes("999,999,999,999,999,999");
            Assert.True(BytesHelper.TryParseDecimalLimited64(buffer, 0, buffer.Length, out value));
            Assert.Equal(999999999999999999m, value);

            // Max Negative
            buffer = Encoding.ASCII.GetBytes("-999,999,999,999,999,999");
            Assert.True(BytesHelper.TryParseDecimalLimited64(buffer, 0, buffer.Length, out value));
            Assert.Equal(-999999999999999999m, value);

            // Zero
            buffer = Encoding.ASCII.GetBytes("0");
            Assert.True(BytesHelper.TryParseDecimalLimited64(buffer, 0, buffer.Length, out value));
            Assert.Equal(0m, value);

            // Zero Negative
            buffer = Encoding.ASCII.GetBytes("-0");
            Assert.True(BytesHelper.TryParseDecimalLimited64(buffer, 0, buffer.Length, out value));
            Assert.Equal(0m, value);

            // 32bit
            buffer = Encoding.ASCII.GetBytes(0xFFFFFFFF.ToString(CultureInfo.InvariantCulture));
            Assert.True(BytesHelper.TryParseDecimalLimited64(buffer, 0, buffer.Length, out value));
            Assert.Equal(0xFFFFFFFF, value);

            // 32bit+1
            buffer = Encoding.ASCII.GetBytes(0x100000000.ToString(CultureInfo.InvariantCulture));
            Assert.True(BytesHelper.TryParseDecimalLimited64(buffer, 0, buffer.Length, out value));
            Assert.Equal(0x100000000, value);

            // 64bit
            buffer = Encoding.ASCII.GetBytes(0xFFFFFFFFFFFFFFFF.ToString(CultureInfo.InvariantCulture));
            Assert.True(BytesHelper.TryParseDecimalLimited64(buffer, 0, buffer.Length, out value));
            Assert.Equal(0xFFFFFFFFFFFFFFFF, value);

            // Trim
            buffer = Encoding.ASCII.GetBytes(" 1234567890123456.78 ");
            Assert.True(BytesHelper.TryParseDecimalLimited64(buffer, 0, buffer.Length, out value));
            Assert.Equal(1234567890123456.78m, value);

            // Failed

            // Empty
            buffer = Encoding.ASCII.GetBytes("                  ");
            Assert.False(BytesHelper.TryParseDecimalLimited64(buffer, 0, buffer.Length, out value));

            // Overflow
            buffer = Encoding.ASCII.GetBytes(((decimal)0xFFFFFFFFFFFFFFFF + 1).ToString(CultureInfo.InvariantCulture));
            Assert.True(BytesHelper.TryParseDecimalLimited64(buffer, 0, buffer.Length, out value));
            Assert.NotEqual((decimal)0xFFFFFFFFFFFFFFFF + 1, value);

            // Invalid Valie
            buffer = Encoding.ASCII.GetBytes("1,234,567,8 90,123,456.78");
            Assert.False(BytesHelper.TryParseDecimalLimited64(buffer, 0, buffer.Length, out value));

            buffer = Encoding.ASCII.GetBytes("a1,234,567,890,123,456.78");
            Assert.False(BytesHelper.TryParseDecimalLimited64(buffer, 0, buffer.Length, out value));

            buffer = Encoding.ASCII.GetBytes("1,234,567,890,123,456.78a");
            Assert.False(BytesHelper.TryParseDecimalLimited64(buffer, 0, buffer.Length, out value));
        }

        [Fact]
        public void FormatDecimal()
        {
            // TODO
        }

        [Fact]
        public void FormatDecimalGrouping()
        {
            // TODO
        }
    }
}