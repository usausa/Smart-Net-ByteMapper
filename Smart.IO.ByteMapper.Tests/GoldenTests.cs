namespace Smart.IO.ByteMapper;

using System;
using System.Text;

using Xunit;

// ---- Target types ----

[Map(36)]
internal sealed class SimpleRecord
{
    [MapBinary<int>(0)]
    public int Id { get; set; }

    [MapText(4, 32)]
    public string Name { get; set; } = string.Empty;
}

[Map(38, UseDelimiter = true)]
internal sealed class RecordWithDelimiter
{
    [MapBinary<int>(0)]
    public int Value { get; set; }

    [MapText(4, 32)]
    public string Label { get; set; } = string.Empty;
}

[Map(10)]
internal sealed class BoolRecord
{
    [MapBoolean(0)]
    public bool? Flag { get; set; }

    [MapText(1, 9)]
    public string Note { get; set; } = string.Empty;
}

// ---- Mapper ----

internal static partial class GoldenMappers
{
    [ByteReader]
    public static partial void ReadSimple(ReadOnlySpan<byte> buffer, SimpleRecord target);

    [ByteWriter]
    public static partial void WriteSimple(SimpleRecord source, Span<byte> buffer);

    [ByteReader]
    public static partial SimpleRecord ReadSimpleNew(ReadOnlySpan<byte> buffer);

    [ByteWriter]
    public static partial byte[] WriteSimpleAlloc(SimpleRecord source);

    [ByteReader]
    public static partial void ReadBool(ReadOnlySpan<byte> buffer, BoolRecord target);

    [ByteWriter]
    public static partial void WriteBool(BoolRecord source, Span<byte> buffer);
}

// ---- Tests ----

public class GoldenTests
{
    [Fact]
    public void WhenWriteSimpleRecordThenBytesAreCorrect()
    {
        var record = new SimpleRecord { Id = 1, Name = "TEST" };
        var buffer = new byte[36];
        GoldenMappers.WriteSimple(record, buffer);

        // Id = 1 big-endian at offset 0
        Assert.Equal(new byte[] { 0x00, 0x00, 0x00, 0x01 }, buffer[..4]);
        // Name = "TEST" padded with spaces at offset 4
        Assert.Equal("TEST", Encoding.ASCII.GetString(buffer[4..8]));
        Assert.Equal(0x20, buffer[8]); // first padding byte
    }

    [Fact]
    public void WhenReadSimpleRecordThenValuesAreCorrect()
    {
        var buffer = new byte[36];
        // Write Id = 42 big-endian
        buffer[2] = 0x00;
        buffer[3] = 0x2A;
        // Write Name = "HELLO"
        Encoding.ASCII.GetBytes("HELLO").CopyTo(buffer, 4);
        // Fill remaining name bytes with space
        for (var i = 9; i < 36; i++) buffer[i] = 0x20;

        var record = new SimpleRecord();
        GoldenMappers.ReadSimple(buffer, record);

        Assert.Equal(42, record.Id);
        Assert.Equal("HELLO", record.Name);
    }

    [Fact]
    public void WhenRoundTripSimpleRecordThenValuesPreserved()
    {
        var original = new SimpleRecord { Id = 99999, Name = "RoundTrip" };
        var buffer = new byte[36];
        GoldenMappers.WriteSimple(original, buffer);

        var read = new SimpleRecord();
        GoldenMappers.ReadSimple(buffer, read);

        Assert.Equal(original.Id, read.Id);
        Assert.Equal(original.Name, read.Name);
    }

    [Fact]
    public void WhenNewInstanceReaderThenReturnsCorrectRecord()
    {
        var buffer = new byte[36];
        var original = new SimpleRecord { Id = 7, Name = "NewInst" };
        GoldenMappers.WriteSimple(original, buffer);

        var result = GoldenMappers.ReadSimpleNew(buffer);
        Assert.Equal(7, result.Id);
        Assert.Equal("NewInst", result.Name);
    }

    [Fact]
    public void WhenAllocWriterThenBytesMatchSpanWriter()
    {
        var record = new SimpleRecord { Id = 123, Name = "AllocTest" };
        var spanBuffer = new byte[36];
        GoldenMappers.WriteSimple(record, spanBuffer);

        var allocBuffer = GoldenMappers.WriteSimpleAlloc(record);
        Assert.Equal(spanBuffer, allocBuffer);
    }

    [Fact]
    public void WhenBufferTooSmallThenThrows()
    {
        var record = new SimpleRecord { Id = 1, Name = "X" };
        Assert.Throws<ByteMapperException>(() =>
        {
            var buffer = new byte[10]; // too small
            GoldenMappers.WriteSimple(record, buffer);
        });
    }

    [Fact]
    public void WhenReadBoolTrueThenFlagIsTrue()
    {
        var buffer = new byte[10];
        buffer[0] = 0x31; // true
        Encoding.ASCII.GetBytes("NOTE     ").CopyTo(buffer, 1);
        var record = new BoolRecord();
        GoldenMappers.ReadBool(buffer, record);
        Assert.Equal(true, record.Flag);
    }

    [Fact]
    public void WhenWriteBoolFalseThenCorrectByte()
    {
        var record = new BoolRecord { Flag = false, Note = "X" };
        var buffer = new byte[10];
        GoldenMappers.WriteBool(record, buffer);
        Assert.Equal(0x30, buffer[0]);
    }

    [Fact]
    public void WhenWriteBoolNullThenNullByte()
    {
        var record = new BoolRecord { Flag = null, Note = "X" };
        var buffer = new byte[10];
        GoldenMappers.WriteBool(record, buffer);
        Assert.Equal(0x20, buffer[0]);
    }
}
