namespace Smart.IO.ByteMapper.Converters;

using Smart.IO.ByteMapper.Mock;

public sealed class BigEndianLongBinaryConverterTest
{
    private const int Offset = 1;

    private const long Value = 1L;

    private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, [0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01]);

    private readonly BigEndianLongBinaryConverter converter = BigEndianLongBinaryConverter.Default;

    [Fact]
    public void ReadToBigEndianLongBinary()
    {
        Assert.Equal(Value, (long)converter.Read(ValueBytes, Offset));
    }

    [Fact]
    public void WriteBigEndianLongBinaryToBuffer()
    {
        var buffer = new byte[8 + Offset];
        converter.Write(buffer, Offset, Value);

        Assert.Equal(ValueBytes, buffer);
    }
}

public sealed class LittleEndianLongBinaryConverterTest
{
    private const int Offset = 1;

    private const long Value = 1L;

    private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, [0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00]);

    private readonly LittleEndianLongBinaryConverter converter = LittleEndianLongBinaryConverter.Default;

    [Fact]
    public void ReadToLittleEndianLongBinary()
    {
        Assert.Equal(Value, (long)converter.Read(ValueBytes, Offset));
    }

    [Fact]
    public void WriteLittleEndianLongBinaryToBuffer()
    {
        var buffer = new byte[8 + Offset];
        converter.Write(buffer, Offset, Value);

        Assert.Equal(ValueBytes, buffer);
    }
}
