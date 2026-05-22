namespace Smart.IO.ByteMapper;

using System;
using System.Text;

// ---- Target type ----

[Map(36, UseDelimiter = false)]
internal sealed class SimpleRecord
{
    [MapBinary<int>(0)]
    public int Id { get; set; }

    [MapText(4, 32)]
    public string Name { get; set; } = string.Empty;
}

// ---- Mapper ----

internal static partial class SimpleMappers
{
    [ByteReader]
    public static partial void Read(ReadOnlySpan<byte> buffer, SimpleRecord target);

    [ByteWriter]
    public static partial void Write(SimpleRecord source, Span<byte> buffer);

    [ByteReader]
    public static partial SimpleRecord ReadNew(ReadOnlySpan<byte> buffer);

    [ByteWriter]
    public static partial byte[] WriteAlloc(SimpleRecord source);
}

// ---- Tests ----

public class SimpleMapperWriteTests
{
    [Fact]
    public void WhenWriteSimpleRecordThenIdIsBigEndianAtOffset0()
    {
        var record = new SimpleRecord { Id = 1, Name = "TEST" };
        var buffer = new byte[36];
        SimpleMappers.Write(record, buffer);

        Assert.Equal(new byte[] { 0x00, 0x00, 0x00, 0x01 }, buffer[..4]);
    }

    [Fact]
    public void WhenWriteSimpleRecordThenNameIsAsciiAtOffset4()
    {
        var record = new SimpleRecord { Id = 1, Name = "TEST" };
        var buffer = new byte[36];
        SimpleMappers.Write(record, buffer);

        Assert.Equal("TEST", Encoding.ASCII.GetString(buffer[4..8]));
    }

    [Fact]
    public void WhenWriteSimpleRecordThenNameIsRightPaddedWithSpace()
    {
        var record = new SimpleRecord { Id = 1, Name = "TEST" };
        var buffer = new byte[36];
        SimpleMappers.Write(record, buffer);

        Assert.Equal(0x20, buffer[8]);
    }

    [Fact]
    public void WhenWriteZeroIdThenAllIdBytesAreZero()
    {
        var record = new SimpleRecord { Id = 0, Name = string.Empty };
        var buffer = new byte[36];
        SimpleMappers.Write(record, buffer);

        Assert.Equal(new byte[] { 0x00, 0x00, 0x00, 0x00 }, buffer[..4]);
    }

    [Fact]
    public void WhenWriteNegativeIdThenCorrectBigEndianBytes()
    {
        var record = new SimpleRecord { Id = -1, Name = string.Empty };
        var buffer = new byte[36];
        SimpleMappers.Write(record, buffer);

        Assert.Equal(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF }, buffer[..4]);
    }

    [Fact]
    public void WhenWriteMaxIntIdThenCorrectBigEndianBytes()
    {
        var record = new SimpleRecord { Id = int.MaxValue, Name = string.Empty };
        var buffer = new byte[36];
        SimpleMappers.Write(record, buffer);

        Assert.Equal(new byte[] { 0x7F, 0xFF, 0xFF, 0xFF }, buffer[..4]);
    }

    [Fact]
    public void WhenAllocWriterThenBytesMatchSpanWriter()
    {
        var record = new SimpleRecord { Id = 123, Name = "AllocTest" };
        var spanBuffer = new byte[36];
        SimpleMappers.Write(record, spanBuffer);

        var allocBuffer = SimpleMappers.WriteAlloc(record);

        Assert.Equal(spanBuffer, allocBuffer);
    }

    [Fact]
    public void WhenAllocWriterThenReturnedBufferIsExactSize()
    {
        var record = new SimpleRecord { Id = 1, Name = "A" };
        var buffer = SimpleMappers.WriteAlloc(record);

        Assert.Equal(36, buffer.Length);
    }
}

public class SimpleMapperReadTests
{
    [Fact]
    public void WhenReadSimpleRecordThenIdIsCorrect()
    {
        var buffer = BuildBuffer(42, "HELLO");
        var record = new SimpleRecord();
        SimpleMappers.Read(buffer, record);

        Assert.Equal(42, record.Id);
    }

    [Fact]
    public void WhenReadSimpleRecordThenNameIsTrimmed()
    {
        var buffer = BuildBuffer(1, "HELLO");
        var record = new SimpleRecord();
        SimpleMappers.Read(buffer, record);

        Assert.Equal("HELLO", record.Name);
    }

    [Fact]
    public void WhenReadRecordWithFullNameThenNameIsCorrect()
    {
        var buffer = BuildBuffer(1, new string('A', 32));
        var record = new SimpleRecord();
        SimpleMappers.Read(buffer, record);

        Assert.Equal(new string('A', 32), record.Name);
    }

    [Fact]
    public void WhenReadEmptyNameThenNameIsEmpty()
    {
        var buffer = BuildBuffer(1, string.Empty);
        var record = new SimpleRecord();
        SimpleMappers.Read(buffer, record);

        Assert.Equal(string.Empty, record.Name);
    }

    [Fact]
    public void WhenNewInstanceReaderThenReturnsNewObject()
    {
        var buffer = BuildBuffer(7, "NewInst");
        var result = SimpleMappers.ReadNew(buffer);

        Assert.NotNull(result);
    }

    [Fact]
    public void WhenNewInstanceReaderThenIdIsCorrect()
    {
        var buffer = BuildBuffer(7, "NewInst");
        var result = SimpleMappers.ReadNew(buffer);

        Assert.Equal(7, result.Id);
    }

    [Fact]
    public void WhenNewInstanceReaderThenNameIsCorrect()
    {
        var buffer = BuildBuffer(7, "NewInst");
        var result = SimpleMappers.ReadNew(buffer);

        Assert.Equal("NewInst", result.Name);
    }

    private static byte[] BuildBuffer(int id, string name)
    {
        var buffer = new byte[36];
        buffer[0] = (byte)(id >> 24);
        buffer[1] = (byte)(id >> 16);
        buffer[2] = (byte)(id >> 8);
        buffer[3] = (byte)id;
        var nameBytes = Encoding.ASCII.GetBytes(name);
        nameBytes.CopyTo(buffer, 4);
        buffer.AsSpan(4 + nameBytes.Length, 32 - nameBytes.Length).Fill(0x20);
        return buffer;
    }
}

public class SimpleMapperRoundTripTests
{
    [Fact]
    public void WhenRoundTripThenIdIsPreserved()
    {
        var original = new SimpleRecord { Id = 99999, Name = "RoundTrip" };
        var buffer = new byte[36];
        SimpleMappers.Write(original, buffer);
        var read = new SimpleRecord();
        SimpleMappers.Read(buffer, read);

        Assert.Equal(original.Id, read.Id);
    }

    [Fact]
    public void WhenRoundTripThenNameIsPreserved()
    {
        var original = new SimpleRecord { Id = 99999, Name = "RoundTrip" };
        var buffer = new byte[36];
        SimpleMappers.Write(original, buffer);
        var read = new SimpleRecord();
        SimpleMappers.Read(buffer, read);

        Assert.Equal(original.Name, read.Name);
    }

    [Fact]
    public void WhenRoundTripNegativeIdThenIdIsPreserved()
    {
        var original = new SimpleRecord { Id = -12345, Name = "Neg" };
        var buffer = SimpleMappers.WriteAlloc(original);
        var read = SimpleMappers.ReadNew(buffer);

        Assert.Equal(original.Id, read.Id);
    }

    [Fact]
    public void WhenRoundTripEmptyNameThenNameIsEmpty()
    {
        var original = new SimpleRecord { Id = 1, Name = string.Empty };
        var buffer = SimpleMappers.WriteAlloc(original);
        var read = SimpleMappers.ReadNew(buffer);

        Assert.Equal(string.Empty, read.Name);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(-1)]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    public void WhenRoundTripVariousIdsThenIdIsPreserved(int id)
    {
        var original = new SimpleRecord { Id = id, Name = "X" };
        var buffer = SimpleMappers.WriteAlloc(original);
        var read = SimpleMappers.ReadNew(buffer);

        Assert.Equal(id, read.Id);
    }
}
