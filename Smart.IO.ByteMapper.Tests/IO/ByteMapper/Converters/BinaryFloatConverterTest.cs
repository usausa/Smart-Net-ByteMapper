namespace Smart.IO.ByteMapper.Converters;

using Smart.IO.ByteMapper.Mock;

using Xunit;

public class BigEndianFloatBinaryConverterTest
{
    private const int Offset = 1;

    private const float Value = 2;

    private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, new byte[]
    {
        0x40, 0x00, 0x00, 0x00
    });

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

public class LittleEndianFloatBinaryConverterTest
{
    private const int Offset = 1;

    private const float Value = 2;

    private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, new byte[]
    {
        0x00, 0x00, 0x00, 0x40
    });

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
