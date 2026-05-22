// ReSharper disable RedundantArgumentDefaultValue
// ReSharper disable UseUtf8StringLiteral
#pragma warning disable IDE0230
namespace Smart.IO.ByteMapper.Converters;

public class BinaryConverterTests
{
    // ---- int ----

    [Fact]
    public void WhenReadIntBigEndianThenCorrectValue()
    {
        var converter = new BinaryConverter<int>(Endian.Big);
        var buffer = new byte[] { 0x00, 0x00, 0x30, 0x39 }; // 12345
        Assert.Equal(12345, converter.Read(buffer));
    }

    [Fact]
    public void WhenWriteIntBigEndianThenCorrectBytes()
    {
        var converter = new BinaryConverter<int>(Endian.Big);
        var buffer = new byte[4];
        converter.Write(buffer, 12345);
        Assert.Equal(new byte[] { 0x00, 0x00, 0x30, 0x39 }, buffer);
    }

    [Fact]
    public void WhenReadIntLittleEndianThenCorrectValue()
    {
        var converter = new BinaryConverter<int>(Endian.Little);
        var buffer = new byte[] { 0x39, 0x30, 0x00, 0x00 }; // 12345 LE
        Assert.Equal(12345, converter.Read(buffer));
    }

    [Fact]
    public void WhenWriteIntLittleEndianThenCorrectBytes()
    {
        var converter = new BinaryConverter<int>(Endian.Little);
        var buffer = new byte[4];
        converter.Write(buffer, 12345);
        Assert.Equal(new byte[] { 0x39, 0x30, 0x00, 0x00 }, buffer);
    }

    // ---- round-trip ----

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(-1)]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    public void WhenIntRoundTripBigEndianThenSameValue(int value)
    {
        var converter = new BinaryConverter<int>(Endian.Big);
        var buffer = new byte[BinaryConverter<int>.Size];
        converter.Write(buffer, value);
        Assert.Equal(value, converter.Read(buffer));
    }

    [Theory]
    [InlineData(0L)]
    [InlineData(long.MaxValue)]
    [InlineData(long.MinValue)]
    public void WhenLongRoundTripBigEndianThenSameValue(long value)
    {
        var converter = new BinaryConverter<long>(Endian.Big);
        var buffer = new byte[BinaryConverter<long>.Size];
        converter.Write(buffer, value);
        Assert.Equal(value, converter.Read(buffer));
    }

    [Fact]
    public void WhenSizeIsCorrectForInt()
    {
        Assert.Equal(4, BinaryConverter<int>.Size);
    }

    [Fact]
    public void WhenSizeIsCorrectForLong()
    {
        Assert.Equal(8, BinaryConverter<long>.Size);
    }
}
