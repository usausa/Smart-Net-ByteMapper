namespace Smart.IO.ByteMapper.Converters;

using Smart.IO.ByteMapper.Mock;

public sealed class NullableBooleanConverterTest
{
    private const int Offset = 1;

    private const byte TrueByte = 0x31;

    private const byte FalseByte = 0x30;

    private const byte NullByte = 0x00;

    private static readonly byte[] TrueBytes = TestBytes.Offset(Offset, [TrueByte]);

    private static readonly byte[] FalseBytes = TestBytes.Offset(Offset, [FalseByte]);

    private static readonly byte[] NullBytes = TestBytes.Offset(Offset, [NullByte]);

    private readonly NullableBooleanConverter converter;

    public NullableBooleanConverterTest()
    {
        converter = new NullableBooleanConverter(TrueByte, FalseByte, NullByte);
    }

    //--------------------------------------------------------------------------------
    // bool
    //--------------------------------------------------------------------------------

    [Fact]
    public void ReadToNullableBoolean()
    {
        // True
        Assert.True((bool?)converter.Read(TrueBytes, Offset));

        // False
        Assert.False((bool?)converter.Read(FalseBytes, Offset));

        // Null
        Assert.Null(converter.Read(NullBytes, Offset));
    }

    [Fact]
    public void WriteNullableBooleanToBuffer()
    {
        var buffer = new byte[1 + Offset];

        // True
        converter.Write(buffer, Offset, true);
        Assert.Equal(TrueBytes, buffer);

        // False
        converter.Write(buffer, Offset, false);
        Assert.Equal(FalseBytes, buffer);

        // Null
        converter.Write(buffer, Offset, null);
        Assert.Equal(NullBytes, buffer);
    }
}
