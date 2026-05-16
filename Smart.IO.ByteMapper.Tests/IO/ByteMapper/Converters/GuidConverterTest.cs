namespace Smart.IO.ByteMapper.Converters;

using Smart.IO.ByteMapper.Mock;

public sealed class BigEndianGuidBinaryConverterTest
{
    private const int Offset = 1;

    // {12345678-1234-1234-1234-123456789ABC} in big-endian bytes
    private static readonly Guid Value = new("12345678-1234-1234-1234-123456789ABC");

    // Big-endian layout: Data1 BE, Data2 BE, Data3 BE, Data4 as-is
    private static readonly byte[] ValueBytes = TestBytes.Offset(
        Offset,
        [
            0x12, 0x34, 0x56, 0x78,  // Data1 big-endian
            0x12, 0x34,              // Data2 big-endian
            0x12, 0x34,              // Data3 big-endian
            0x12, 0x34,              // Data4[0-1]
            0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC   // Data4[2-7]
        ]);

    private readonly BigEndianGuidBinaryConverter converter = BigEndianGuidBinaryConverter.Default;

    [Fact]
    public void ReadToBigEndianGuidBinary()
    {
        Assert.Equal(Value, (Guid)converter.Read(ValueBytes.AsSpan(Offset)));
    }

    [Fact]
    public void WriteBigEndianGuidBinaryToBuffer()
    {
        var buffer = new byte[16 + Offset];
        converter.Write(buffer.AsSpan(Offset), Value);

        Assert.Equal(ValueBytes, buffer);
    }

    [Fact]
    public void WriteNullBigEndianGuidBinaryFillsZero()
    {
        var buffer = new byte[16 + Offset];
        converter.Write(buffer.AsSpan(Offset), null);

        Assert.Equal(new byte[16 + Offset], buffer);
    }
}

public sealed class LittleEndianGuidBinaryConverterTest
{
    private const int Offset = 1;

    private static readonly Guid Value = new("12345678-1234-1234-1234-123456789ABC");

    // Little-endian: .NET native Guid.TryWriteBytes layout
    private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, GetNativeBytes(Value));

    private static byte[] GetNativeBytes(Guid g)
    {
        var b = new byte[16];
        g.TryWriteBytes(b);
        return b;
    }

    private readonly LittleEndianGuidBinaryConverter converter = LittleEndianGuidBinaryConverter.Default;

    [Fact]
    public void ReadToLittleEndianGuidBinary()
    {
        Assert.Equal(Value, (Guid)converter.Read(ValueBytes.AsSpan(Offset)));
    }

    [Fact]
    public void WriteLittleEndianGuidBinaryToBuffer()
    {
        var buffer = new byte[16 + Offset];
        converter.Write(buffer.AsSpan(Offset), Value);

        Assert.Equal(ValueBytes, buffer);
    }

    [Fact]
    public void WriteNullLittleEndianGuidBinaryFillsZero()
    {
        var buffer = new byte[16 + Offset];
        converter.Write(buffer.AsSpan(Offset), null);

        Assert.Equal(new byte[16 + Offset], buffer);
    }
}

public sealed class LittleEndianNullableGuidBinaryConverterTest
{
    private const int Offset = 1;

    private static readonly Guid GuidValue = new("12345678-1234-1234-1234-123456789ABC");

    private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, GetNativeBytes(GuidValue));
    private static readonly byte[] NullBytes = TestBytes.Offset(Offset, new byte[16]);

    private static byte[] GetNativeBytes(Guid g)
    {
        var b = new byte[16];
        g.TryWriteBytes(b);
        return b;
    }

    private readonly LittleEndianNullableGuidBinaryConverter converter = LittleEndianNullableGuidBinaryConverter.Default;

    [Fact]
    public void ReadToNullableGuid()
    {
        Assert.Equal((Guid?)GuidValue, (Guid?)converter.Read(ValueBytes.AsSpan(Offset)));
    }

    [Fact]
    public void ReadNullBytesReturnsNull()
    {
        Assert.Null(converter.Read(NullBytes.AsSpan(Offset)));
    }

    [Fact]
    public void WriteNullFillsZero()
    {
        var buffer = new byte[16 + Offset];
        converter.Write(buffer.AsSpan(Offset), null);

        Assert.Equal(NullBytes, buffer);
    }
}
