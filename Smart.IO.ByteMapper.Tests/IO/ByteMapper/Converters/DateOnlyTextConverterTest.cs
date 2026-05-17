namespace Smart.IO.ByteMapper.Converters;

using System.Text;

using Smart.IO.ByteMapper.Builders;
using Smart.IO.ByteMapper.Mock;

public sealed class DateOnlyTextConverterTest
{
    private const int Offset = 1;

    private const int Length = 8;

    private const string Format = "yyyyMMdd";

    private static readonly DateOnly Value = new(2000, 12, 31);

    private static readonly byte[] EmptyBytes = TestBytes.Offset(Offset, "        "u8.ToArray());

    private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, "20001231"u8.ToArray());

    private static readonly byte[] InvalidBytes = TestBytes.Offset(Offset, "xxxxxxxx"u8.ToArray());

    private readonly DateOnlyTextConverter dateOnlyConverter;

    private readonly DateOnlyTextConverter nullableDateOnlyConverter;

    public DateOnlyTextConverterTest()
    {
        dateOnlyConverter = CreateConverter(typeof(DateOnly), Format);
        nullableDateOnlyConverter = CreateConverter(typeof(DateOnly?), Format);
    }

    private static DateOnlyTextConverter CreateConverter(Type type, string format)
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
    // DateOnly (BCL path)
    //--------------------------------------------------------------------------------

    [Fact]
    public void ReadToDateOnly()
    {
        // Default
        Assert.Equal(default(DateOnly), dateOnlyConverter.Read(EmptyBytes.AsSpan(Offset)));

        // Invalid
        Assert.Equal(default(DateOnly), dateOnlyConverter.Read(InvalidBytes.AsSpan(Offset)));

        // Value
        Assert.Equal(Value, dateOnlyConverter.Read(ValueBytes.AsSpan(Offset)));
    }

    [Fact]
    public void WriteDateOnlyToBuffer()
    {
        var buffer = new byte[Length + Offset];

        // Value
        dateOnlyConverter.Write(buffer.AsSpan(Offset), Value);
        Assert.Equal(ValueBytes, buffer);
    }

    //--------------------------------------------------------------------------------
    // DateOnly? (BCL path)
    //--------------------------------------------------------------------------------

    [Fact]
    public void ReadToNullableDateOnly()
    {
        // Null
        Assert.Null(nullableDateOnlyConverter.Read(EmptyBytes.AsSpan(Offset)));

        // Invalid
        Assert.Null(nullableDateOnlyConverter.Read(InvalidBytes.AsSpan(Offset)));

        // Value
        Assert.Equal(Value, nullableDateOnlyConverter.Read(ValueBytes.AsSpan(Offset)));
    }

    [Fact]
    public void WriteNullDateOnlyToBuffer()
    {
        var buffer = new byte[Length + Offset];

        // Null
        nullableDateOnlyConverter.Write(buffer.AsSpan(Offset), null);
        Assert.Equal(EmptyBytes, buffer);
    }

    //--------------------------------------------------------------------------------
    // Builder dispatch: fast-path roundtrip
    //--------------------------------------------------------------------------------

    [Fact]
    public void BuilderDispatchesFastPathAndRoundTrips()
    {
        var converter = CreateViaBuilder(8, Format, typeof(DateOnly));
        var samples = new[]
        {
            new DateOnly(2000, 1, 1),
            new DateOnly(2024, 2, 29),  // leap day
            new DateOnly(2099, 12, 31),
            new DateOnly(1, 1, 1)
        };

        var buffer = new byte[8];
        foreach (var sample in samples)
        {
            converter.Write(buffer, sample);
            Assert.Equal(sample, converter.Read(buffer));
        }
    }

    [Fact]
    public void FastPathRejectsInvalidDateComponents()
    {
        var converter = CreateViaBuilder(8, Format, typeof(DateOnly));
        var cases = new[]
        {
            "20240230"u8.ToArray(), // Feb 30
            "20231301"u8.ToArray(), // month 13
            "20231232"u8.ToArray() // day 32
        };
        foreach (var invalid in cases)
        {
            Assert.Equal(default(DateOnly), converter.Read(invalid));
        }
    }

    //--------------------------------------------------------------------------------
    // Separator formats
    //--------------------------------------------------------------------------------

    [Fact]
    public void BuilderFastPathForVariousSeparators()
    {
        var cases = new (string Format, int Length, DateOnly Sample, string Expected)[]
        {
            ("yyyy-MM-dd", 10, new DateOnly(2026, 5, 17), "2026-05-17"),
            ("yyyy/MM/dd", 10, new DateOnly(2026, 5, 17), "2026/05/17"),
            ("yyyy.MM.dd", 10, new DateOnly(2026, 5, 17), "2026.05.17"),
            ("yyyyMMdd", 8, new DateOnly(2026, 5, 17), "20260517"),
            ("yy/MM/dd", 8, new DateOnly(2026, 5, 17), "26/05/17")
        };

        foreach (var (format, len, sample, expected) in cases)
        {
            var converter = CreateViaBuilder(len, format, typeof(DateOnly));
            var buffer = new byte[len];
            converter.Write(buffer, sample);
            Assert.Equal(Encoding.ASCII.GetBytes(expected), buffer);
            Assert.Equal(sample, converter.Read(buffer));
        }
    }

    [Fact]
    public void FastPathLiteralMismatchReturnsDefault()
    {
        var converter = CreateViaBuilder(10, "yyyy/MM/dd", typeof(DateOnly));

        // '-' at offset 4 where '/' is expected
        Assert.Equal(default(DateOnly), converter.Read("2026-05-17"u8.ToArray()));
    }

    //--------------------------------------------------------------------------------
    // Width < Length: filler for remainder
    //--------------------------------------------------------------------------------

    [Fact]
    public void FastPathFillerWhenWidthLessThanLength()
    {
        // yyyyMMdd (width=8) with length=14 → fast path: 8 date bytes + 6 filler bytes
        var converter = CreateViaBuilder(14, Format, typeof(DateOnly));
        var buffer = new byte[14];
        converter.Write(buffer, new DateOnly(2026, 5, 17));
        Assert.Equal("20260517"u8.ToArray(), buffer[..8]);
        Assert.Equal("      "u8.ToArray(), buffer[8..]);

        // Read succeeds on the first 8 bytes
        Assert.Equal(new DateOnly(2026, 5, 17), converter.Read(buffer));
    }

    [Fact]
    public void FastPathNullableFillerWhenWidthLessThanLength()
    {
        // Null fills entire length
        var converter = CreateViaBuilder(14, Format, typeof(DateOnly?));
        var buffer = new byte[14];
        converter.Write(buffer, null);
        var expected = new byte[14];
        expected.AsSpan().Fill(0x20);
        Assert.Equal(expected, buffer);
    }
}
