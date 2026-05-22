// ReSharper disable RedundantArgumentDefaultValue
namespace Smart.IO.ByteMapper.Converters;

public class BooleanConverterTests
{
    [Fact]
    public void WhenReadTrueValueThenTrue()
    {
        var converter = new BooleanConverter(0x31, 0x30, 0x20);
        Assert.Equal(true, converter.Read([0x31]));
    }

    [Fact]
    public void WhenReadFalseValueThenFalse()
    {
        var converter = new BooleanConverter(0x31, 0x30, 0x20);
        Assert.Equal(false, converter.Read([0x30]));
    }

    [Fact]
    public void WhenReadNullValueThenNull()
    {
        var converter = new BooleanConverter(0x31, 0x30, 0x20);
        Assert.Null(converter.Read([0x20]));
    }

    [Fact]
    public void WhenWriteTrueThenCorrectByte()
    {
        var converter = new BooleanConverter(0x31, 0x30, 0x20);
        var buffer = new byte[1];
        converter.Write(buffer, true);
        Assert.Equal(0x31, buffer[0]);
    }

    [Fact]
    public void WhenWriteFalseThenCorrectByte()
    {
        var converter = new BooleanConverter(0x31, 0x30, 0x20);
        var buffer = new byte[1];
        converter.Write(buffer, false);
        Assert.Equal(0x30, buffer[0]);
    }

    [Fact]
    public void WhenWriteNullThenNullByte()
    {
        var converter = new BooleanConverter(0x31, 0x30, 0x20);
        var buffer = new byte[1];
        converter.Write(buffer, null);
        Assert.Equal(0x20, buffer[0]);
    }
}
