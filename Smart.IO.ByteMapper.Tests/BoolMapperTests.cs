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
    public string Note { get; set; } = default!;
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

// ---- Non-nullable bool: the reader maps an unknown byte to false via GetValueOrDefault ----

[Map(2, UseDelimiter = false)]
internal sealed class BoolStrictRecord
{
    [MapBoolean(0)]
    public bool Flag { get; set; }

    [MapBoolean(1, TrueValue = 0x59, FalseValue = 0x4E)]
    public bool YesNo { get; set; }
}

internal static partial class BoolStrictMappers
{
    [ByteReader]
    public static partial void Read(ReadOnlySpan<byte> buffer, BoolStrictRecord target);

    [ByteWriter]
    public static partial void Write(Span<byte> buffer, BoolStrictRecord source);
}

public class BoolStrictMapperTests
{
    [Theory]
    [InlineData(true, 0x31)]
    [InlineData(false, 0x30)]
    public void WhenWriteFlagThenByteIsWritten(bool flag, byte expected)
    {
        var buffer = new byte[2];
        BoolStrictMappers.Write(buffer, new BoolStrictRecord { Flag = flag });

        Assert.Equal(expected, buffer[0]);
    }

    [Theory]
    [InlineData(0x31, true)]
    [InlineData(0x30, false)]
    public void WhenReadFlagByteThenFlagIsMapped(byte value, bool expected)
    {
        var buffer = new byte[] { value, 0x4E };
        var record = new BoolStrictRecord();
        BoolStrictMappers.Read(buffer, record);

        Assert.Equal(expected, record.Flag);
    }

    [Fact]
    public void WhenReadUnknownByteThenFlagIsFalse()
    {
        // A byte that is neither TrueValue nor FalseValue maps to null in the converter;
        // for a non-nullable property it becomes false. (0x20 unknown, 0x59 'Y')
        var buffer = " Y"u8.ToArray();
        var record = new BoolStrictRecord { Flag = true };
        BoolStrictMappers.Read(buffer, record);

        Assert.False(record.Flag);
        Assert.True(record.YesNo);
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(false, false)]
    [InlineData(true, false)]
    public void WhenRoundTripThenFlagsArePreserved(bool flag, bool yesNo)
    {
        var original = new BoolStrictRecord { Flag = flag, YesNo = yesNo };
        var buffer = new byte[2];
        BoolStrictMappers.Write(buffer, original);

        var read = new BoolStrictRecord();
        BoolStrictMappers.Read(buffer, read);

        Assert.Equal(flag, read.Flag);
        Assert.Equal(yesNo, read.YesNo);
    }
}
