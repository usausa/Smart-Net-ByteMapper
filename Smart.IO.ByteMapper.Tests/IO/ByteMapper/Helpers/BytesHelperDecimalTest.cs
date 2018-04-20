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

            // Invalid Value
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
            // Left

            // Default scale grouping
            var expected = Encoding.ASCII.GetBytes("  1,234,567,890,123,456.78");
            var buffer = new byte[expected.Length];
            BytesHelper.FormatDecimalLimited64(buffer, 0, buffer.Length, 1234567890123456.78m, 2, 3, Padding.Left, false);
            Assert.Equal(expected, buffer);

            // Negative scale grouping
            expected = Encoding.ASCII.GetBytes(" -1,234,567,890,123,456.78");
            buffer = new byte[expected.Length];
            BytesHelper.FormatDecimalLimited64(buffer, 0, buffer.Length, -1234567890123456.78m, 2, 3, Padding.Left, false);
            Assert.Equal(expected, buffer);

            // Max grouping
            expected = Encoding.ASCII.GetBytes("  999,999,999,999,999,999");
            buffer = new byte[expected.Length];
            BytesHelper.FormatDecimalLimited64(buffer, 0, buffer.Length, 999999999999999999m, 0, 3, Padding.Left, false);
            Assert.Equal(expected, buffer);

            // Max Negative grouping
            expected = Encoding.ASCII.GetBytes(" -999,999,999,999,999,999");
            buffer = new byte[expected.Length];
            BytesHelper.FormatDecimalLimited64(buffer, 0, buffer.Length, -999999999999999999m, 0, 3, Padding.Left, false);
            Assert.Equal(expected, buffer);

            // Zero
            expected = Encoding.ASCII.GetBytes(" 0");
            buffer = new byte[expected.Length];
            BytesHelper.FormatDecimalLimited64(buffer, 0, buffer.Length, 0m, 0, -1, Padding.Left, false);
            Assert.Equal(expected, buffer);

            // TODO

            // ZeroFill

            // Default scale grouping
            expected = Encoding.ASCII.GetBytes("001,234,567,890,123,456.78");
            buffer = new byte[expected.Length];
            BytesHelper.FormatDecimalLimited64(buffer, 0, buffer.Length, 1234567890123456.78m, 2, 3, Padding.Left, true);
            Assert.Equal(expected, buffer);

            // Negative scale grouping
            expected = Encoding.ASCII.GetBytes("-01,234,567,890,123,456.78");
            buffer = new byte[expected.Length];
            BytesHelper.FormatDecimalLimited64(buffer, 0, buffer.Length, -1234567890123456.78m, 2, 3, Padding.Left, true);
            Assert.Equal(expected, buffer);

            // Max grouping
            expected = Encoding.ASCII.GetBytes("0,999,999,999,999,999,999");
            buffer = new byte[expected.Length];
            BytesHelper.FormatDecimalLimited64(buffer, 0, buffer.Length, 999999999999999999m, 0, 3, Padding.Left, true);
            Assert.Equal(expected, buffer);

            // Max Negative grouping
            expected = Encoding.ASCII.GetBytes("-,999,999,999,999,999,999");
            buffer = new byte[expected.Length];
            BytesHelper.FormatDecimalLimited64(buffer, 0, buffer.Length, -999999999999999999m, 0, 3, Padding.Left, true);
            Assert.Equal(expected, buffer);

            // Zero
            expected = Encoding.ASCII.GetBytes("00");
            buffer = new byte[expected.Length];
            BytesHelper.FormatDecimalLimited64(buffer, 0, buffer.Length, 0m, 0, -1, Padding.Left, true);
            Assert.Equal(expected, buffer);

            // TODO

            // Right

            // Default scale grouping
            expected = Encoding.ASCII.GetBytes("1,234,567,890,123,456.78  ");
            buffer = new byte[expected.Length];
            BytesHelper.FormatDecimalLimited64(buffer, 0, buffer.Length, 1234567890123456.78m, 2, 3, Padding.Right, false);
            Assert.Equal(expected, buffer);

            // Negative scale grouping
            expected = Encoding.ASCII.GetBytes("-1,234,567,890,123,456.78 ");
            buffer = new byte[expected.Length];
            BytesHelper.FormatDecimalLimited64(buffer, 0, buffer.Length, -1234567890123456.78m, 2, 3, Padding.Right, false);
            Assert.Equal(expected, buffer);

            // Max grouping
            expected = Encoding.ASCII.GetBytes("999,999,999,999,999,999  ");
            buffer = new byte[expected.Length];
            BytesHelper.FormatDecimalLimited64(buffer, 0, buffer.Length, 999999999999999999m, 0, 3, Padding.Right, false);
            Assert.Equal(expected, buffer);

            // Max Negative grouping
            expected = Encoding.ASCII.GetBytes("-999,999,999,999,999,999 ");
            buffer = new byte[expected.Length];
            BytesHelper.FormatDecimalLimited64(buffer, 0, buffer.Length, -999999999999999999m, 0, 3, Padding.Right, false);
            Assert.Equal(expected, buffer);

            // Zero
            expected = Encoding.ASCII.GetBytes("0 ");
            buffer = new byte[expected.Length];
            BytesHelper.FormatDecimalLimited64(buffer, 0, buffer.Length, 0m, 0, -1, Padding.Right, false);
            Assert.Equal(expected, buffer);

            // TODO
        }

        [Fact]
        public void FormatDecimalGrouping()
        {
            // TODO 桁を変えつつ！
        }
    }
}