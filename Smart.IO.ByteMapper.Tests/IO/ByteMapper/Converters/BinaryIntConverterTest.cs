namespace Smart.IO.ByteMapper.Converters;

using Smart.IO.ByteMapper.Mock;

public sealed class BinaryIntConverterTest
{
    private const int Offset = 1;

    private const int Value = 1;

    private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, [0x00, 0x00, 0x00, 0x01]);

    private readonly BigEndianIntBinaryConverter converter = BigEndianIntBinaryConverter.Default;

    [Fact]
    public void ReadToBigEndianIntBinary()
    {
        Assert.Equal(Value, (int)converter.Read(ValueBytes, Offset));
    }

    [Fact]
    public void WriteBigEndianIntBinaryToBuffer()
    {
        var buffer = new byte[4 + Offset];
        converter.Write(buffer, Offset, Value);

        Assert.Equal(ValueBytes, buffer);
    }
}

public sealed class LittleEndianIntBinaryConverterTest
{
    private const int Offset = 1;

    private const int Value = 1;

    private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, [0x01, 0x00, 0x00, 0x00]);

    private readonly LittleEndianIntBinaryConverter converter = LittleEndianIntBinaryConverter.Default;

    [Fact]
    public void ReadToLittleEndianIntBinary()
    {
        Assert.Equal(Value, (int)converter.Read(ValueBytes, Offset));
    }

    [Fact]
    public void WriteLittleEndianIntBinaryToBuffer()
    {
        var buffer = new byte[4 + Offset];
        converter.Write(buffer, Offset, Value);

        Assert.Equal(ValueBytes, buffer);
    }
}
