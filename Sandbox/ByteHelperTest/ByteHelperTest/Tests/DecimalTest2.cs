namespace ByteHelperTest.Tests
{
    using System;
    using System.Text;

    using Xunit;

    public class DecimalTest2
    {
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
        // Scale short
        [InlineData("0.4294967295", " 0.429496730", 9, -1, Padding.Left, false)]
        [InlineData("0.4294967295", "00.429496730", 9, -1, Padding.Left, true)]
        [InlineData("0.4294967295", "0.429496730 ", 9, -1, Padding.Right, false)]
        // 32bit + 1 scale
        [InlineData("0.4294967296", " 0.4294967296", 10, -1, Padding.Left, false)]
        [InlineData("0.4294967296", "00.4294967296", 10, -1, Padding.Left, true)]
        [InlineData("0.4294967296", "0.4294967296 ", 10, -1, Padding.Right, false)]
        // Buffer short
        [InlineData("0.4294967296", ".4294967296", 10, -1, Padding.Left, false)]
        [InlineData("0.4294967296", ".4294967296", 10, -1, Padding.Right, false)]
        [InlineData("0.4294967296", "4294967296", 10, -1, Padding.Left, false)]
        [InlineData("0.4294967296", "4294967296", 10, -1, Padding.Right, false)]
        // Scale short
        [InlineData("0.4294967296", " 0.429496730", 9, -1, Padding.Left, false)]
        [InlineData("0.4294967296", "00.429496730", 9, -1, Padding.Left, true)]
        [InlineData("0.4294967296", "0.429496730 ", 9, -1, Padding.Right, false)]
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
        // Scale short
        [InlineData("0.18446744073709551615", " 0.1844674407370955162", 19, -1, Padding.Left, false)]
        [InlineData("0.18446744073709551615", "00.1844674407370955162", 19, -1, Padding.Left, true)]
        [InlineData("0.18446744073709551615", "0.1844674407370955162 ", 19, -1, Padding.Right, false)]
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
        // Scale short
        [InlineData("1.23456", "1.2346", 4, -1, Padding.Left, false)]
        [InlineData("1.23456", "1.235", 3, -1, Padding.Left, false)]
        [InlineData("1.23456", "1.23", 2, -1, Padding.Left, false)]
        [InlineData("1.000000001", "1", 0, -1, Padding.Left, false)]
        [InlineData("0.4294967295", "0", 0, -1, Padding.Left, false)]
        [InlineData("0.4294967296", "0", 0, -1, Padding.Left, false)]
        [InlineData("0.5000000001", "1", 0, -1, Padding.Left, false)]
        [InlineData("0.18446744073709551615", "0", 0, -1, Padding.Left, false)]
        // Scale shortage
        [InlineData("0.123", " 0.12300", 5, -1, Padding.Left, false)]
        [InlineData("0.123", "00.12300", 5, -1, Padding.Left, true)]
        [InlineData("0.123", "0.12300 ", 5, -1, Padding.Right, false)]
        [InlineData("1", ".0", 1, -1, Padding.Left, false)]
        [InlineData("1", ".0", 1, -1, Padding.Left, true)]
        [InlineData("1", ".0", 1, -1, Padding.Right, false)]
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
        // TODO 0.00xxxxxxxxxxxxxxxxxxxxxxxxxx ? 127パターンも
        //[InlineData("0.0999999999999999999", " 0.100000000000000000", 18, -1, Padding.Left, false)]
        //[InlineData("0.0999999999999999999", "00.100000000000000000", 18, -1, Padding.Left, true)]
        //[InlineData("0.0999999999999999999", "0.100000000000000000 ", 18, -1, Padding.Right, false)]
        //[InlineData("0.01", " 0.01", 2, -1, Padding.Left, false)]
        //[InlineData("0.01", "00.01", 2, -1, Padding.Left, true)]
        //[InlineData("0.01", "0.01 ", 2, -1, Padding.Right, false)]
        public void FormatDecimal(string input, string output, byte scale, int groupingSize, Padding padding, bool zerofill)
        {
            var value = Decimal.Parse(input);
            var expected = Encoding.ASCII.GetBytes(output);
            var buffer = new byte[expected.Length];
            ByteHelper2.FormatDecimal(buffer, 0, buffer.Length, value, scale, groupingSize, padding, zerofill, (byte)' ');
            var actual = Encoding.ASCII.GetString(buffer);
            System.Diagnostics.Debug.WriteLine(actual);
            Assert.Equal(output, actual);
            //Assert.Equal(expected, buffer);
        }
    }
}
