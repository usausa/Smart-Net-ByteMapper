namespace Smart.IO.ByteMapper.Converters;

public class ByteConverterTests
{
    private readonly ByteConverter converter = new();

    [Theory]
    [InlineData(0x00)]
    [InlineData(0xFF)]
    [InlineData(0x41)]
    public void WhenReadThenCorrectValue(byte value)
    {
        Assert.Equal(value, converter.Read([value]));
    }

    [Theory]
    [InlineData(0x00)]
    [InlineData(0xFF)]
    [InlineData(0x41)]
    public void WhenWriteThenCorrectByte(byte value)
    {
        var buffer = new byte[1];
        converter.Write(buffer, value);
        Assert.Equal(value, buffer[0]);
    }

    [Fact]
    public void WhenSizeIsOne() => Assert.Equal(1, ByteConverter.Size);

    [Fact]
    public void WhenRoundTripThenSameValue()
    {
        var buffer = new byte[1];
        converter.Write(buffer, 0xAB);
        Assert.Equal(0xAB, converter.Read(buffer));
    }
}
