namespace Smart.IO.ByteMapper.Helpers;

using System.Globalization;
using System.Text;

public sealed class NumberByteHelperIntegerTest
{
    [Fact]
    public void ParseInteger()
    {
        // Default
        var buffer = "1234567890123456789"u8.ToArray();
        Assert.True(NumberByteHelper.TryParseInt64(buffer, 0, buffer.Length, 0x20, out var value));
        Assert.Equal(1234567890123456789L, value);

        // Negative
        buffer = "-1234567890123456789"u8.ToArray();
        Assert.True(NumberByteHelper.TryParseInt64(buffer, 0, buffer.Length, 0x20, out value));
        Assert.Equal(-1234567890123456789L, value);

        // Padded
        buffer = "0001234567890123456789"u8.ToArray();
        Assert.True(NumberByteHelper.TryParseInt64(buffer, 0, buffer.Length, 0x20, out value));
        Assert.Equal(1234567890123456789L, value);

        // Padded Negative
        buffer = "-0001234567890123456789"u8.ToArray();
        Assert.True(NumberByteHelper.TryParseInt64(buffer, 0, buffer.Length, 0x20, out value));
        Assert.Equal(-1234567890123456789L, value);

        // Zero
        buffer = "0"u8.ToArray();
        Assert.True(NumberByteHelper.TryParseInt64(buffer, 0, buffer.Length, 0x20, out value));
        Assert.Equal(0L, value);

        // Minus zero
        buffer = "-0"u8.ToArray();
        Assert.True(NumberByteHelper.TryParseInt64(buffer, 0, buffer.Length, 0x20, out value));
        Assert.Equal(0L, value);

        // Max value
        buffer = Encoding.ASCII.GetBytes(Int64.MaxValue.ToString(CultureInfo.InvariantCulture));
        Assert.True(NumberByteHelper.TryParseInt64(buffer, 0, buffer.Length, 0x20, out value));
        Assert.Equal(Int64.MaxValue, value);

        // Min value
        buffer = Encoding.ASCII.GetBytes(Int64.MinValue.ToString(CultureInfo.InvariantCulture));
        Assert.True(NumberByteHelper.TryParseInt64(buffer, 0, buffer.Length, 0x20, out value));
        Assert.Equal(Int64.MinValue, value);

        // Trim
        buffer = " 1234567890123456789 "u8.ToArray();
        Assert.True(NumberByteHelper.TryParseInt64(buffer, 0, buffer.Length, 0x20, out value));
        Assert.Equal(1234567890123456789L, value);

        // Int16
        buffer = Encoding.ASCII.GetBytes(Int16.MaxValue.ToString(CultureInfo.InvariantCulture));
        Assert.True(NumberByteHelper.TryParseInt16(buffer, 0, buffer.Length, 0x20, out var shortValue));
        Assert.Equal(Int16.MaxValue, shortValue);

        buffer = Encoding.ASCII.GetBytes(Int16.MinValue.ToString(CultureInfo.InvariantCulture));
        Assert.True(NumberByteHelper.TryParseInt16(buffer, 0, buffer.Length, 0x20, out shortValue));
        Assert.Equal(Int16.MinValue, shortValue);

        // Int32
        buffer = Encoding.ASCII.GetBytes(Int32.MaxValue.ToString(CultureInfo.InvariantCulture));
        Assert.True(NumberByteHelper.TryParseInt32(buffer, 0, buffer.Length, 0x20, out var intValue));
        Assert.Equal(Int32.MaxValue, intValue);

        buffer = Encoding.ASCII.GetBytes(Int32.MinValue.ToString(CultureInfo.InvariantCulture));
        Assert.True(NumberByteHelper.TryParseInt32(buffer, 0, buffer.Length, 0x20, out intValue));
        Assert.Equal(Int32.MinValue, intValue);

        // Failed

        // Empty
        buffer = "                   "u8.ToArray();
        Assert.False(NumberByteHelper.TryParseInt64(buffer, 0, buffer.Length, 0x20, out _));

        // Invalid Value
        buffer = "1234567890 123456789"u8.ToArray();
        Assert.False(NumberByteHelper.TryParseInt64(buffer, 0, buffer.Length, 0x20, out _));

        buffer = "a1234567890123456789"u8.ToArray();
        Assert.False(NumberByteHelper.TryParseInt64(buffer, 0, buffer.Length, 0x20, out _));

        buffer = "1234567890123456789a"u8.ToArray();
        Assert.False(NumberByteHelper.TryParseInt64(buffer, 0, buffer.Length, 0x20, out _));
    }

    [Fact]
    public void FormatInteger()
    {
        // Default
        var buffer = new byte[22];
        NumberByteHelper.FormatInt64(buffer, 0, buffer.Length, 1234567890123456789L, Padding.Left, false, 0x20);
        Assert.Equal("   1234567890123456789", Encoding.ASCII.GetString(buffer, 0, buffer.Length));

        buffer = new byte[22];
        NumberByteHelper.FormatInt64(buffer, 0, buffer.Length, 1234567890123456789L, Padding.Left, true, 0x20);
        Assert.Equal("0001234567890123456789", Encoding.ASCII.GetString(buffer, 0, buffer.Length));

        buffer = new byte[22];
        NumberByteHelper.FormatInt64(buffer, 0, buffer.Length, 1234567890123456789L, Padding.Right, false, 0x20);
        Assert.Equal("1234567890123456789   ", Encoding.ASCII.GetString(buffer, 0, buffer.Length));

        // Negative
        buffer = new byte[22];
        NumberByteHelper.FormatInt64(buffer, 0, buffer.Length, -1234567890123456789L, Padding.Left, false, 0x20);
        Assert.Equal("  -1234567890123456789", Encoding.ASCII.GetString(buffer, 0, buffer.Length));

        buffer = new byte[22];
        NumberByteHelper.FormatInt64(buffer, 0, buffer.Length, -1234567890123456789L, Padding.Left, true, 0x20);
        Assert.Equal("-001234567890123456789", Encoding.ASCII.GetString(buffer, 0, buffer.Length));

        buffer = new byte[22];
        NumberByteHelper.FormatInt64(buffer, 0, buffer.Length, -1234567890123456789L, Padding.Right, false, 0x20);
        Assert.Equal("-1234567890123456789  ", Encoding.ASCII.GetString(buffer, 0, buffer.Length));

        // Max value
        buffer = new byte[22];
        NumberByteHelper.FormatInt64(buffer, 0, buffer.Length, Int64.MaxValue, Padding.Left, false, 0x20);
        Assert.Equal($"{Int64.MaxValue,22}", Encoding.ASCII.GetString(buffer, 0, buffer.Length));

        buffer = new byte[22];
        NumberByteHelper.FormatInt64(buffer, 0, buffer.Length, Int64.MaxValue, Padding.Left, true, 0x20);
        Assert.Equal($"{Int64.MaxValue,22:D22}", Encoding.ASCII.GetString(buffer, 0, buffer.Length));

        buffer = new byte[22];
        NumberByteHelper.FormatInt64(buffer, 0, buffer.Length, Int64.MaxValue, Padding.Right, false, 0x20);
        Assert.Equal($"{Int64.MaxValue,-22}", Encoding.ASCII.GetString(buffer, 0, buffer.Length));

        // Min value
        buffer = new byte[22];
        NumberByteHelper.FormatInt64(buffer, 0, buffer.Length, Int64.MinValue, Padding.Left, false, 0x20);
        Assert.Equal($"{Int64.MinValue,22}", Encoding.ASCII.GetString(buffer, 0, buffer.Length));

        buffer = new byte[22];
        NumberByteHelper.FormatInt64(buffer, 0, buffer.Length, Int64.MinValue, Padding.Left, true, 0x20);
        Assert.Equal($"{Int64.MinValue,22:D21}", Encoding.ASCII.GetString(buffer, 0, buffer.Length));

        buffer = new byte[22];
        NumberByteHelper.FormatInt64(buffer, 0, buffer.Length, Int64.MinValue, Padding.Right, false, 0x20);
        Assert.Equal($"{Int64.MinValue,-22}", Encoding.ASCII.GetString(buffer, 0, buffer.Length));

        // Int16

        // Max value
        buffer = new byte[9];
        NumberByteHelper.FormatInt16(buffer, 0, buffer.Length, Int16.MaxValue, Padding.Left, false, 0x20);
        Assert.Equal($"{Int16.MaxValue,9}", Encoding.ASCII.GetString(buffer, 0, buffer.Length));

        buffer = new byte[9];
        NumberByteHelper.FormatInt16(buffer, 0, buffer.Length, Int16.MaxValue, Padding.Left, true, 0x20);
        Assert.Equal($"{Int16.MaxValue,9:D9}", Encoding.ASCII.GetString(buffer, 0, buffer.Length));

        buffer = new byte[9];
        NumberByteHelper.FormatInt16(buffer, 0, buffer.Length, Int16.MaxValue, Padding.Right, false, 0x20);
        Assert.Equal($"{Int16.MaxValue,-9}", Encoding.ASCII.GetString(buffer, 0, buffer.Length));

        // Min value
        buffer = new byte[9];
        NumberByteHelper.FormatInt16(buffer, 0, buffer.Length, Int16.MinValue, Padding.Left, false, 0x20);
        Assert.Equal($"{Int16.MinValue,9}", Encoding.ASCII.GetString(buffer, 0, buffer.Length));

        buffer = new byte[9];
        NumberByteHelper.FormatInt16(buffer, 0, buffer.Length, Int16.MinValue, Padding.Left, true, 0x20);
        Assert.Equal($"{Int16.MinValue,9:D8}", Encoding.ASCII.GetString(buffer, 0, buffer.Length));

        buffer = new byte[9];
        NumberByteHelper.FormatInt16(buffer, 0, buffer.Length, Int16.MinValue, Padding.Right, false, 0x20);
        Assert.Equal($"{Int16.MinValue,-9}", Encoding.ASCII.GetString(buffer, 0, buffer.Length));

        // Int32

        // Max value
        buffer = new byte[12];
        NumberByteHelper.FormatInt32(buffer, 0, buffer.Length, Int32.MaxValue, Padding.Left, false, 0x20);
        Assert.Equal($"{Int32.MaxValue,12}", Encoding.ASCII.GetString(buffer, 0, buffer.Length));

        buffer = new byte[12];
        NumberByteHelper.FormatInt32(buffer, 0, buffer.Length, Int32.MaxValue, Padding.Left, true, 0x20);
        Assert.Equal($"{Int32.MaxValue,12:D12}", Encoding.ASCII.GetString(buffer, 0, buffer.Length));

        buffer = new byte[12];
        NumberByteHelper.FormatInt32(buffer, 0, buffer.Length, Int32.MaxValue, Padding.Right, false, 0x20);
        Assert.Equal($"{Int32.MaxValue,-12}", Encoding.ASCII.GetString(buffer, 0, buffer.Length));

        // Min value
        buffer = new byte[12];
        NumberByteHelper.FormatInt32(buffer, 0, buffer.Length, Int32.MinValue, Padding.Left, false, 0x20);
        Assert.Equal($"{Int32.MinValue,12}", Encoding.ASCII.GetString(buffer, 0, buffer.Length));

        buffer = new byte[12];
        NumberByteHelper.FormatInt32(buffer, 0, buffer.Length, Int32.MinValue, Padding.Left, true, 0x20);
        Assert.Equal($"{Int32.MinValue,12:D11}", Encoding.ASCII.GetString(buffer, 0, buffer.Length));

        buffer = new byte[12];
        NumberByteHelper.FormatInt32(buffer, 0, buffer.Length, Int32.MinValue, Padding.Right, false, 0x20);
        Assert.Equal($"{Int32.MinValue,-12}", Encoding.ASCII.GetString(buffer, 0, buffer.Length));
    }
}
