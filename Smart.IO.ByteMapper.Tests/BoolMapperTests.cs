namespace Smart.IO.ByteMapper;

using System;
using System.Text;

// ---- Target type ----

[Map(10, UseDelimiter = false)]
internal sealed class BoolRecord
{
    [MapBoolean(0)]
    public bool? Flag { get; set; }

    [MapText(1, 9)]
    public string Note { get; set; } = string.Empty;
}

// ---- Mapper ----

internal static partial class BoolMappers
{
    [ByteReader]
    public static partial void Read(ReadOnlySpan<byte> buffer, BoolRecord target);

    [ByteWriter]
    public static partial void Write(Span<byte> buffer, BoolRecord source);

    [ByteReader]
    public static partial BoolRecord ReadNew(ReadOnlySpan<byte> buffer);

    [ByteWriter]
    public static partial byte[] WriteAlloc(BoolRecord source);
}

// ---- Tests ----

public class BoolMapperWriteTests
{
    [Fact]
    // ReSharper disable once InconsistentNaming
    public void WhenWriteTrueFlagThenFirstByteIs0x31()
    {
        var record = new BoolRecord { Flag = true, Note = "X" };
        var buffer = new byte[10];
        BoolMappers.Write(buffer, record);

        Assert.Equal(0x31, buffer[0]);
    }

    [Fact]
    // ReSharper disable once InconsistentNaming
    public void WhenWriteFalseFlagThenFirstByteIs0x30()
    {
        var record = new BoolRecord { Flag = false, Note = "X" };
        var buffer = new byte[10];
        BoolMappers.Write(buffer, record);

        Assert.Equal(0x30, buffer[0]);
    }

    [Fact]
    public void WhenWriteNullFlagThenFirstByteIsSpace()
    {
        var record = new BoolRecord { Flag = null, Note = "X" };
        var buffer = new byte[10];
        BoolMappers.Write(buffer, record);

        Assert.Equal(0x20, buffer[0]);
    }

    [Fact]
    public void WhenWriteNoteThenNoteIsAtOffset1()
    {
        var record = new BoolRecord { Flag = true, Note = "ABC" };
        var buffer = new byte[10];
        BoolMappers.Write(buffer, record);

        Assert.Equal("ABC", Encoding.ASCII.GetString(buffer[1..4]));
    }

    [Fact]
    public void WhenWriteNoteThenRemainingBytesArePaddedWithSpace()
    {
        var record = new BoolRecord { Flag = true, Note = "A" };
        var buffer = new byte[10];
        BoolMappers.Write(buffer, record);

        Assert.Equal(0x20, buffer[2]);
    }

    [Fact]
    public void WhenAllocWriterThenReturnedBufferIsExactSize()
    {
        var record = new BoolRecord { Flag = true, Note = "Z" };
        var buffer = BoolMappers.WriteAlloc(record);

        Assert.Equal(10, buffer.Length);
    }

    [Fact]
    public void WhenAllocWriterThenBytesMatchSpanWriter()
    {
        var record = new BoolRecord { Flag = false, Note = "NOTE" };
        var spanBuffer = new byte[10];
        BoolMappers.Write(spanBuffer, record);

        var allocBuffer = BoolMappers.WriteAlloc(record);

        Assert.Equal(spanBuffer, allocBuffer);
    }
}

public class BoolMapperReadTests
{
    [Fact]
    // ReSharper disable once InconsistentNaming
    public void WhenRead0x31ThenFlagIsTrue()
    {
        var buffer = BuildBuffer(0x31, "NOTE     ");
        var record = new BoolRecord();
        BoolMappers.Read(buffer, record);

        Assert.Equal(true, record.Flag);
    }

    [Fact]
    // ReSharper disable once InconsistentNaming
    public void WhenRead0x30ThenFlagIsFalse()
    {
        var buffer = BuildBuffer(0x30, "NOTE     ");
        var record = new BoolRecord();
        BoolMappers.Read(buffer, record);

        Assert.Equal(false, record.Flag);
    }

    [Fact]
    public void WhenReadSpaceThenFlagIsNull()
    {
        var buffer = BuildBuffer(0x20, "NOTE     ");
        var record = new BoolRecord();
        BoolMappers.Read(buffer, record);

        Assert.Null(record.Flag);
    }

    [Fact]
    public void WhenReadNoteThenNoteIsTrimmed()
    {
        var buffer = BuildBuffer(0x31, "ABC      ");
        var record = new BoolRecord();
        BoolMappers.Read(buffer, record);

        Assert.Equal("ABC", record.Note);
    }

    [Fact]
    public void WhenNewInstanceReaderThenFlagIsCorrect()
    {
        var buffer = BuildBuffer(0x31, "X        ");
        var result = BoolMappers.ReadNew(buffer);

        Assert.Equal(true, result.Flag);
    }

    [Fact]
    public void WhenNewInstanceReaderThenNoteIsCorrect()
    {
        var buffer = BuildBuffer(0x31, "HELLO    ");
        var result = BoolMappers.ReadNew(buffer);

        Assert.Equal("HELLO", result.Note);
    }

    private static byte[] BuildBuffer(byte flagByte, string note)
    {
        var buffer = new byte[10];
        buffer[0] = flagByte;
        Encoding.ASCII.GetBytes(note).CopyTo(buffer, 1);
        return buffer;
    }
}

public class BoolMapperRoundTripTests
{
    [Fact]
    public void WhenRoundTripTrueThenFlagIsPreserved()
    {
        var original = new BoolRecord { Flag = true, Note = "ROUND" };
        var buffer = BoolMappers.WriteAlloc(original);
        var read = BoolMappers.ReadNew(buffer);

        Assert.Equal(true, read.Flag);
    }

    [Fact]
    public void WhenRoundTripFalseThenFlagIsPreserved()
    {
        var original = new BoolRecord { Flag = false, Note = "TEST" };
        var buffer = BoolMappers.WriteAlloc(original);
        var read = BoolMappers.ReadNew(buffer);

        Assert.Equal(false, read.Flag);
    }

    [Fact]
    public void WhenRoundTripNullThenFlagIsNull()
    {
        var original = new BoolRecord { Flag = null, Note = "NULL" };
        var buffer = BoolMappers.WriteAlloc(original);
        var read = BoolMappers.ReadNew(buffer);

        Assert.Null(read.Flag);
    }

    [Fact]
    public void WhenRoundTripThenNoteIsPreserved()
    {
        var original = new BoolRecord { Flag = true, Note = "ABCDE" };
        var buffer = BoolMappers.WriteAlloc(original);
        var read = BoolMappers.ReadNew(buffer);

        Assert.Equal(original.Note, read.Note);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public void WhenRoundTripVariousFlagsThenFlagIsPreserved(bool? flag)
    {
        var original = new BoolRecord { Flag = flag, Note = "X" };
        var buffer = BoolMappers.WriteAlloc(original);
        var read = BoolMappers.ReadNew(buffer);

        Assert.Equal(flag, read.Flag);
    }
}
