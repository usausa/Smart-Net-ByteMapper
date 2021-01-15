namespace ByteHelperTest.Tests
{
    using System;
    using System.Globalization;
    using System.Text;

    using Xunit;

    public class DecimalTest
    {
        [Fact]
        public void ParseDecimal()
        {
            // TODO

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

            // Failed

            // Empty
            buffer = Encoding.ASCII.GetBytes("                  ");
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

        //[Theory]
        //// Default grouping scale
        //[InlineData("1234567890123456.78", "  1,234,567,890,123,456.78", 2, 3, Padding.Left, false)]
        //[InlineData("1234567890123456.78", "001,234,567,890,123,456.78", 2, 3, Padding.Left, true)]
        //[InlineData("1234567890123456.78", "1,234,567,890,123,456.78  ", 2, 3, Padding.Right, false)]
        //// Negative grouping scale
        //[InlineData("-1234567890123456.78", " -1,234,567,890,123,456.78", 2, 3, Padding.Left, false)]
        //[InlineData("-1234567890123456.78", "-01,234,567,890,123,456.78", 2, 3, Padding.Left, true)]
        //[InlineData("-1234567890123456.78", "-1,234,567,890,123,456.78 ", 2, 3, Padding.Right, false)]
        //// Zero
        //[InlineData("0", " 0", 0, -1, Padding.Left, false)]
        //[InlineData("0", "00", 0, -1, Padding.Left, true)]
        //[InlineData("0", "0 ", 0, -1, Padding.Right, false)]
        //[InlineData("0", " 0.0", 1, -1, Padding.Left, false)]
        //[InlineData("0", "00.0", 1, -1, Padding.Left, true)]
        //[InlineData("0", "0.0 ", 1, -1, Padding.Right, false)]
        //// Max grouping
        //[InlineData("999999999999999999", "  999,999,999,999,999,999", 0, 3, Padding.Left, false)]
        //[InlineData("999999999999999999", "0,999,999,999,999,999,999", 0, 3, Padding.Left, true)]
        //[InlineData("999999999999999999", "999,999,999,999,999,999  ", 0, 3, Padding.Right, false)]
        //// Max Negative grouping
        //[InlineData("-999999999999999999", " -999,999,999,999,999,999", 0, 3, Padding.Left, false)]
        //[InlineData("-999999999999999999", "-,999,999,999,999,999,999", 0, 3, Padding.Left, true)]
        //[InlineData("-999999999999999999", "-999,999,999,999,999,999 ", 0, 3, Padding.Right, false)]
        //// Scale
        //[InlineData("0.01", " 0.01", 2, -1, Padding.Left, false)]
        //[InlineData("0.01", "00.01", 2, -1, Padding.Left, true)]
        //[InlineData("0.01", "0.01 ", 2, -1, Padding.Right, false)]
        //// 32bit
        //[InlineData("4294967295", "  4294967295", 0, -1, Padding.Left, false)]
        //[InlineData("4294967295", "004294967295", 0, -1, Padding.Left, true)]
        //[InlineData("4294967295", "4294967295  ", 0, -1, Padding.Right, false)]
        //// 32bit + 1
        //[InlineData("4294967296", "  4294967296", 0, -1, Padding.Left, false)]
        //[InlineData("4294967296", "004294967296", 0, -1, Padding.Left, true)]
        //[InlineData("4294967296", "4294967296  ", 0, -1, Padding.Right, false)]
        //// 32bit scale
        //[InlineData("0.4294967295", " 0.4294967295", 10, -1, Padding.Left, false)]
        //[InlineData("0.4294967295", "00.4294967295", 10, -1, Padding.Left, true)]
        //[InlineData("0.4294967295", "0.4294967295 ", 10, -1, Padding.Right, false)]
        //// Buffer short
        //[InlineData("0.4294967295", ".4294967295", 10, -1, Padding.Left, false)]
        //[InlineData("0.4294967295", ".4294967295", 10, -1, Padding.Right, false)]
        //[InlineData("0.4294967295", "4294967295", 10, -1, Padding.Left, false)]
        //[InlineData("0.4294967295", "4294967295", 10, -1, Padding.Right, false)]
        //// Scale short
        //[InlineData("0.4294967295", " 0.429496730", 9, -1, Padding.Left, false)]
        //[InlineData("0.4294967295", "00.429496730", 9, -1, Padding.Left, true)]
        //[InlineData("0.4294967295", "0.429496730 ", 9, -1, Padding.Right, false)]
        //// 32bit + 1 scale
        //[InlineData("0.4294967296", " 0.4294967296", 10, -1, Padding.Left, false)]
        //[InlineData("0.4294967296", "00.4294967296", 10, -1, Padding.Left, true)]
        //[InlineData("0.4294967296", "0.4294967296 ", 10, -1, Padding.Right, false)]
        //// Buffer short
        //[InlineData("0.4294967296", ".4294967296", 10, -1, Padding.Left, false)]
        //[InlineData("0.4294967296", ".4294967296", 10, -1, Padding.Right, false)]
        //[InlineData("0.4294967296", "4294967296", 10, -1, Padding.Left, false)]
        //[InlineData("0.4294967296", "4294967296", 10, -1, Padding.Right, false)]
        //// Scale short
        //[InlineData("0.4294967296", " 0.429496730", 9, -1, Padding.Left, false)]
        //[InlineData("0.4294967296", "00.429496730", 9, -1, Padding.Left, true)]
        //[InlineData("0.4294967296", "0.429496730 ", 9, -1, Padding.Right, false)]
        //// Parse time fixed
        //[InlineData("0.0999999999999999999", " 0.100000000000000000", 18, -1, Padding.Left, false)]
        //[InlineData("0.0999999999999999999", "00.100000000000000000", 18, -1, Padding.Left, true)]
        //[InlineData("0.0999999999999999999", "0.100000000000000000 ", 18, -1, Padding.Right, false)]
        //// Over scale
        //[InlineData("0.999999999999999999", " 1.00000000000000000", 17, -1, Padding.Left, false)]
        //[InlineData("0.999999999999999999", "01.00000000000000000", 17, -1, Padding.Left, true)]
        //[InlineData("0.999999999999999999", "1.00000000000000000 ", 17, -1, Padding.Right, false)]
        //// Buffer short
        //[InlineData("0.999999999999999999", ".00000000000000000", 17, -1, Padding.Left, false)]
        //[InlineData("0.999999999999999999", ".00000000000000000", 17, -1, Padding.Right, false)]
        //[InlineData("0.999999999999999999", "00000000000000000", 17, -1, Padding.Left, false)]
        //[InlineData("0.999999999999999999", "00000000000000000", 17, -1, Padding.Right, false)]
        //// Buffer short
        //[InlineData("1.23", "1.23", 2, -1, Padding.Left, false)]
        //[InlineData("1.23", ".23", 2, -1, Padding.Left, false)]
        //[InlineData("1.23", "23", 2, -1, Padding.Left, false)]
        //[InlineData("1.23", "1.23", 2, -1, Padding.Right, false)]
        //[InlineData("1.23", ".23", 2, -1, Padding.Right, false)]
        //[InlineData("1.23", "23", 2, -1, Padding.Right, false)]
        //[InlineData("-1.23", "-1.23", 2, -1, Padding.Left, false)]
        //[InlineData("-1.23", "1.23", 2, -1, Padding.Left, false)]
        //[InlineData("-1.23", ".23", 2, -1, Padding.Left, false)]
        //[InlineData("-1.23", "-1.23", 2, -1, Padding.Right, false)]
        //[InlineData("-1.23", "1.23", 2, -1, Padding.Right, false)]
        //[InlineData("-1.23", ".23", 2, -1, Padding.Right, false)]
        //// Scale short
        //[InlineData("1.23456", "1.2346", 4, -1, Padding.Left, false)]
        //[InlineData("1.23456", "1.235", 3, -1, Padding.Left, false)]
        //[InlineData("1.23456", "1.23", 2, -1, Padding.Left, false)]
        //[InlineData("1.000000001", "1", 0, -1, Padding.Left, false)]
        //[InlineData("0.4294967295", "0", 0, -1, Padding.Left, false)]
        //[InlineData("0.4294967296", "0", 0, -1, Padding.Left, false)]
        //[InlineData("0.5000000001", "1", 0, -1, Padding.Left, false)]
        //[InlineData("0.18446744073709551615", "0", 0, -1, Padding.Left, false)]
        //// Scale shortage
        //[InlineData("0.123", " 0.12300", 5, -1, Padding.Left, false)]
        //[InlineData("0.123", "00.12300", 5, -1, Padding.Left, true)]
        //[InlineData("0.123", "0.12300 ", 5, -1, Padding.Right, false)]
        //[InlineData("1", ".0", 1, -1, Padding.Left, false)]
        //[InlineData("1", ".0", 1, -1, Padding.Left, true)]
        //[InlineData("1", ".0", 1, -1, Padding.Right, false)]
        //// Grouping shortage
        //[InlineData("1234", "1,234", 0, 3, Padding.Left, false)]
        //[InlineData("1234", ",234", 0, 3, Padding.Left, false)]
        //[InlineData("1234", "234", 0, 3, Padding.Left, false)]
        //[InlineData("1234", "34", 0, 3, Padding.Left, false)]
        //[InlineData("1234", "4", 0, 3, Padding.Left, false)]
        //[InlineData("1234", "1,234", 0, 3, Padding.Right, false)]
        //[InlineData("1234", ",234", 0, 3, Padding.Right, false)]
        //[InlineData("1234", "234", 0, 3, Padding.Right, false)]
        //[InlineData("1234", "34", 0, 3, Padding.Right, false)]
        //[InlineData("1234", "4", 0, 3, Padding.Right, false)]
        //[InlineData("-1234", "-1,234", 0, 3, Padding.Left, false)]
        //[InlineData("-1234", "1,234", 0, 3, Padding.Left, false)]
        //[InlineData("-1234", ",234", 0, 3, Padding.Left, false)]
        //[InlineData("-1234", "-1,234", 0, 3, Padding.Right, false)]
        //[InlineData("-1234", "1,234", 0, 3, Padding.Right, false)]
        //[InlineData("-1234", ",234", 0, 3, Padding.Right, false)]
        //[InlineData("1", "0,001", 0, 3, Padding.Left, true)]
        //[InlineData("1", ",001", 0, 3, Padding.Left, true)]
        //[InlineData("1", "001", 0, 3, Padding.Left, true)]
        //[InlineData("1", "01", 0, 3, Padding.Left, true)]
        //[InlineData("1", "1", 0, 3, Padding.Left, true)]
        //[InlineData("1", "0,001", 0, 3, Padding.Right, true)]
        //[InlineData("1", ",001", 0, 3, Padding.Right, true)]
        //[InlineData("1", "001", 0, 3, Padding.Right, true)]
        //[InlineData("1", "01", 0, 3, Padding.Right, true)]
        //[InlineData("1", "1", 0, 3, Padding.Right, true)]
        //[InlineData("-1", "-0,001", 0, 3, Padding.Left, true)]
        //[InlineData("-1", "-,001", 0, 3, Padding.Left, true)]
        //[InlineData("-1", "-001", 0, 3, Padding.Left, true)]
        //[InlineData("-1", "-01", 0, 3, Padding.Left, true)]
        //[InlineData("-1", "-1", 0, 3, Padding.Left, true)]
        //[InlineData("-1", "1", 0, 3, Padding.Left, true)]
        //[InlineData("-1", "-0,001", 0, 3, Padding.Right, true)]
        //[InlineData("-1", "-,001", 0, 3, Padding.Right, true)]
        //[InlineData("-1", "-001", 0, 3, Padding.Right, true)]
        //[InlineData("-1", "-01", 0, 3, Padding.Right, true)]
        //[InlineData("-1", "-1", 0, 3, Padding.Right, true)]
        //[InlineData("-1", "1", 0, 3, Padding.Right, true)]
        //[InlineData("123456789012345678", ",8", 0, 1, Padding.Left, false)]
        //[InlineData("123456789012345678", "8", 0, 1, Padding.Left, false)]
        //[InlineData("123456789012345678", ",8", 0, 1, Padding.Right, false)]
        //[InlineData("123456789012345678", "8", 0, 1, Padding.Right, false)]
        //public void FormatDecimalLimited64(string input, string output, byte scale, int groupingSize, Padding padding, bool zerofill)
        //{
        //    var bytes = Encoding.ASCII.GetBytes(input);
        //    ByteHelper2.TryParseDecimal(bytes, 0, bytes.Length, (byte)' ', out var value);
        //    var expected = Encoding.ASCII.GetBytes(output);
        //    var buffer = new byte[expected.Length];
        //    ByteHelper2.FormatDecimalLimited64(buffer, 0, buffer.Length, value, scale, groupingSize, padding, zerofill, (byte)' ');
        //    Assert.Equal(expected, buffer);
        //}

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
        // Max grouping
        [InlineData("9999999999999999999999999999", "  9,999,999,999,999,999,999,999,999,999", 0, 3, Padding.Left, false)]
        [InlineData("9999999999999999999999999999", "009,999,999,999,999,999,999,999,999,999", 0, 3, Padding.Left, true)]
        [InlineData("9999999999999999999999999999", "9,999,999,999,999,999,999,999,999,999  ", 0, 3, Padding.Right, false)]
        // Max Negative grouping
        [InlineData("-9999999999999999999999999999", " -9,999,999,999,999,999,999,999,999,999", 0, 3, Padding.Left, false)]
        [InlineData("-9999999999999999999999999999", "-09,999,999,999,999,999,999,999,999,999", 0, 3, Padding.Left, true)]
        [InlineData("-9999999999999999999999999999", "-9,999,999,999,999,999,999,999,999,999 ", 0, 3, Padding.Right, false)]
        // Scale
        [InlineData("0.01", " 0.01", 2, -1, Padding.Left, false)]
        [InlineData("0.01", "00.01", 2, -1, Padding.Left, true)]
        [InlineData("0.01", "0.01 ", 2, -1, Padding.Right, false)]
        // 64bit
        [InlineData("18446744073709551615", "  18446744073709551615", 0, -1, Padding.Left, false)]
        [InlineData("18446744073709551615", "0018446744073709551615", 0, -1, Padding.Left, true)]
        [InlineData("18446744073709551615", "18446744073709551615  ", 0, -1, Padding.Right, false)]
        // 64bit + 1
        [InlineData("4294967296", "  4294967296", 0, -1, Padding.Left, false)]
        [InlineData("18446744073709551616", "0018446744073709551616", 0, -1, Padding.Left, true)]
        [InlineData("18446744073709551616", "18446744073709551616  ", 0, -1, Padding.Right, false)]
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
        // 64bit + 1 scale
        [InlineData("0.18446744073709551616", " 0.18446744073709551616", 20, -1, Padding.Left, false)]
        [InlineData("0.18446744073709551616", "00.18446744073709551616", 20, -1, Padding.Left, true)]
        [InlineData("0.18446744073709551616", "0.18446744073709551616 ", 20, -1, Padding.Right, false)]
        // Buffer short
        [InlineData("0.18446744073709551616", ".18446744073709551616", 20, -1, Padding.Left, false)]
        [InlineData("0.18446744073709551616", ".18446744073709551616", 20, -1, Padding.Right, false)]
        [InlineData("0.18446744073709551616", "18446744073709551616", 20, -1, Padding.Left, false)]
        [InlineData("0.18446744073709551616", "18446744073709551616", 20, -1, Padding.Right, false)]
        // Scale short
        [InlineData("0.18446744073709551616", " 0.1844674407370955162", 19, -1, Padding.Left, false)]
        [InlineData("0.18446744073709551616", "00.1844674407370955162", 19, -1, Padding.Left, true)]
        [InlineData("0.18446744073709551616", "0.1844674407370955162 ", 19, -1, Padding.Right, false)]
        // Parse time fixed
        [InlineData("0.09999999999999999999999999999", " 0.1000000000000000000000000000", 28, -1, Padding.Left, false)]
        [InlineData("0.09999999999999999999999999999", "00.1000000000000000000000000000", 28, -1, Padding.Left, true)]
        [InlineData("0.09999999999999999999999999999", "0.1000000000000000000000000000 ", 28, -1, Padding.Right, false)]
        // Over scale
        [InlineData("0.9999999999999999999999999999", " 1.000000000000000000000000000", 27, -1, Padding.Left, false)]
        [InlineData("0.9999999999999999999999999999", "01.000000000000000000000000000", 27, -1, Padding.Left, true)]
        // Buffer short
        [InlineData("0.9999999999999999999999999999", ".000000000000000000000000000", 27, -1, Padding.Left, false)]
        [InlineData("0.9999999999999999999999999999", ".000000000000000000000000000", 27, -1, Padding.Right, false)]
        [InlineData("0.9999999999999999999999999999", "000000000000000000000000000", 27, -1, Padding.Left, false)]
        [InlineData("0.9999999999999999999999999999", "000000000000000000000000000", 27, -1, Padding.Right, false)]
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
        [InlineData("0.18446744073709551615", "0", 0, -1, Padding.Left, false)]
        [InlineData("0.18446744073709551616", "0", 0, -1, Padding.Left, false)]
        [InlineData("0.5000000001", "1", 0, -1, Padding.Left, false)]
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
        [InlineData("79228162514264337593543950335", ",5", 0, 1, Padding.Left, false)]
        [InlineData("79228162514264337593543950335", "5", 0, 1, Padding.Left, false)]
        [InlineData("79228162514264337593543950335", ",5", 0, 1, Padding.Right, false)]
        [InlineData("79228162514264337593543950335", "5", 0, 1, Padding.Right, false)]
        public void FormatDecimal(string input, string output, byte scale, int groupingSize, Padding padding, bool zerofill)
        {
            var value = Decimal.Parse(input);
            var expected = Encoding.ASCII.GetBytes(output);
            var buffer = new byte[expected.Length];
            ByteHelper2.FormatDecimal(buffer, 0, buffer.Length, value, scale, groupingSize, padding, zerofill, (byte)' ');
            Assert.Equal(expected, buffer);
        }
    }
}
