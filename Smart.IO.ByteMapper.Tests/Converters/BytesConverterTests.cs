namespace Smart.IO.ByteMapper.Converters;

public class BytesConverterTests
{
    [Fact]
    public void WhenReadThenReturnsCorrectBytes()
    {
        var converter = new BytesConverter(4, 0x00);
        var buffer = new byte[] { 0x01, 0x02, 0x03, 0x04 };
        Assert.Equal(new byte[] { 0x01, 0x02, 0x03, 0x04 }, converter.Read(buffer));
    }

    [Fact]
    public void WhenWriteExactLengthThenCopied()
    {
        var converter = new BytesConverter(4, 0x00);
        var buffer = new byte[4];
        converter.Write(buffer, [0x0A, 0x0B, 0x0C, 0x0D]);
        Assert.Equal(new byte[] { 0x0A, 0x0B, 0x0C, 0x0D }, buffer);
    }

    [Fact]
    public void WhenWriteNullThenFillerFilled()
    {
        var converter = new BytesConverter(4, 0xFF);
        var buffer = new byte[4];
        converter.Write(buffer, null!);
        Assert.All(buffer, b => Assert.Equal(0xFF, b));
    }

    [Fact]
    public void WhenWriteEmptyArrayThenFillerFilled()
    {
        var converter = new BytesConverter(4, 0xAA);
        var buffer = new byte[4];
        converter.Write(buffer, []);
        Assert.All(buffer, b => Assert.Equal(0xAA, b));
    }

    [Fact]
    public void WhenWriteShorterValueThenPaddedRight()
    {
        var converter = new BytesConverter(4, 0x00);
        var buffer = new byte[4];
        converter.Write(buffer, [0x01, 0x02]);
        Assert.Equal(new byte[] { 0x01, 0x02, 0x00, 0x00 }, buffer);
    }

    [Fact]
    public void WhenWriteLongerValueThenTruncated()
    {
        var converter = new BytesConverter(3, 0x00);
        var buffer = new byte[3];
        converter.Write(buffer, [0x01, 0x02, 0x03, 0x04, 0x05]);
        Assert.Equal(new byte[] { 0x01, 0x02, 0x03 }, buffer);
    }

    [Fact]
    public void WhenSizeMatchesLength()
    {
        var converter = new BytesConverter(6, 0x00);
        Assert.Equal(6, converter.Size);
    }
}
