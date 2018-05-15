namespace Smart.IO.ByteMapper.Helpers
{
    using System;
    using System.Text;

    using Xunit;

    public class NumberHelperDecimalTest
    {
        [Theory]
        // Default grouping scale
        [InlineData("1234567890123456789012345.6789", "  1,234,567,890,123,456,789,012,345.6789", 4, 3, Padding.Left, false)]
        [InlineData("1234567890123456789012345.6789", "001,234,567,890,123,456,789,012,345.6789", 4, 3, Padding.Left, true)]
        [InlineData("1234567890123456789012345.6789", "1,234,567,890,123,456,789,012,345.6789  ", 4, 3, Padding.Right, false)]
        // Negative grouping scale
        [InlineData("-1234567890123456789012345.6789", " -1,234,567,890,123,456,789,012,345.6789", 4, 3, Padding.Left, false)]
        [InlineData("-1234567890123456789012345.6789", "-01,234,567,890,123,456,789,012,345.6789", 4, 3, Padding.Left, true)]
        [InlineData("-1234567890123456789012345.6789", "-1,234,567,890,123,456,789,012,345.6789 ", 4, 3, Padding.Right, false)]
        // Zero
        [InlineData("0", " 0", 0, -1, Padding.Left, false)]
        [InlineData("0", "00", 0, -1, Padding.Left, true)]
        [InlineData("0", "0 ", 0, -1, Padding.Right, false)]
        [InlineData("0", " 0.0", 1, -1, Padding.Left, false)]
        [InlineData("0", "00.0", 1, -1, Padding.Left, true)]
        [InlineData("0", "0.0 ", 1, -1, Padding.Right, false)]
        // Max
        [InlineData("79228162514264337593543950335", "  79228162514264337593543950335", 0, -1, Padding.Left, false)]
        [InlineData("79228162514264337593543950335", "0079228162514264337593543950335", 0, -1, Padding.Left, true)]
        [InlineData("79228162514264337593543950335", "79228162514264337593543950335  ", 0, -1, Padding.Right, false)]
        // Max grouping
        [InlineData("79228162514264337593543950335", "  79,228,162,514,264,337,593,543,950,335", 0, 3, Padding.Left, false)]
        [InlineData("79228162514264337593543950335", ",079,228,162,514,264,337,593,543,950,335", 0, 3, Padding.Left, true)]
        [InlineData("79228162514264337593543950335", "79,228,162,514,264,337,593,543,950,335  ", 0, 3, Padding.Right, false)]
        // Max Negative grouping
        [InlineData("-79228162514264337593543950335", " -79,228,162,514,264,337,593,543,950,335", 0, 3, Padding.Left, false)]
        [InlineData("-79228162514264337593543950335", "-079,228,162,514,264,337,593,543,950,335", 0, 3, Padding.Left, true)]
        [InlineData("-79228162514264337593543950335", "-79,228,162,514,264,337,593,543,950,335 ", 0, 3, Padding.Right, false)]
        // Scale
        [InlineData("0.01", " 0.01", 2, -1, Padding.Left, false)]
        [InlineData("0.01", "00.01", 2, -1, Padding.Left, true)]
        [InlineData("0.01", "0.01 ", 2, -1, Padding.Right, false)]
        // Scale short
        [InlineData("1.23456", "1.2346", 4, -1, Padding.Left, false)]
        [InlineData("1.23456", "1.235", 3, -1, Padding.Left, false)]
        [InlineData("1.23456", "1.23", 2, -1, Padding.Left, false)]
        [InlineData("1.000000001", "1", 0, -1, Padding.Left, false)]
        [InlineData("0.5000000001", "1", 0, -1, Padding.Left, false)]
        // Scale shortage
        [InlineData("0.123", " 0.12300", 5, -1, Padding.Left, false)]
        [InlineData("0.123", "00.12300", 5, -1, Padding.Left, true)]
        [InlineData("0.123", "0.12300 ", 5, -1, Padding.Right, false)]
        [InlineData("1", ".0", 1, -1, Padding.Left, false)]
        [InlineData("1", ".0", 1, -1, Padding.Left, true)]
        [InlineData("1", ".0", 1, -1, Padding.Right, false)]
        // Parse time fixed
        [InlineData("0.09999999999999999999999999999", " 0.1000000000000000000000000000", 28, -1, Padding.Left, false)]
        [InlineData("0.09999999999999999999999999999", "00.1000000000000000000000000000", 28, -1, Padding.Left, true)]
        [InlineData("0.09999999999999999999999999999", "0.1000000000000000000000000000 ", 28, -1, Padding.Right, false)]
        // Over scale
        [InlineData("0.9999999999999999999999999999", " 1.000000000000000000000000000", 27, -1, Padding.Left, false)]
        [InlineData("0.9999999999999999999999999999", "01.000000000000000000000000000", 27, -1, Padding.Left, true)]
        // Over scale buffer short
        [InlineData("0.9999999999999999999999999999", ".000000000000000000000000000", 27, -1, Padding.Left, false)]
        [InlineData("0.9999999999999999999999999999", ".000000000000000000000000000", 27, -1, Padding.Right, false)]
        [InlineData("0.9999999999999999999999999999", "000000000000000000000000000", 27, -1, Padding.Left, false)]
        [InlineData("0.9999999999999999999999999999", "000000000000000000000000000", 27, -1, Padding.Right, false)]
        // Buffer short
        [InlineData("1.23", "1.23", 2, -1, Padding.Left, false)]
        [InlineData("1.23", ".23", 2, -1, Padding.Left, false)]
        [InlineData("1.23", "23", 2, -1, Padding.Left, false)]
        [InlineData("1.23", "3", 2, -1, Padding.Left, false)]
        [InlineData("1.23", "1.23", 2, -1, Padding.Right, false)]
        [InlineData("1.23", ".23", 2, -1, Padding.Right, false)]
        [InlineData("1.23", "23", 2, -1, Padding.Right, false)]
        [InlineData("1.23", "3", 2, -1, Padding.Right, false)]
        [InlineData("-1.23", "-1.23", 2, -1, Padding.Left, false)]
        [InlineData("-1.23", "1.23", 2, -1, Padding.Left, false)]
        [InlineData("-1.23", ".23", 2, -1, Padding.Left, false)]
        [InlineData("-1.23", "23", 2, -1, Padding.Left, false)]
        [InlineData("-1.23", "3", 2, -1, Padding.Left, false)]
        [InlineData("-1.23", "-1.23", 2, -1, Padding.Right, false)]
        [InlineData("-1.23", "1.23", 2, -1, Padding.Right, false)]
        [InlineData("-1.23", ".23", 2, -1, Padding.Right, false)]
        [InlineData("-1.23", "23", 2, -1, Padding.Right, false)]
        [InlineData("-1.23", "3", 2, -1, Padding.Right, false)]
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
        public void FormatDecimal(string input, string output, byte scale, int groupingSize, Padding padding, bool zerofill)
        {
            var value = Decimal.Parse(input);
            var expected = Encoding.ASCII.GetBytes(output);
            var buffer = new byte[expected.Length];
            NumberHelper.FormatDecimal(buffer, 0, buffer.Length, value, scale, groupingSize, padding, zerofill, (byte)' ');
            Assert.Equal(expected, buffer);
        }
    }
}