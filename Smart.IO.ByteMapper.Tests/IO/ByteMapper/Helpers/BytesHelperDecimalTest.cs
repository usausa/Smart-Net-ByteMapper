namespace Smart.IO.ByteMapper.Helpers
{
    using System;
    //using System.Collections.Generic;
    using System.Globalization;
    //using System.Linq;
    using System.Text;

    using Xunit;

    public class BytesHelperDecimalTest
    {
        [Fact]
        public void ParseDecimal()
        {
            // Default
            var buffer = Encoding.ASCII.GetBytes("1,234,567,890,123,456.78");
            Assert.True(BytesHelper.TryParseDecimalLimited64(buffer, 0, buffer.Length, 0x20, out var value));
            Assert.Equal(1234567890123456.78m, value);

            // Negative
            buffer = Encoding.ASCII.GetBytes("-1,234,567,890,123,456.78");
            Assert.True(BytesHelper.TryParseDecimalLimited64(buffer, 0, buffer.Length, 0x20, out value));
            Assert.Equal(-1234567890123456.78m, value);

            // Max
            buffer = Encoding.ASCII.GetBytes("999,999,999,999,999,999");
            Assert.True(BytesHelper.TryParseDecimalLimited64(buffer, 0, buffer.Length, 0x20, out value));
            Assert.Equal(999999999999999999m, value);

            // Max Negative
            buffer = Encoding.ASCII.GetBytes("-999,999,999,999,999,999");
            Assert.True(BytesHelper.TryParseDecimalLimited64(buffer, 0, buffer.Length, 0x20, out value));
            Assert.Equal(-999999999999999999m, value);

            // Zero
            buffer = Encoding.ASCII.GetBytes("0");
            Assert.True(BytesHelper.TryParseDecimalLimited64(buffer, 0, buffer.Length, 0x20, out value));
            Assert.Equal(0m, value);

            // Zero Negative
            buffer = Encoding.ASCII.GetBytes("-0");
            Assert.True(BytesHelper.TryParseDecimalLimited64(buffer, 0, buffer.Length, 0x20, out value));
            Assert.Equal(0m, value);

            // 32bit
            buffer = Encoding.ASCII.GetBytes(0xFFFFFFFF.ToString(CultureInfo.InvariantCulture));
            Assert.True(BytesHelper.TryParseDecimalLimited64(buffer, 0, buffer.Length, 0x20, out value));
            Assert.Equal(0xFFFFFFFF, value);

            // 32bit+1
            buffer = Encoding.ASCII.GetBytes(0x100000000.ToString(CultureInfo.InvariantCulture));
            Assert.True(BytesHelper.TryParseDecimalLimited64(buffer, 0, buffer.Length, 0x20, out value));
            Assert.Equal(0x100000000, value);

            // 64bit
            buffer = Encoding.ASCII.GetBytes(0xFFFFFFFFFFFFFFFF.ToString(CultureInfo.InvariantCulture));
            Assert.True(BytesHelper.TryParseDecimalLimited64(buffer, 0, buffer.Length, 0x20, out value));
            Assert.Equal(0xFFFFFFFFFFFFFFFF, value);

            // Trim
            buffer = Encoding.ASCII.GetBytes(" 1234567890123456.78 ");
            Assert.True(BytesHelper.TryParseDecimalLimited64(buffer, 0, buffer.Length, 0x20, out value));
            Assert.Equal(1234567890123456.78m, value);

            // Failed

            // Empty
            buffer = Encoding.ASCII.GetBytes("                  ");
            Assert.False(BytesHelper.TryParseDecimalLimited64(buffer, 0, buffer.Length, 0x20, out value));

            // Overflow
            buffer = Encoding.ASCII.GetBytes(((decimal)0xFFFFFFFFFFFFFFFF + 1).ToString(CultureInfo.InvariantCulture));
            Assert.True(BytesHelper.TryParseDecimalLimited64(buffer, 0, buffer.Length, 0x20, out value));
            Assert.NotEqual((decimal)0xFFFFFFFFFFFFFFFF + 1, value);

            // Invalid Value
            buffer = Encoding.ASCII.GetBytes("1,234,567,8 90,123,456.78");
            Assert.False(BytesHelper.TryParseDecimalLimited64(buffer, 0, buffer.Length, 0x20, out value));

            buffer = Encoding.ASCII.GetBytes("a1,234,567,890,123,456.78");
            Assert.False(BytesHelper.TryParseDecimalLimited64(buffer, 0, buffer.Length, 0x20, out value));

            buffer = Encoding.ASCII.GetBytes("1,234,567,890,123,456.78a");
            Assert.False(BytesHelper.TryParseDecimalLimited64(buffer, 0, buffer.Length, 0x20, out value));
        }

        [Theory]
        // Default grouping scale
        [InlineData("1234567890123456.78", "  1,234,567,890,123,456.78", 2, 3, Padding.Left, false)]
        [InlineData("1234567890123456.78", "001,234,567,890,123,456.78", 2, 3, Padding.Left, true)]
        [InlineData("1234567890123456.78", "1,234,567,890,123,456.78  ", 2, 3, Padding.Right, false)]
        // Negative grouping scale
        [InlineData("-1234567890123456.78", " -1,234,567,890,123,456.78", 2, 3, Padding.Left, false)]
        [InlineData("-1234567890123456.78", "-01,234,567,890,123,456.78", 2, 3, Padding.Left, true)]
        [InlineData("-1234567890123456.78", "-1,234,567,890,123,456.78 ", 2, 3, Padding.Right, false)]
        // Zero
        [InlineData("0", " 0", 0, -1, Padding.Left, false)]
        [InlineData("0", "00", 0, -1, Padding.Left, true)]
        [InlineData("0", "0 ", 0, -1, Padding.Right, false)]
        // Max grouping
        [InlineData("999999999999999999", "  999,999,999,999,999,999", 0, 3, Padding.Left, false)]
        [InlineData("999999999999999999", "0,999,999,999,999,999,999", 0, 3, Padding.Left, true)]
        [InlineData("999999999999999999", "999,999,999,999,999,999  ", 0, 3, Padding.Right, false)]
        // Max Negative grouping
        [InlineData("-999999999999999999", " -999,999,999,999,999,999", 0, 3, Padding.Left, false)]
        [InlineData("-999999999999999999", "-,999,999,999,999,999,999", 0, 3, Padding.Left, true)]
        [InlineData("-999999999999999999", "-999,999,999,999,999,999 ", 0, 3, Padding.Right, false)]
        // Max scale
        [InlineData("0.999999999999999999", "  0.999999999999999999", 18, -1, Padding.Left, false)]
        [InlineData("0.999999999999999999", "000.999999999999999999", 18, -1, Padding.Left, true)]
        [InlineData("0.999999999999999999", "0.999999999999999999  ", 18, -1, Padding.Right, false)]
        // 32bit
        [InlineData("4294967295", "  4294967295", 0, -1, Padding.Left, false)]
        [InlineData("4294967295", "004294967295", 0, -1, Padding.Left, true)]
        [InlineData("4294967295", "4294967295  ", 0, -1, Padding.Right, false)]
        // 32bit + 1
        [InlineData("4294967296", "  4294967296", 0, -1, Padding.Left, false)]
        [InlineData("4294967296", "004294967296", 0, -1, Padding.Left, true)]
        [InlineData("4294967296", "4294967296  ", 0, -1, Padding.Right, false)]
        // 32bit scale
        [InlineData("0.4294967295", " 0.4294967295", 10, -1, Padding.Left, false)]
        [InlineData("0.4294967295", "00.4294967295", 10, -1, Padding.Left, true)]
        [InlineData("0.4294967295", "0.4294967295 ", 10, -1, Padding.Right, false)]
        // Buffer short
        [InlineData("0.4294967295", ".4294967295", 10, -1, Padding.Left, false)]
        [InlineData("0.4294967295", ".4294967295", 10, -1, Padding.Right, false)]
        [InlineData("0.4294967295", "4294967295", 10, -1, Padding.Left, false)]
        [InlineData("0.4294967295", "4294967295", 10, -1, Padding.Right, false)]
        // 32bit + 1 scale
        [InlineData("0.4294967296", " 0.4294967296", 10, -1, Padding.Left, false)]
        [InlineData("0.4294967296", "00.4294967296", 10, -1, Padding.Left, true)]
        [InlineData("0.4294967296", "0.4294967296 ", 10, -1, Padding.Right, false)]
        // Buffer short
        [InlineData("0.4294967296", ".4294967296", 10, -1, Padding.Left, false)]
        [InlineData("0.4294967296", ".4294967296", 10, -1, Padding.Right, false)]
        [InlineData("0.4294967296", "4294967296", 10, -1, Padding.Left, false)]
        [InlineData("0.4294967296", "4294967296", 10, -1, Padding.Right, false)]
        // 64bit
        [InlineData("18446744073709551615", "  18446744073709551615", 0, -1, Padding.Left, false)]
        [InlineData("18446744073709551615", "0018446744073709551615", 0, -1, Padding.Left, true)]
        [InlineData("18446744073709551615", "18446744073709551615  ", 0, -1, Padding.Right, false)]
        // 64bit scale
        [InlineData("0.18446744073709551615", " 0.18446744073709551615", 20, -1, Padding.Left, false)]
        [InlineData("0.18446744073709551615", "00.18446744073709551615", 20, -1, Padding.Left, true)]
        [InlineData("0.18446744073709551615", "0.18446744073709551615 ", 20, -1, Padding.Right, false)]
        // Buffer short
        [InlineData("0.18446744073709551615", ".18446744073709551615", 20, -1, Padding.Left, false)]
        [InlineData("0.18446744073709551615", ".18446744073709551615", 20, -1, Padding.Right, false)]
        [InlineData("0.18446744073709551615", "18446744073709551615", 20, -1, Padding.Left, false)]
        [InlineData("0.18446744073709551615", "18446744073709551615", 20, -1, Padding.Right, false)]
        // Buffer short
        [InlineData("1.23", "1.23", 2, -1, Padding.Left, false)]
        [InlineData("1.23", ".23", 2, -1, Padding.Left, false)]
        [InlineData("1.23", "23", 2, -1, Padding.Left, false)]
        [InlineData("1.23", "1.23", 2, -1, Padding.Right, false)]
        [InlineData("1.23", ".23", 2, -1, Padding.Right, false)]
        [InlineData("1.23", "23", 2, -1, Padding.Right, false)]
        [InlineData("-1.23", "-1.23", 2, -1, Padding.Left, false)]
        [InlineData("-1.23", "1.23", 2, -1, Padding.Left, false)]
        [InlineData("-1.23", ".23", 2, -1, Padding.Left, false)]
        [InlineData("-1.23", "-1.23", 2, -1, Padding.Right, false)]
        [InlineData("-1.23", "1.23", 2, -1, Padding.Right, false)]
        [InlineData("-1.23", ".23", 2, -1, Padding.Right, false)]
        // Scale shortage
        [InlineData("0.123", " 0.12300", 5, -1, Padding.Left, false)]
        [InlineData("0.123", "00.12300", 5, -1, Padding.Left, true)]
        [InlineData("0.123", "0.12300 ", 5, -1, Padding.Right, false)]
        // Grouping shortage
        [InlineData("1234", "1,234", 0, 3, Padding.Left, false)]
        [InlineData("1234", ",234", 0, 3, Padding.Left, false)]
        [InlineData("1234", "234", 0, 3, Padding.Left, false)]
        [InlineData("1234", "34", 0, 3, Padding.Left, false)]
        [InlineData("1234", "4", 0, 3, Padding.Left, false)]
        [InlineData("1234", "1,234", 0, 3, Padding.Right, false)]
        [InlineData("1234", ",234", 0, 3, Padding.Right, false)]
        [InlineData("1234", "234", 0, 3, Padding.Right, false)]
        [InlineData("1234", "34", 0, 3, Padding.Right, false)]
        [InlineData("1234", "4", 0, 3, Padding.Right, false)]
        [InlineData("-1234", "-1,234", 0, 3, Padding.Left, false)]
        [InlineData("-1234", "1,234", 0, 3, Padding.Left, false)]
        [InlineData("-1234", ",234", 0, 3, Padding.Left, false)]
        [InlineData("-1234", "-1,234", 0, 3, Padding.Right, false)]
        [InlineData("-1234", "1,234", 0, 3, Padding.Right, false)]
        [InlineData("-1234", ",234", 0, 3, Padding.Right, false)]
        [InlineData("1", "0,001", 0, 3, Padding.Left, true)]
        [InlineData("1", ",001", 0, 3, Padding.Left, true)]
        [InlineData("1", "001", 0, 3, Padding.Left, true)]
        [InlineData("1", "01", 0, 3, Padding.Left, true)]
        [InlineData("1", "1", 0, 3, Padding.Left, true)]
        [InlineData("1", "0,001", 0, 3, Padding.Right, true)]
        [InlineData("1", ",001", 0, 3, Padding.Right, true)]
        [InlineData("1", "001", 0, 3, Padding.Right, true)]
        [InlineData("1", "01", 0, 3, Padding.Right, true)]
        [InlineData("1", "1", 0, 3, Padding.Right, true)]
        [InlineData("-1", "-0,001", 0, 3, Padding.Left, true)]
        [InlineData("-1", "-,001", 0, 3, Padding.Left, true)]
        [InlineData("-1", "-001", 0, 3, Padding.Left, true)]
        [InlineData("-1", "-01", 0, 3, Padding.Left, true)]
        [InlineData("-1", "-1", 0, 3, Padding.Left, true)]
        [InlineData("-1", "1", 0, 3, Padding.Left, true)]
        [InlineData("-1", "-0,001", 0, 3, Padding.Right, true)]
        [InlineData("-1", "-,001", 0, 3, Padding.Right, true)]
        [InlineData("-1", "-001", 0, 3, Padding.Right, true)]
        [InlineData("-1", "-01", 0, 3, Padding.Right, true)]
        [InlineData("-1", "-1", 0, 3, Padding.Right, true)]
        [InlineData("-1", "1", 0, 3, Padding.Right, true)]
        [InlineData("18446744073709551615", ",5", 0, 1, Padding.Left, false)]
        [InlineData("18446744073709551615", "5", 0, 1, Padding.Left, false)]
        [InlineData("18446744073709551615", ",5", 0, 1, Padding.Right, false)]
        [InlineData("18446744073709551615", "5", 0, 1, Padding.Right, false)]
        public void FormatDecimal(string input, string output, byte scale, int groupingSize, Padding padding, bool zerofill)
        {
            var value = Decimal.Parse(input);
            var expected = Encoding.ASCII.GetBytes(output);
            var buffer = new byte[expected.Length];
            BytesHelper.FormatDecimalLimited64(buffer, 0, buffer.Length, value, scale, groupingSize, padding, zerofill, (byte)' ');
            Assert.Equal(expected, buffer);
        }

        // TODO Applicable 上に組み込む？
        //    // TODO 3 小数点多い、四捨五入
    }
}