namespace Smart.IO.ByteMapper.Converters;

using Smart.IO.ByteMapper.Mock;

public sealed class BytesConverterTest
{
    private const int Offset = 1;

    private const int Length = 8;

    private static readonly byte[] Value = [0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08];

    private static readonly byte[] ShortValue = [0x01, 0x02, 0x03, 0x04];

    private static readonly byte[] OverflowValue = [0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09];

    private static readonly byte[] ValueBytes;

    private static readonly byte[] ShortValueBytes;

    private static readonly byte[] EmptyBytes;

    private readonly BytesConverter converter;

    static BytesConverterTest()
    {
        ValueBytes = TestBytes.Offset(Offset, Value);
        ShortValueBytes = TestBytes.Offset(Offset, ShortValue.Combine(new byte[Length - ShortValue.Length]));
        EmptyBytes = TestBytes.Offset(Offset, new byte[Length]);
    }

    public BytesConverterTest()
    {
        converter = new BytesConverter(Length, 0x00);
    }

    //--------------------------------------------------------------------------------
    // byte[]
    //--------------------------------------------------------------------------------

    [Fact]
    public void ReadToBytes()
    {
        Assert.Equal(Value, converter.Read(ValueBytes, Offset));
    }

    [Fact]
    public void WriteBytesToBuffer()
    {
        var buffer = new byte[Length + Offset];

        // Value
        converter.Write(buffer, Offset, Value);
        Assert.Equal(ValueBytes, buffer);

        // Null
        converter.Write(buffer, Offset, null);
        Assert.Equal(EmptyBytes, buffer);

        // Short
        converter.Write(buffer, Offset, ShortValue);
        Assert.Equal(ShortValueBytes, buffer);

        // Overflow
        converter.Write(buffer, Offset, OverflowValue);
        Assert.Equal(ValueBytes, buffer);
    }
}
