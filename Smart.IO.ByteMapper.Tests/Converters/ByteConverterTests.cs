namespace Smart.IO.ByteMapper.Converters;

using Xunit;

public class ByteConverterTests
{
    [Theory]
    [InlineData(0x00)]
    [InlineData(0xFF)]
    [InlineData(0x41)]
    public void WhenReadThenCorrectValue(byte value)
    {
        Assert.Equal(value, ByteConverter.Read([value]));
    }

    [Theory]
    [InlineData(0x00)]
    [InlineData(0xFF)]
    [InlineData(0x41)]
    public void WhenWriteThenCorrectByte(byte value)
    {
        var buffer = new byte[1];
        ByteConverter.Write(buffer, value);
        Assert.Equal(value, buffer[0]);
    }

    [Fact]
    public void WhenSizeIsOne() => Assert.Equal(1, ByteConverter.Size);

    [Fact]
    public void WhenRoundTripThenSameValue()
    {
        var buffer = new byte[1];
        ByteConverter.Write(buffer, 0xAB);
        Assert.Equal(0xAB, ByteConverter.Read(buffer));
    }
}
