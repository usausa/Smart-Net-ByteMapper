namespace Smart.IO.ByteMapper.Helpers
{
    //using System;
    //using System.Collections.Generic;
    using System.Globalization;
    //using System.Linq;
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

            // Zero
            expected = Encoding.ASCII.GetBytes(" 0");
            buffer = new byte[expected.Length];
            BytesHelper.FormatDecimalLimited64(buffer, 0, buffer.Length, 0m, 0, -1, Padding.Left, false);
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

            // 32bit
            expected = Encoding.ASCII.GetBytes(" 4294967295");
            buffer = new byte[expected.Length];
            BytesHelper.FormatDecimalLimited64(buffer, 0, buffer.Length, 4294967295m, 0, -1, Padding.Left, false);
            Assert.Equal(expected, buffer);

            // 32bit+1
            expected = Encoding.ASCII.GetBytes(" 4294967296");
            buffer = new byte[expected.Length];
            BytesHelper.FormatDecimalLimited64(buffer, 0, buffer.Length, 4294967296m, 0, -1, Padding.Left, false);
            Assert.Equal(expected, buffer);

            // 64bit
            expected = Encoding.ASCII.GetBytes(" 18446744073709551615");
            buffer = new byte[expected.Length];
            BytesHelper.FormatDecimalLimited64(buffer, 0, buffer.Length, 18446744073709551615m, 0, -1, Padding.Left, false);
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

            // Zero
            expected = Encoding.ASCII.GetBytes("00");
            buffer = new byte[expected.Length];
            BytesHelper.FormatDecimalLimited64(buffer, 0, buffer.Length, 0m, 0, -1, Padding.Left, true);
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

            // 32bit
            expected = Encoding.ASCII.GetBytes("04294967295");
            buffer = new byte[expected.Length];
            BytesHelper.FormatDecimalLimited64(buffer, 0, buffer.Length, 4294967295m, 0, -1, Padding.Left, true);
            Assert.Equal(expected, buffer);

            // 32bit+1
            expected = Encoding.ASCII.GetBytes("04294967296");
            buffer = new byte[expected.Length];
            BytesHelper.FormatDecimalLimited64(buffer, 0, buffer.Length, 4294967296m, 0, -1, Padding.Left, true);
            Assert.Equal(expected, buffer);

            // 64bit
            expected = Encoding.ASCII.GetBytes("018446744073709551615");
            buffer = new byte[expected.Length];
            BytesHelper.FormatDecimalLimited64(buffer, 0, buffer.Length, 18446744073709551615m, 0, -1, Padding.Left, true);
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

            // Zero
            expected = Encoding.ASCII.GetBytes("0 ");
            buffer = new byte[expected.Length];
            BytesHelper.FormatDecimalLimited64(buffer, 0, buffer.Length, 0m, 0, -1, Padding.Right, false);
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

            // 32bit
            expected = Encoding.ASCII.GetBytes("4294967295 ");
            buffer = new byte[expected.Length];
            BytesHelper.FormatDecimalLimited64(buffer, 0, buffer.Length, 4294967295m, 0, -1, Padding.Right, false);
            Assert.Equal(expected, buffer);

            // 32bit+1
            expected = Encoding.ASCII.GetBytes("4294967296 ");
            buffer = new byte[expected.Length];
            BytesHelper.FormatDecimalLimited64(buffer, 0, buffer.Length, 4294967296m, 0, -1, Padding.Right, false);
            Assert.Equal(expected, buffer);

            // 64bit
            expected = Encoding.ASCII.GetBytes("18446744073709551615 ");
            buffer = new byte[expected.Length];
            BytesHelper.FormatDecimalLimited64(buffer, 0, buffer.Length, 18446744073709551615m, 0, -1, Padding.Right, false);
            Assert.Equal(expected, buffer);

            // TODO

            // TODO 小数点不足
            // TODO 小数点多い、四捨五入
            // TODO マイナスの以下パター
            //            Assert.Equal("-0,123,456,789,012,345,678", Encoding.ASCII.GetString(buffer, 0, 26));
            //            Assert.Equal("0,0012,3456,7890,1234,5678", Encoding.ASCII.GetString(buffer, 0, 26));
            //            Assert.Equal("-,0012,3456,7890,1234,5678", Encoding.ASCII.GetString(buffer, 0, 26));
            // TODO .位置が微妙な場合
        }

        //[Fact]
        //public void FormatDecimalScale()
        //{
        //    // TODO 桁を変えつつ！
        //    var values = new[]
        //    {
        //        "0",
        //        "999999999999999999",
        //        "4294967295",
        //        "4294967296",
        //        "18446744073709551615",
        //    };

        //    foreach (var pair in values.SelectMany(Dotnize))
        //    {
        //        var value = Decimal.Parse(pair.Item1);

        //        var expected = Encoding.ASCII.GetBytes(pair.Item1);
        //        var buffer = new byte[expected.Length];

        //        BytesHelper.FormatDecimalLimited64(buffer, 0, buffer.Length, value, pair.Item2, -1, Padding.Left, false);
        //        Assert.Equal(expected, buffer);

        //        BytesHelper.FormatDecimalLimited64(buffer, 0, buffer.Length, value, pair.Item2, -1, Padding.Right, false);
        //        Assert.Equal(expected, buffer);
        //    }
        //}

        //[Fact]
        //public void FormatDecimalGrouping()
        //{
        //    // TODO 桁を変えつつ！
        //}

        //private static IEnumerable<Tuple<string, byte>> Dotnize(string value)
        //{
        //    for (var i = 1; i < value.Length; i++)
        //    {
        //        yield return Tuple.Create(value.Substring(0, i) + "." + value.Substring(i), (byte)(value.Length - i));
        //    }

        //    yield return Tuple.Create(value, (byte)0);
        //}
    }
}