namespace Smart.IO.ByteMapper;

using System;

// ---- Target type using [MapByte] ----

[Map(3, UseDelimiter = false)]
internal sealed class ByteRecord
{
    [MapByte(0)]
    public byte First { get; set; }

    [MapByte(1)]
    public byte Second { get; set; }

    [MapByte(2)]
    public byte Third { get; set; }
}

// ---- Mapper ----

internal static partial class ByteRecordMappers
{
    [ByteReader]
    public static partial void Read(ReadOnlySpan<byte> buffer, ByteRecord target);

    [ByteWriter]
    public static partial void Write(Span<byte> buffer, ByteRecord source);
}

// ---- Tests ----

public class MapByteMapperTests
{
    [Fact]
    public void WhenWriteThenEachByteIsAtItsOffset()
    {
        var record = new ByteRecord { First = 0x0A, Second = 0xBB, Third = 0xCC };
        var buffer = new byte[3];
        ByteRecordMappers.Write(buffer, record);

        Assert.Equal(new byte[] { 0x0A, 0xBB, 0xCC }, buffer);
    }

    [Fact]
    public void WhenReadThenEachByteIsParsed()
    {
        var buffer = new byte[] { 0x01, 0x02, 0x03 };
        var record = new ByteRecord();
        ByteRecordMappers.Read(buffer, record);

        Assert.Equal(0x01, record.First);
        Assert.Equal(0x02, record.Second);
        Assert.Equal(0x03, record.Third);
    }

    [Fact]
    public void WhenRoundTripThenValuesPreserved()
    {
        var original = new ByteRecord { First = 0xFF, Second = 0x00, Third = 0x7F };
        var buffer = new byte[3];
        ByteRecordMappers.Write(buffer, original);
        var read = new ByteRecord();
        ByteRecordMappers.Read(buffer, read);

        Assert.Equal(original.First, read.First);
        Assert.Equal(original.Second, read.Second);
        Assert.Equal(original.Third, read.Third);
    }
}
