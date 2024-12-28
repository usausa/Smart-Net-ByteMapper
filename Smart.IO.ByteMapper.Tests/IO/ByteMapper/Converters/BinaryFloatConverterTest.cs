namespace Smart.IO.ByteMapper.Converters;

using Smart.IO.ByteMapper.Mock;

public sealed class BigEndianFloatBinaryConverterTest
{
    private const int Offset = 1;

    private const float Value = 2;

    // ReSharper disable UseUtf8StringLiteral
    private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, [0x40, 0x00, 0x00, 0x00]);
    // ReSharper restore UseUtf8StringLiteral

    private readonly BigEndianFloatBinaryConverter converter = BigEndianFloatBinaryConverter.Default;

    [Fact]
    public void ReadToBigEndianFloatBinary()
    {
        Assert.Equal(Value, (float)converter.Read(ValueBytes, Offset));
    }

    [Fact]
    public void WriteBigEndianFloatBinaryToBuffer()
    {
        var buffer = new byte[4 + Offset];
        converter.Write(buffer, Offset, Value);

        Assert.Equal(ValueBytes, buffer);
    }
}

public sealed class LittleEndianFloatBinaryConverterTest
{
    private const int Offset = 1;

    private const float Value = 2;

    // ReSharper disable UseUtf8StringLiteral
    private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, [0x00, 0x00, 0x00, 0x40]);
    // ReSharper restore UseUtf8StringLiteral

    private readonly LittleEndianFloatBinaryConverter converter = LittleEndianFloatBinaryConverter.Default;

    [Fact]
    public void ReadToLittleEndianFloatBinary()
    {
        Assert.Equal(Value, (float)converter.Read(ValueBytes, Offset));
    }

    [Fact]
    public void WriteLittleEndianFloatBinaryToBuffer()
    {
        var buffer = new byte[4 + Offset];
        converter.Write(buffer, Offset, Value);

        Assert.Equal(ValueBytes, buffer);
    }
}
