namespace ByteHelperTest.Tests
{
    using System.Text;

    using Xunit;

    public class DecimalTest
    {
        //[Fact]
        //public void TryParseDecimal28()
        //{
        //    var buffer = Encoding.ASCII.GetBytes("12345678901234567890123456.78");
        //    var ret = ByteHelper.TryParseDecimal(buffer, 0, buffer.Length, out var value);
        //    Assert.True(ret);
        //    Assert.Equal(12345678901234567890123456.78m, value);

        //    buffer = Encoding.ASCII.GetBytes("-12345678901234567890123456.78");
        //    ret = ByteHelper.TryParseDecimal(buffer, 0, buffer.Length, out value);
        //    Assert.True(ret);
        //    Assert.Equal(-12345678901234567890123456.78m, value);

        //    buffer = Encoding.ASCII.GetBytes("0");
        //    ret = ByteHelper.TryParseDecimal(buffer, 0, buffer.Length, out value);
        //    Assert.True(ret);
        //    Assert.Equal(0m, value);

        //    buffer = Encoding.ASCII.GetBytes("-0");
        //    ret = ByteHelper.TryParseDecimal(buffer, 0, buffer.Length, out value);
        //    Assert.True(ret);
        //    Assert.Equal(0m, value);

        //    buffer = Encoding.ASCII.GetBytes(" 12345678901234567890123456.78 ");
        //    ret = ByteHelper.TryParseDecimal(buffer, 0, buffer.Length, out value);
        //    Assert.True(ret);
        //    Assert.Equal(12345678901234567890123456.78m, value);

        //    buffer = Encoding.ASCII.GetBytes("1234567890 1234567890123456.78");
        //    ret = ByteHelper.TryParseDecimal(buffer, 0, buffer.Length, out value);
        //    Assert.False(ret);

        //    buffer = Encoding.ASCII.GetBytes("a12345678901234567890123456.78");
        //    ret = ByteHelper.TryParseDecimal(buffer, 0, buffer.Length, out value);
        //    Assert.False(ret);

        //    buffer = Encoding.ASCII.GetBytes("12345678901234567890123456.78a");
        //    ret = ByteHelper.TryParseDecimal(buffer, 0, buffer.Length, out value);
        //    Assert.False(ret);
        //}

        //[Fact]
        //public void FormatDecimal2()
        //{
        //    var buffer = new byte[32];

        //    // TODO

        //    // Left

        //    buffer.Fill(0, buffer.Length, 0);
        //    ByteHelper.FormatDecimal2(buffer, 0, 10, 1234567m, 0, Padding.Left, false, -1);
        //    Assert.Equal("   1234567", Encoding.ASCII.GetString(buffer, 0, 10));

        //    buffer.Fill(0, buffer.Length, 0);
        //    ByteHelper.FormatDecimal2(buffer, 0, 10, 12345.67m, 2, Padding.Left, false, -1);
        //    Assert.Equal("  12345.67", Encoding.ASCII.GetString(buffer, 0, 10));

        //    buffer.Fill(0, buffer.Length, 0);
        //    ByteHelper.FormatDecimal2(buffer, 0, 10, -12345.67m, 2, Padding.Left, false, -1);
        //    Assert.Equal(" -12345.67", Encoding.ASCII.GetString(buffer, 0, 10));

        //    buffer.Fill(0, buffer.Length, 0);
        //    ByteHelper.FormatDecimal2(buffer, 0, 10, 12345.67m, 3, Padding.Left, false, -1);
        //    Assert.Equal(" 12345.670", Encoding.ASCII.GetString(buffer, 0, 10));

        //    buffer.Fill(0, buffer.Length, 0);
        //    ByteHelper.FormatDecimal2(buffer, 0, 10, 12345.67m, 1, Padding.Left, false, -1);
        //    Assert.Equal("   12345.6", Encoding.ASCII.GetString(buffer, 0, 10));

        //    // Zero

        //    buffer.Fill(0, buffer.Length, 0);
        //    ByteHelper.FormatDecimal2(buffer, 0, 10, 1234567m, 0, Padding.Left, true, -1);
        //    Assert.Equal("0001234567", Encoding.ASCII.GetString(buffer, 0, 10));

        //    buffer.Fill(0, buffer.Length, 0);
        //    ByteHelper.FormatDecimal2(buffer, 0, 10, 12345.67m, 2, Padding.Left, true, -1);
        //    Assert.Equal("0012345.67", Encoding.ASCII.GetString(buffer, 0, 10));

        //    buffer.Fill(0, buffer.Length, 0);
        //    ByteHelper.FormatDecimal2(buffer, 0, 10, -12345.67m, 2, Padding.Left, true, -1);
        //    Assert.Equal("-012345.67", Encoding.ASCII.GetString(buffer, 0, 10));

        //    buffer.Fill(0, buffer.Length, 0);
        //    ByteHelper.FormatDecimal2(buffer, 0, 10, 12345.67m, 3, Padding.Left, true, -1);
        //    Assert.Equal("012345.670", Encoding.ASCII.GetString(buffer, 0, 10));

        //    buffer.Fill(0, buffer.Length, 0);
        //    ByteHelper.FormatDecimal2(buffer, 0, 10, 12345.67m, 1, Padding.Left, true, -1);
        //    Assert.Equal("00012345.6", Encoding.ASCII.GetString(buffer, 0, 10));

        //    // Right

        //    buffer.Fill(0, buffer.Length, 0);
        //    ByteHelper.FormatDecimal2(buffer, 0, 10, 1234567m, 0, Padding.Right, false, -1);
        //    Assert.Equal("1234567   ", Encoding.ASCII.GetString(buffer, 0, 10));

        //    buffer.Fill(0, buffer.Length, 0);
        //    ByteHelper.FormatDecimal2(buffer, 0, 10, 12345.67m, 2, Padding.Right, false, -1);
        //    Assert.Equal("12345.67  ", Encoding.ASCII.GetString(buffer, 0, 10));

        //    buffer.Fill(0, buffer.Length, 0);
        //    ByteHelper.FormatDecimal2(buffer, 0, 10, -12345.67m, 2, Padding.Right, false, -1);
        //    Assert.Equal("-12345.67 ", Encoding.ASCII.GetString(buffer, 0, 10));

        //    buffer.Fill(0, buffer.Length, 0);
        //    ByteHelper.FormatDecimal2(buffer, 0, 10, 12345.67m, 3, Padding.Right, false, -1);
        //    Assert.Equal("12345.670 ", Encoding.ASCII.GetString(buffer, 0, 10));

        //    buffer.Fill(0, buffer.Length, 0);
        //    ByteHelper.FormatDecimal2(buffer, 0, 10, 12345.67m, 1, Padding.Right, false, -1);
        //    Assert.Equal("12345.6   ", Encoding.ASCII.GetString(buffer, 0, 10));
        //}

        //[Fact]
        //public void FormatDecimal2Grouping()
        //{
        //    var buffer = new byte[32];

        //    // Left
        //    buffer.Fill(0, buffer.Length, 0);
        //    ByteHelper.FormatDecimal2(buffer, 0, 26, 123456789012345678m, 0, Padding.Left, false, 3);
        //    Assert.Equal("   123,456,789,012,345,678", Encoding.ASCII.GetString(buffer, 0, 26));

        //    buffer.Fill(0, buffer.Length, 0);
        //    ByteHelper.FormatDecimal2(buffer, 0, 26, -123456789012345678m, 0, Padding.Left, false, 3);
        //    Assert.Equal("  -123,456,789,012,345,678", Encoding.ASCII.GetString(buffer, 0, 26));

        //    buffer.Fill(0, buffer.Length, 0);
        //    ByteHelper.FormatDecimal2(buffer, 0, 26, 123456789012345678m, 0, Padding.Left, false, 4);
        //    Assert.Equal("    12,3456,7890,1234,5678", Encoding.ASCII.GetString(buffer, 0, 26));

        //    buffer.Fill(0, buffer.Length, 0);
        //    ByteHelper.FormatDecimal2(buffer, 0, 26, -123456789012345678m, 0, Padding.Left, false, 4);
        //    Assert.Equal("   -12,3456,7890,1234,5678", Encoding.ASCII.GetString(buffer, 0, 26));

        //    // Zero
        //    buffer.Fill(0, buffer.Length, 0);
        //    ByteHelper.FormatDecimal2(buffer, 0, 26, 123456789012345678m, 0, Padding.Left, true, 3);
        //    Assert.Equal("00,123,456,789,012,345,678", Encoding.ASCII.GetString(buffer, 0, 26));

        //    buffer.Fill(0, buffer.Length, 0);
        //    ByteHelper.FormatDecimal2(buffer, 0, 26, -123456789012345678m, 0, Padding.Left, true, 3);
        //    Assert.Equal("-0,123,456,789,012,345,678", Encoding.ASCII.GetString(buffer, 0, 26));

        //    buffer.Fill(0, buffer.Length, 0);
        //    ByteHelper.FormatDecimal2(buffer, 0, 26, 123456789012345678m, 0, Padding.Left, true, 4);
        //    Assert.Equal("0,0012,3456,7890,1234,5678", Encoding.ASCII.GetString(buffer, 0, 26));

        //    buffer.Fill(0, buffer.Length, 0);
        //    ByteHelper.FormatDecimal2(buffer, 0, 26, -123456789012345678m, 0, Padding.Left, true, 4);
        //    Assert.Equal("-,0012,3456,7890,1234,5678", Encoding.ASCII.GetString(buffer, 0, 26));

        //    // Right
        //    buffer.Fill(0, buffer.Length, 0);
        //    ByteHelper.FormatDecimal2(buffer, 0, 26, 123456789012345678m, 0, Padding.Right, false, 3);
        //    Assert.Equal("123,456,789,012,345,678   ", Encoding.ASCII.GetString(buffer, 0, 26));

        //    buffer.Fill(0, buffer.Length, 0);
        //    ByteHelper.FormatDecimal2(buffer, 0, 26, -123456789012345678m, 0, Padding.Right, false, 3);
        //    Assert.Equal("-123,456,789,012,345,678  ", Encoding.ASCII.GetString(buffer, 0, 26));

        //    buffer.Fill(0, buffer.Length, 0);
        //    ByteHelper.FormatDecimal2(buffer, 0, 26, 123456789012345678m, 0, Padding.Right, false, 4);
        //    Assert.Equal("12,3456,7890,1234,5678    ", Encoding.ASCII.GetString(buffer, 0, 26));

        //    buffer.Fill(0, buffer.Length, 0);
        //    ByteHelper.FormatDecimal2(buffer, 0, 26, -123456789012345678m, 0, Padding.Right, false, 4);
        //    Assert.Equal("-12,3456,7890,1234,5678   ", Encoding.ASCII.GetString(buffer, 0, 26));
        //}
    }
}
