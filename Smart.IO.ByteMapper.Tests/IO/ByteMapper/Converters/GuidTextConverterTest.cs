namespace Smart.IO.ByteMapper.Converters;

using System.Text;

using Smart.IO.ByteMapper.Mock;

public sealed class GuidTextConverterTest
{
    private const int Offset = 1;
    private const int Length = 36;

    private static readonly Guid Value = new("12345678-1234-1234-1234-123456789abc");
    private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, "12345678-1234-1234-1234-123456789abc"u8.ToArray());
    private static readonly byte[] EmptyBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes(string.Empty.PadRight(Length, ' ')));

    private static GuidTextConverter CreateConverter() =>
        new(Length, "D", Encoding.ASCII, 0x20);

    [Fact]
    public void ReadToGuid()
    {
        Assert.Equal(Value, (Guid)CreateConverter().Read(ValueBytes.AsSpan(Offset)));
    }

    [Fact]
    public void ReadEmptyReturnsDefault()
    {
        Assert.Equal(default, (Guid)CreateConverter().Read(EmptyBytes.AsSpan(Offset)));
    }

    [Fact]
    public void WriteGuidToBuffer()
    {
        var buffer = new byte[Length + Offset];
        CreateConverter().Write(buffer.AsSpan(Offset), Value);

        Assert.Equal(ValueBytes, buffer);
    }

    [Fact]
    public void WriteNullFillsFiller()
    {
        var buffer = new byte[Length + Offset];
        CreateConverter().Write(buffer.AsSpan(Offset), null);

        Assert.Equal(EmptyBytes, buffer);
    }
}

public sealed class NullableGuidTextConverterTest
{
    private const int Offset = 1;
    private const int Length = 36;

    private static readonly Guid GuidValue = new("12345678-1234-1234-1234-123456789abc");
    private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, "12345678-1234-1234-1234-123456789abc"u8.ToArray());
    private static readonly byte[] EmptyBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes(string.Empty.PadRight(Length, ' ')));

    private static NullableGuidTextConverter CreateConverter() =>
        new(Length, "D", Encoding.ASCII, 0x20);

    [Fact]
    public void ReadToNullableGuid()
    {
        Assert.Equal(GuidValue, (Guid?)CreateConverter().Read(ValueBytes.AsSpan(Offset)));
    }

    [Fact]
    public void ReadEmptyReturnsNull()
    {
        Assert.Null(CreateConverter().Read(EmptyBytes.AsSpan(Offset)));
    }

    [Fact]
    public void WriteNullableGuidToBuffer()
    {
        var buffer = new byte[Length + Offset];
        CreateConverter().Write(buffer.AsSpan(Offset), (Guid?)GuidValue);

        Assert.Equal(ValueBytes, buffer);
    }

    [Fact]
    public void WriteNullFillsFiller()
    {
        var buffer = new byte[Length + Offset];
        CreateConverter().Write(buffer.AsSpan(Offset), null);

        Assert.Equal(EmptyBytes, buffer);
    }
}
