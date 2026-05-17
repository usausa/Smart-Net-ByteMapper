namespace Smart.IO.ByteMapper.Converters;

using System.Text;

using Smart.IO.ByteMapper.Builders;
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

public sealed class GuidTextFastConverterTest
{
    private static readonly Guid Value = new("12345678-1234-1234-1234-123456789abc");

    private static IMapConverter CreateViaBuilder(int length, string format, Type type)
        => new GuidTextConverterBuilder
        {
            Length = length,
            Format = format,
            Encoding = Encoding.ASCII,
            Filler = 0x20
        }.CreateConverter(new MockBuilderContext(), type);

    [Fact]
    public void BuilderDispatchesFastPathFormatN()
    {
        var converter = CreateViaBuilder(32, "N", typeof(Guid));
        var buffer = new byte[32];
        converter.Write(buffer, Value);
        Assert.Equal("12345678123412341234123456789abc"u8.ToArray(), buffer);
        Assert.Equal(Value, converter.Read(buffer));
    }

    [Fact]
    public void BuilderDispatchesFastPathFormatD()
    {
        var converter = CreateViaBuilder(36, "D", typeof(Guid));
        var buffer = new byte[36];
        converter.Write(buffer, Value);
        Assert.Equal("12345678-1234-1234-1234-123456789abc"u8.ToArray(), buffer);
        Assert.Equal(Value, converter.Read(buffer));
    }

    [Fact]
    public void FastPathFillerWhenWidthLessThanLength()
    {
        // "D" width=36, length=40: 36 Guid bytes + 4 filler bytes
        var converter = CreateViaBuilder(40, "D", typeof(Guid));
        var buffer = new byte[40];
        converter.Write(buffer, Value);
        Assert.Equal("12345678-1234-1234-1234-123456789abc"u8.ToArray(), buffer[..36]);
        Assert.Equal("    "u8.ToArray(), buffer[36..]);
        Assert.Equal(Value, converter.Read(buffer));
    }

    [Fact]
    public void FastPathInvalidBytesReturnsDefault()
    {
        var converter = CreateViaBuilder(36, "D", typeof(Guid));
        var buffer = new byte[36];
        buffer.AsSpan().Fill((byte)'x');
        Assert.Equal(Guid.Empty, (Guid)converter.Read(buffer));
    }

    [Fact]
    public void NullableFastPathRoundtrip()
    {
        var converter = CreateViaBuilder(36, "D", typeof(Guid?));
        var buffer = new byte[36];
        converter.Write(buffer, (Guid?)Value);
        Assert.Equal(Value, converter.Read(buffer));
    }

    [Fact]
    public void NullableFastPathEmptyReturnsNull()
    {
        var converter = CreateViaBuilder(36, "D", typeof(Guid?));
        var buffer = new byte[36];
        buffer.AsSpan().Fill(0x20);
        Assert.Null(converter.Read(buffer));
    }

    [Fact]
    public void NullableFastPathWriteNull()
    {
        var converter = CreateViaBuilder(36, "D", typeof(Guid?));
        var buffer = new byte[36];
        converter.Write(buffer, null);
        Assert.Equal("                                    "u8.ToArray(), buffer);
    }
}
