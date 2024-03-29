namespace Smart.IO.ByteMapper.Converters;

using Smart.IO.ByteMapper.Mock;

public sealed class ByteConverterTest
{
    private const int Offset = 1;

    private const byte Value = 0x01;

    private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, [Value]);

    private readonly ByteConverter converter = ByteConverter.Default;

    [Fact]
    public void ReadToByte()
    {
        Assert.Equal(Value, (byte)converter.Read(ValueBytes, Offset));
    }

    [Fact]
    public void WriteByteToBuffer()
    {
        var buffer = new byte[1 + Offset];
        converter.Write(buffer, Offset, Value);

        Assert.Equal(ValueBytes, buffer);
    }
}
