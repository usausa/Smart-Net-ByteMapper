namespace ByteHelperTest.Tests
{
    using System.Globalization;
    using System.Text;

    using Xunit;

    public class DecimalTest
    {
        [Fact]
        public void ParseDecimal()
        {
            // Default
            var buffer = Encoding.ASCII.GetBytes("1,234,567,890,123,456.78");
            Assert.True(ByteHelper2.TryParseDecimal(buffer, 0, buffer.Length, 0x20, out var value));
            Assert.Equal(1234567890123456.78m, value);

            // Negative
            buffer = Encoding.ASCII.GetBytes("-1,234,567,890,123,456.78");
            Assert.True(ByteHelper2.TryParseDecimal(buffer, 0, buffer.Length, 0x20, out value));
            Assert.Equal(-1234567890123456.78m, value);

            // Max
            buffer = Encoding.ASCII.GetBytes("99,999,999,999,999,999");
            Assert.True(ByteHelper2.TryParseDecimal(buffer, 0, buffer.Length, 0x20, out value));
            Assert.Equal(99999999999999999m, value);

            // Max Negative
            buffer = Encoding.ASCII.GetBytes("-99,999,999,999,999,999");
            Assert.True(ByteHelper2.TryParseDecimal(buffer, 0, buffer.Length, 0x20, out value));
            Assert.Equal(-99999999999999999m, value);

            // Zero
            buffer = Encoding.ASCII.GetBytes("0");
            Assert.True(ByteHelper2.TryParseDecimal(buffer, 0, buffer.Length, 0x20, out value));
            Assert.Equal(0m, value);

            // Zero Negative
            buffer = Encoding.ASCII.GetBytes("-0");
            Assert.True(ByteHelper2.TryParseDecimal(buffer, 0, buffer.Length, 0x20, out value));
            Assert.Equal(0m, value);

            // 32bit
            buffer = Encoding.ASCII.GetBytes(0xFFFFFFFF.ToString(CultureInfo.InvariantCulture));
            Assert.True(ByteHelper2.TryParseDecimal(buffer, 0, buffer.Length, 0x20, out value));
            Assert.Equal(0xFFFFFFFF, value);

            // 32bit+1
            buffer = Encoding.ASCII.GetBytes(0x100000000.ToString(CultureInfo.InvariantCulture));
            Assert.True(ByteHelper2.TryParseDecimal(buffer, 0, buffer.Length, 0x20, out value));
            Assert.Equal(0x100000000, value);

            // Trim
            buffer = Encoding.ASCII.GetBytes(" 1234567890123456.78 ");
            Assert.True(ByteHelper2.TryParseDecimal(buffer, 0, buffer.Length, 0x20, out value));
            Assert.Equal(1234567890123456.78m, value);

            // Max
            buffer = Encoding.ASCII.GetBytes("79228162514264337593543950335");
            Assert.True(ByteHelper2.TryParseDecimal(buffer, 0, buffer.Length, 0x20, out value));

            // Round
            buffer = Encoding.ASCII.GetBytes("0.99999999999999999999999999995");
            Assert.True(ByteHelper2.TryParseDecimal(buffer, 0, buffer.Length, 0x20, out value));
            Assert.Equal(1.0000000000000000000000000000m, value);

            buffer = Encoding.ASCII.GetBytes("0.99999999999999999999999999994");
            Assert.True(ByteHelper2.TryParseDecimal(buffer, 0, buffer.Length, 0x20, out value));
            Assert.Equal(0.9999999999999999999999999999m, value);

            // Dot only
            buffer = Encoding.ASCII.GetBytes("1.");
            Assert.True(ByteHelper2.TryParseDecimal(buffer, 0, buffer.Length, 0x20, out value));
            Assert.Equal(1m, value);

            // Failed

            // Empty
            buffer = Encoding.ASCII.GetBytes("                  ");
            Assert.False(ByteHelper2.TryParseDecimal(buffer, 0, buffer.Length, 0x20, out value));

            // Multiple dot
            buffer = Encoding.ASCII.GetBytes("1..");
            Assert.False(ByteHelper2.TryParseDecimal(buffer, 0, buffer.Length, 0x20, out value));

            // Overflow
            buffer = Encoding.ASCII.GetBytes("79228162514264337593543950336");
            Assert.False(ByteHelper2.TryParseDecimal(buffer, 0, buffer.Length, 0x20, out value));

            // Invalid Value
            buffer = Encoding.ASCII.GetBytes("1,234.567,89");
            Assert.False(ByteHelper2.TryParseDecimal(buffer, 0, buffer.Length, 0x20, out value));

            buffer = Encoding.ASCII.GetBytes("1,234,567,8 90,123,456.78");
            Assert.False(ByteHelper2.TryParseDecimal(buffer, 0, buffer.Length, 0x20, out value));

            buffer = Encoding.ASCII.GetBytes("a1,234,567,890,123,456.78");
            Assert.False(ByteHelper2.TryParseDecimal(buffer, 0, buffer.Length, 0x20, out value));

            buffer = Encoding.ASCII.GetBytes("1,234,567,890,123,456.78a");
            Assert.False(ByteHelper2.TryParseDecimal(buffer, 0, buffer.Length, 0x20, out value));
        }
    }
}
