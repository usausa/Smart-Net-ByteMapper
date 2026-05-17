namespace Smart.IO.ByteMapper.Converters;

using System.Text;

using Smart.IO.ByteMapper.Builders;
using Smart.IO.ByteMapper.Mock;

public sealed class TimeOnlyTextConverterTest
{
    private const int Offset = 1;

    private const int Length = 6;

    private const string Format = "HHmmss";

    private static readonly TimeOnly Value = new(12, 34, 56);

    private static readonly byte[] EmptyBytes = TestBytes.Offset(Offset, "      "u8.ToArray());

    private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, "123456"u8.ToArray());

    private static readonly byte[] InvalidBytes = TestBytes.Offset(Offset, "xxxxxx"u8.ToArray());

    private readonly TimeOnlyTextConverter timeOnlyConverter;

    private readonly TimeOnlyTextConverter nullableTimeOnlyConverter;

    public TimeOnlyTextConverterTest()
    {
        timeOnlyConverter = CreateConverter(typeof(TimeOnly), Format);
        nullableTimeOnlyConverter = CreateConverter(typeof(TimeOnly?), Format);
    }

    private static TimeOnlyTextConverter CreateConverter(Type type, string format)
        => new(Length, format, Encoding.ASCII, 0x20, type);

    private static IMapConverter CreateViaBuilder(int length, string format, Type type)
        => new DateTimeTextConverterBuilder
        {
            Length = length,
            Format = format,
            Encoding = Encoding.ASCII,
            Filler = 0x20
        }.CreateConverter(new MockBuilderContext(), type);

    //--------------------------------------------------------------------------------
    // TimeOnly (BCL path)
    //--------------------------------------------------------------------------------

    [Fact]
    public void ReadToTimeOnly()
    {
        // Default
        Assert.Equal(default(TimeOnly), timeOnlyConverter.Read(EmptyBytes.AsSpan(Offset)));

        // Invalid
        Assert.Equal(default(TimeOnly), timeOnlyConverter.Read(InvalidBytes.AsSpan(Offset)));

        // Value
        Assert.Equal(Value, timeOnlyConverter.Read(ValueBytes.AsSpan(Offset)));
    }

    [Fact]
    public void WriteTimeOnlyToBuffer()
    {
        var buffer = new byte[Length + Offset];

        // Value
        timeOnlyConverter.Write(buffer.AsSpan(Offset), Value);
        Assert.Equal(ValueBytes, buffer);
    }

    //--------------------------------------------------------------------------------
    // TimeOnly? (BCL path)
    //--------------------------------------------------------------------------------

    [Fact]
    public void ReadToNullableTimeOnly()
    {
        // Null
        Assert.Null(nullableTimeOnlyConverter.Read(EmptyBytes.AsSpan(Offset)));

        // Invalid
        Assert.Null(nullableTimeOnlyConverter.Read(InvalidBytes.AsSpan(Offset)));

        // Value
        Assert.Equal(Value, nullableTimeOnlyConverter.Read(ValueBytes.AsSpan(Offset)));
    }

    [Fact]
    public void WriteNullTimeOnlyToBuffer()
    {
        var buffer = new byte[Length + Offset];

        // Null
        nullableTimeOnlyConverter.Write(buffer.AsSpan(Offset), null);
        Assert.Equal(EmptyBytes, buffer);
    }

    //--------------------------------------------------------------------------------
    // Builder dispatch: fast-path roundtrip
    //--------------------------------------------------------------------------------

    [Fact]
    public void BuilderDispatchesFastPathAndRoundTrips()
    {
        var converter = CreateViaBuilder(6, Format, typeof(TimeOnly));
        var samples = new[]
        {
            new TimeOnly(0, 0, 0),
            new TimeOnly(12, 34, 56),
            new TimeOnly(23, 59, 59)
        };

        var buffer = new byte[6];
        foreach (var sample in samples)
        {
            converter.Write(buffer, sample);
            Assert.Equal(sample, converter.Read(buffer));
        }
    }

    [Fact]
    public void FastPathRejectsInvalidTimeComponents()
    {
        var converter = CreateViaBuilder(6, Format, typeof(TimeOnly));
        var cases = new[]
        {
            "240000"u8.ToArray(), // hour 24
            "006000"u8.ToArray(), // minute 60
            "000060"u8.ToArray() // second 60
        };
        foreach (var invalid in cases)
        {
            Assert.Equal(default(TimeOnly), converter.Read(invalid));
        }
    }

    //--------------------------------------------------------------------------------
    // BCL fallback: seconds absent → fast path requires HH+mm+ss all three
    //--------------------------------------------------------------------------------

    [Fact]
    public void BuilderFallsBackToBclWhenSecondsMissing()
    {
        // "HH:mm" has no seconds → fast path requires all three (HH/mm/ss) → BCL converter
        var converter = CreateViaBuilder(5, "HH:mm", typeof(TimeOnly));
        var buffer = new byte[5];
        converter.Write(buffer, new TimeOnly(12, 34, 0));
        Assert.Equal("12:34"u8.ToArray(), buffer);
        Assert.Equal(new TimeOnly(12, 34, 0), converter.Read(buffer));
    }

    //--------------------------------------------------------------------------------
    // Separator formats
    //--------------------------------------------------------------------------------

    [Fact]
    public void BuilderFastPathForVariousSeparators()
    {
        var cases = new (string Format, int Length, TimeOnly Sample, string Expected)[]
        {
            ("HH:mm:ss", 8, new TimeOnly(12, 34, 56), "12:34:56"),
            ("HH-mm-ss", 8, new TimeOnly(12, 34, 56), "12-34-56"),
            ("HH.mm.ss", 8, new TimeOnly(12, 34, 56), "12.34.56"),
            ("HHmmss", 6, new TimeOnly(12, 34, 56), "123456")
        };

        foreach (var (format, len, sample, expected) in cases)
        {
            var converter = CreateViaBuilder(len, format, typeof(TimeOnly));
            var buffer = new byte[len];
            converter.Write(buffer, sample);
            Assert.Equal(Encoding.ASCII.GetBytes(expected), buffer);
            Assert.Equal(sample, converter.Read(buffer));
        }
    }

    [Fact]
    public void FastPathLiteralMismatchReturnsDefault()
    {
        var converter = CreateViaBuilder(8, "HH:mm:ss", typeof(TimeOnly));

        // '.' at offset 2 where ':' is expected
        Assert.Equal(default(TimeOnly), converter.Read("12.34:56"u8.ToArray()));
    }

    //--------------------------------------------------------------------------------
    // Width < Length: filler for remainder
    //--------------------------------------------------------------------------------

    [Fact]
    public void FastPathFillerWhenWidthLessThanLength()
    {
        // HHmmss (width=6) with length=10 → fast path: 6 time bytes + 4 filler bytes
        var converter = CreateViaBuilder(10, Format, typeof(TimeOnly));
        var buffer = new byte[10];
        converter.Write(buffer, new TimeOnly(12, 34, 56));
        Assert.Equal("123456"u8.ToArray(), buffer[..6]);
        Assert.Equal("    "u8.ToArray(), buffer[6..]);

        // Read succeeds on the first 6 bytes
        Assert.Equal(new TimeOnly(12, 34, 56), converter.Read(buffer));
    }
}
