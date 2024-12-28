namespace Smart.IO.ByteMapper.Converters;

using Smart.IO.ByteMapper.Mock;

public sealed class BigEndianDoubleBinaryConverterTest
{
    private const int Offset = 1;

    private const double Value = 2;

    // ReSharper disable UseUtf8StringLiteral
    private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, [0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00]);
    // ReSharper restore UseUtf8StringLiteral

    private readonly BigEndianDoubleBinaryConverter converter = BigEndianDoubleBinaryConverter.Default;

    [Fact]
    public void ReadToBigEndianDoubleBinary()
    {
        Assert.Equal(Value, (double)converter.Read(ValueBytes, Offset));
    }

    [Fact]
    public void WriteBigEndianDoubleBinaryToBuffer()
    {
        var buffer = new byte[8 + Offset];
        converter.Write(buffer, Offset, Value);

        Assert.Equal(ValueBytes, buffer);
    }
}

public sealed class LittleEndianDoubleBinaryConverterTest
{
    private const int Offset = 1;

    private const double Value = 2;

    // ReSharper disable UseUtf8StringLiteral
    private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, [0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x40]);
    // ReSharper restore UseUtf8StringLiteral

    private readonly LittleEndianDoubleBinaryConverter converter = LittleEndianDoubleBinaryConverter.Default;

    [Fact]
    public void ReadToLittleEndianDoubleBinary()
    {
        Assert.Equal(Value, (double)converter.Read(ValueBytes, Offset));
    }

    [Fact]
    public void WriteLittleEndianDoubleBinaryToBuffer()
    {
        var buffer = new byte[8 + Offset];
        converter.Write(buffer, Offset, Value);

        Assert.Equal(ValueBytes, buffer);
    }
}
