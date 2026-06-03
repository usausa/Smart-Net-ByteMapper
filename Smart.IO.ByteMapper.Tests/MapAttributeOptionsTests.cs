// ReSharper disable UseUtf8StringLiteral
#pragma warning disable IDE0230
namespace Smart.IO.ByteMapper;

using System;

// ── Delimiter tests ───────────────────────────────────────────────────────────
// Layout: [Id(0,4)] [Name(4,6)] [CRLF(10,2)] = 12 bytes
// UseDelimiter=true (default) + Delimiter writes 0x0D 0x0A at bytes 10-11

[Map(12, Delimiter = new byte[] { 0x0D, 0x0A })]
internal sealed class CrlfRecord
{
    [MapBinary<int>(0)]
    public int Id { get; set; }

    [MapText(4, 6)]
    public string Name { get; set; } = default!;
}

internal static partial class CrlfMappers
{
    [ByteReader]
    public static partial void Read(ReadOnlySpan<byte> buffer, CrlfRecord target);

    [ByteWriter]
    public static partial void Write(Span<byte> buffer, CrlfRecord source);

    [ByteWriter]
    public static partial byte[] WriteAlloc(CrlfRecord source);
}

// ── UseDelimiter = false: no CRLF written ─────────────────────────────────────

[Map(12, UseDelimiter = false)]
internal sealed class NoCrlfRecord
{
    [MapBinary<int>(0)]
    public int Id { get; set; }

    [MapText(4, 6)]
    public string Name { get; set; } = default!;
}

internal static partial class NoCrlfMappers
{
    [ByteWriter]
    public static partial void Write(Span<byte> buffer, NoCrlfRecord source);

    [ByteWriter]
    public static partial byte[] WriteAlloc(NoCrlfRecord source);
}

// ── Delimiter write tests ─────────────────────────────────────────────────────

public class DelimiterWriteTests
{
    [Fact]
    public void WhenDelimiterSetThenCrIsWrittenAtEnd()
    {
        var record = new CrlfRecord { Id = 1, Name = "AB" };
        var buffer = new byte[12];
        CrlfMappers.Write(buffer, record);

        Assert.Equal(0x0D, buffer[10]);
    }

    [Fact]
    public void WhenDelimiterSetThenLfIsWrittenAtEnd()
    {
        var record = new CrlfRecord { Id = 1, Name = "AB" };
        var buffer = new byte[12];
        CrlfMappers.Write(buffer, record);

        Assert.Equal(0x0A, buffer[11]);
    }

    [Fact]
    public void WhenDelimiterSetThenIdIsCorrect()
    {
        var record = new CrlfRecord { Id = 0x00000064, Name = "HI" };
        var buffer = new byte[12];
        CrlfMappers.Write(buffer, record);

        Assert.Equal(new byte[] { 0x00, 0x00, 0x00, 0x64 }, buffer[..4]);
    }

    [Fact]
    public void WhenAllocWriteThenSizeIsMapSize()
    {
        var buffer = CrlfMappers.WriteAlloc(new CrlfRecord { Id = 1, Name = "X" });

        Assert.Equal(12, buffer.Length);
    }

    [Fact]
    public void WhenAllocWriteThenDelimiterBytesAreCrlf()
    {
        var buffer = CrlfMappers.WriteAlloc(new CrlfRecord { Id = 1, Name = "X" });

        Assert.Equal(0x0D, buffer[10]);
        Assert.Equal(0x0A, buffer[11]);
    }
}

// ── Delimiter read tests ──────────────────────────────────────────────────────

public class DelimiterReadTests
{
    [Fact]
    public void WhenReadBufferWithCrlfThenIdIsCorrect()
    {
        var buffer = CrlfMappers.WriteAlloc(new CrlfRecord { Id = 42, Name = "HELLO" });
        var result = new CrlfRecord();
        CrlfMappers.Read(buffer, result);

        Assert.Equal(42, result.Id);
    }

    [Fact]
    public void WhenReadBufferWithCrlfThenNameIsCorrect()
    {
        var buffer = CrlfMappers.WriteAlloc(new CrlfRecord { Id = 1, Name = "HELLO" });
        var result = new CrlfRecord();
        CrlfMappers.Read(buffer, result);

        Assert.Equal("HELLO", result.Name);
    }

    [Fact]
    public void WhenRoundTripWithCrlfThenAllFieldsPreserved()
    {
        var original = new CrlfRecord { Id = 9999, Name = "WORLD!" };
        var buffer = CrlfMappers.WriteAlloc(original);
        var read = new CrlfRecord();
        CrlfMappers.Read(buffer, read);

        Assert.Equal(original.Id, read.Id);
        Assert.Equal(original.Name, read.Name);
    }
}

// ── UseDelimiter = false tests ────────────────────────────────────────────────

public class UseDelimiterFalseTests
{
    [Fact]
    public void WhenUseDelimiterFalseThenEndBytesAreNotCrlf()
    {
        var buffer = NoCrlfMappers.WriteAlloc(new NoCrlfRecord { Id = 1, Name = "AB" });

        Assert.NotEqual(0x0D, buffer[10]);
        Assert.NotEqual(0x0A, buffer[11]);
    }

    [Fact]
    public void WhenUseDelimiterFalseThenEndBytesAreZero()
    {
        var buffer = NoCrlfMappers.WriteAlloc(new NoCrlfRecord { Id = 1, Name = "AB" });

        Assert.Equal(0x00, buffer[10]);
        Assert.Equal(0x00, buffer[11]);
    }

    [Fact]
    public void WhenUseDelimiterFalseThenSizeIsStillMapSize()
    {
        var buffer = NoCrlfMappers.WriteAlloc(new NoCrlfRecord { Id = 1, Name = "X" });

        Assert.Equal(12, buffer.Length);
    }
}
