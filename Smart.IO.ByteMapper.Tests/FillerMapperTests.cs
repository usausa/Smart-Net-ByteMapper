namespace Smart.IO.ByteMapper;

using System;

// ---- Gap auto-fill (NullFiller / AutoFiller) ----
// Layout: Head(0,4) + gap(4,4) + Tail(8,4) = 12 bytes

[Map(12, UseDelimiter = false, NullFiller = 0x2A)]
internal sealed class GapFillRecord
{
    [MapText(0, 4)]
    public string Head { get; set; } = default!;

    [MapText(8, 4)]
    public string Tail { get; set; } = default!;
}

[Map(12, UseDelimiter = false, NullFiller = 0x2A, AutoFiller = false)]
internal sealed class GapNoAutoFillRecord
{
    [MapText(0, 4)]
    public string Head { get; set; } = default!;

    [MapText(8, 4)]
    public string Tail { get; set; } = default!;
}

[Map(12, UseDelimiter = false)]
internal sealed class GapDefaultRecord
{
    [MapText(0, 4)]
    public string Head { get; set; } = default!;

    [MapText(8, 4)]
    public string Tail { get; set; } = default!;
}

internal static partial class GapFillMappers
{
    [ByteWriter]
    public static partial void Write(Span<byte> buffer, GapFillRecord source);

    [ByteWriter]
    public static partial void WriteNoAutoFill(Span<byte> buffer, GapNoAutoFillRecord source);

    [ByteWriter]
    public static partial void WriteDefault(Span<byte> buffer, GapDefaultRecord source);
}

public class FillerMapperTests
{
    [Fact]
    public void WhenNullFillerSetThenGapIsFilledOnWrite()
    {
        var buffer = new byte[12];
        GapFillMappers.Write(buffer, new GapFillRecord { Head = "HEAD", Tail = "TAIL" });

        Assert.Equal("HEAD****TAIL"u8.ToArray(), buffer);
    }

    [Fact]
    public void WhenAutoFillerFalseThenGapIsLeftUntouched()
    {
        var buffer = new byte[12];
        buffer.AsSpan().Fill(0xFF);
        GapFillMappers.WriteNoAutoFill(buffer, new GapNoAutoFillRecord { Head = "HEAD", Tail = "TAIL" });

        Assert.Equal(0xFF, buffer[4]);
        Assert.Equal(0xFF, buffer[7]);
    }

    [Fact]
    public void WhenNullFillerNotSetThenGapIsLeftUntouched()
    {
        var buffer = new byte[12];
        buffer.AsSpan().Fill(0xFF);
        GapFillMappers.WriteDefault(buffer, new GapDefaultRecord { Head = "HEAD", Tail = "TAIL" });

        Assert.Equal(0xFF, buffer[4]);
        Assert.Equal(0xFF, buffer[7]);
    }
}
