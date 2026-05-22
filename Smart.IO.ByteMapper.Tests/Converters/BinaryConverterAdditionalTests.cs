namespace Smart.IO.ByteMapper.Converters;

public class BinaryConverterAdditionalTests
{
    // ---- byte / sbyte ----

    [Fact]
    public void WhenReadByteThenCorrectValue()
    {
        var converter = new BinaryConverter<byte>();
        Assert.Equal((byte)0xFF, converter.Read([0xFF]));
    }

    [Fact]
    public void WhenWriteByteThenCorrectBytes()
    {
        var converter = new BinaryConverter<byte>();
        var buffer = new byte[1];
        converter.Write(buffer, 0xAB);
        Assert.Equal(0xAB, buffer[0]);
    }

    [Fact]
    public void WhenReadSByteThenCorrectValue()
    {
        var converter = new BinaryConverter<sbyte>();
        Assert.Equal((sbyte)-1, converter.Read([0xFF]));
    }

    [Fact]
    public void WhenWriteSByteThenCorrectBytes()
    {
        var converter = new BinaryConverter<sbyte>();
        var buffer = new byte[1];
        converter.Write(buffer, -1);
        Assert.Equal(0xFF, buffer[0]);
    }

    // ---- short / ushort ----

    [Theory]
    [InlineData(Endian.Big)]
    [InlineData(Endian.Little)]
    public void WhenShortRoundTripThenSameValue(Endian endian)
    {
        var converter = new BinaryConverter<short>(endian);
        var buffer = new byte[BinaryConverter<short>.Size];
        converter.Write(buffer, short.MaxValue);
        Assert.Equal(short.MaxValue, converter.Read(buffer));
    }

    [Theory]
    [InlineData(Endian.Big)]
    [InlineData(Endian.Little)]
    public void WhenUShortRoundTripThenSameValue(Endian endian)
    {
        var converter = new BinaryConverter<ushort>(endian);
        var buffer = new byte[BinaryConverter<ushort>.Size];
        converter.Write(buffer, ushort.MaxValue);
        Assert.Equal(ushort.MaxValue, converter.Read(buffer));
    }

    // ---- uint ----

    [Theory]
    [InlineData(Endian.Big)]
    [InlineData(Endian.Little)]
    public void WhenUIntRoundTripThenSameValue(Endian endian)
    {
        var converter = new BinaryConverter<uint>(endian);
        var buffer = new byte[BinaryConverter<uint>.Size];
        converter.Write(buffer, uint.MaxValue);
        Assert.Equal(uint.MaxValue, converter.Read(buffer));
    }

    // ---- ulong ----

    [Theory]
    [InlineData(Endian.Big)]
    [InlineData(Endian.Little)]
    public void WhenULongRoundTripThenSameValue(Endian endian)
    {
        var converter = new BinaryConverter<ulong>(endian);
        var buffer = new byte[BinaryConverter<ulong>.Size];
        converter.Write(buffer, ulong.MaxValue);
        Assert.Equal(ulong.MaxValue, converter.Read(buffer));
    }

    // ---- float ----

    [Theory]
    [InlineData(Endian.Big)]
    [InlineData(Endian.Little)]
    public void WhenFloatRoundTripThenSameValue(Endian endian)
    {
        var converter = new BinaryConverter<float>(endian);
        var buffer = new byte[BinaryConverter<float>.Size];
        converter.Write(buffer, 3.14f);
        Assert.Equal(3.14f, converter.Read(buffer));
    }

    // ---- double ----

    [Theory]
    [InlineData(Endian.Big)]
    [InlineData(Endian.Little)]
    public void WhenDoubleRoundTripThenSameValue(Endian endian)
    {
        var converter = new BinaryConverter<double>(endian);
        var buffer = new byte[BinaryConverter<double>.Size];
        converter.Write(buffer, Math.PI);
        Assert.Equal(Math.PI, converter.Read(buffer));
    }

    // ---- decimal ----

    [Theory]
    [InlineData(Endian.Big)]
    [InlineData(Endian.Little)]
    public void WhenDecimalRoundTripThenSameValue(Endian endian)
    {
        var converter = new BinaryConverter<decimal>(endian);
        var buffer = new byte[BinaryConverter<decimal>.Size];
        converter.Write(buffer, 1234567890.123456789m);
        Assert.Equal(1234567890.123456789m, converter.Read(buffer));
    }

    // ---- Size constants ----

    [Fact]
    public void WhenSizeIsCorrectForShort() => Assert.Equal(2, BinaryConverter<short>.Size);

    [Fact]
    public void WhenSizeIsCorrectForFloat() => Assert.Equal(4, BinaryConverter<float>.Size);

    [Fact]
    public void WhenSizeIsCorrectForDouble() => Assert.Equal(8, BinaryConverter<double>.Size);

    [Fact]
    public void WhenSizeIsCorrectForDecimal() => Assert.Equal(16, BinaryConverter<decimal>.Size);
}
