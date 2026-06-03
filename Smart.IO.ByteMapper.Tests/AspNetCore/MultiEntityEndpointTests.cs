namespace Smart.IO.ByteMapper.AspNetCore;

using System;

// Two distinct entities declared in one [ByteMapperEndpoint] class. Verifies the model builder pairs
// reader/writer by (entity, profile) — keeping both entities (no silent drop) and pairing the right
// reader with the right writer — and disambiguates factory names by entity short name.

[Map(4, UseDelimiter = false)]
internal sealed class MultiEntityA
{
    [MapBinary<int>(0)]
    public int Id { get; set; }
}

#pragma warning disable CA1812
[Map(2, UseDelimiter = false)]
internal sealed class MultiEntityB
{
    [MapBinary<short>(0)]
    public short Code { get; set; }
}
#pragma warning restore CA1812

[ByteMapperEndpoint]
internal static partial class MultiEntityEndpoint
{
    [ByteReader]
    public static partial void ReadA(ReadOnlySpan<byte> src, MultiEntityA target);

    [ByteWriter]
    public static partial void WriteA(Span<byte> dst, MultiEntityA source);

    [ByteReader]
    public static partial void ReadB(ReadOnlySpan<byte> src, MultiEntityB target);

    [ByteWriter]
    public static partial void WriteB(Span<byte> dst, MultiEntityB source);
}

public class MultiEntityEndpointTests
{
    [Fact]
    public void WhenTwoEntitiesThenEntityASizeIsCorrect()
    {
        var binding = MultiEntityEndpoint.CreateByteMapperBinding_MultiEntityA();

        Assert.Equal(4, binding.Size);
    }

    [Fact]
    public void WhenTwoEntitiesThenEntityBSizeIsCorrect()
    {
        var binding = MultiEntityEndpoint.CreateByteMapperBinding_MultiEntityB();

        Assert.Equal(2, binding.Size);
    }

    [Fact]
    public void WhenEntityARoundTripThenIdPreserved()
    {
        var binding = MultiEntityEndpoint.CreateByteMapperBinding_MultiEntityA();
        var buffer = new byte[4];
        binding.Write(new MultiEntityA { Id = 0x01020304 }, buffer);
        var result = new MultiEntityA();
        binding.Read(buffer, result);

        Assert.Equal(0x01020304, result.Id);
    }

    [Fact]
    public void WhenEntityBRoundTripThenCodePreserved()
    {
        var binding = MultiEntityEndpoint.CreateByteMapperBinding_MultiEntityB();
        var buffer = new byte[2];
        binding.Write(new MultiEntityB { Code = 0x0102 }, buffer);
        var result = new MultiEntityB();
        binding.Read(buffer, result);

        Assert.Equal(0x0102, result.Code);
    }

    [Fact]
    public void WhenTwoEntitiesThenBindingsAreDistinct()
    {
        var a = MultiEntityEndpoint.CreateByteMapperBinding_MultiEntityA();
        var b = MultiEntityEndpoint.CreateByteMapperBinding_MultiEntityB();

        Assert.NotEqual(a.Size, b.Size);
    }
}
