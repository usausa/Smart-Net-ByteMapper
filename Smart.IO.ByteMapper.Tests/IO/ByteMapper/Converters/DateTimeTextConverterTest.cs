namespace Smart.IO.ByteMapper.Converters;

using System.Globalization;
using System.Text;

using Smart.IO.ByteMapper.Builders;
using Smart.IO.ByteMapper.Mock;

// ReSharper disable StringLiteralTypo
public sealed class DateTimeTextConverterTest
{
    private const int Offset = 1;

    private const int Length = 14;

    private const string Format = "yyyyMMddHHmmss";

    private const string ShortFormat = "yyyyMMdd";

    private static readonly DateTime Value = new(2000, 12, 31, 12, 34, 56);

    private static readonly byte[] EmptyBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes(string.Empty.PadRight(Length, ' ')));

    private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, "20001231123456"u8.ToArray());

    private static readonly byte[] ShortBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes("20001231".PadRight(Length, ' ')));

    private static readonly byte[] InvalidBytes = TestBytes.Offset(Offset, "xxxxxxxxxxxxxx"u8.ToArray());

    private readonly DateTimeTextConverter decimalConverter;

    private readonly DateTimeTextConverter nullableDateTimeConverter;

    private readonly DateTimeTextConverter shortDecimalConverter;

    public DateTimeTextConverterTest()
    {
        decimalConverter = CreateConverter(typeof(DateTime), Format);
        nullableDateTimeConverter = CreateConverter(typeof(DateTime?), Format);
        shortDecimalConverter = CreateConverter(typeof(DateTime), ShortFormat);
    }

    private static DateTimeTextConverter CreateConverter(Type type, string format)
    {
        return new(14, format, Encoding.ASCII, 0x20, DateTimeStyles.None, DateTimeFormatInfo.InvariantInfo, type);
    }

    //--------------------------------------------------------------------------------
    // DateTime
    //--------------------------------------------------------------------------------

    [Fact]
    public void ReadToDateTime()
    {
        // Default
        Assert.Equal(default(DateTime), decimalConverter.Read(EmptyBytes.AsSpan(Offset)));

        // Invalid
        Assert.Equal(default(DateTime), decimalConverter.Read(InvalidBytes.AsSpan(Offset)));

        // Value
        Assert.Equal(Value, decimalConverter.Read(ValueBytes.AsSpan(Offset)));
    }

    [Fact]
    public void WriteDateTimeToBuffer()
    {
        var buffer = new byte[Length + Offset];

        // Value
        decimalConverter.Write(buffer.AsSpan(Offset), Value);
        Assert.Equal(ValueBytes, buffer);

        // Short
        shortDecimalConverter.Write(buffer.AsSpan(Offset), Value);
        Assert.Equal(ShortBytes, buffer);
    }

    //--------------------------------------------------------------------------------
    // DateTime?
    //--------------------------------------------------------------------------------

    [Fact]
    public void ReadToNullableDateTime()
    {
        // Null
        Assert.Null(nullableDateTimeConverter.Read(EmptyBytes.AsSpan(Offset)));

        // Invalid
        Assert.Null(nullableDateTimeConverter.Read(InvalidBytes.AsSpan(Offset)));

        // Value
        Assert.Equal(Value, nullableDateTimeConverter.Read(ValueBytes.AsSpan(Offset)));
    }

    [Fact]
    public void WriteNullDateTimeToBuffer()
    {
        var buffer = new byte[Length + Offset];

        // Null
        nullableDateTimeConverter.Write(buffer.AsSpan(Offset), null);
        Assert.Equal(EmptyBytes, buffer);
    }

    //--------------------------------------------------------------------------------
    // Builder dispatch: fast-path vs BCL fallback
    //--------------------------------------------------------------------------------

    private static IMapConverter CreateViaBuilder(int length, string format, Type type)
    {
        return new DateTimeTextConverterBuilder
        {
            Length = length,
            Format = format,
            Encoding = Encoding.ASCII,
            Filler = 0x20,
            Style = DateTimeStyles.None,
            Provider = DateTimeFormatInfo.InvariantInfo
        }.CreateConverter(new MockBuilderContext(), type);
    }

    [Fact]
    public void BuilderDispatchesFastPathAndRoundTrips()
    {
        // Builder returns fast-path converter; verify roundtrip for boundary samples.
        var converter = CreateViaBuilder(14, Format, typeof(DateTime));
        var samples = new[]
        {
            new DateTime(2000, 1, 1, 0, 0, 0),
            new DateTime(2024, 2, 29, 23, 59, 59),
            new DateTime(2099, 12, 31, 12, 34, 56),
            new DateTime(1, 1, 1, 0, 0, 0)
        };

        var buffer = new byte[14];
        foreach (var sample in samples)
        {
            converter.Write(buffer, sample);
            Assert.Equal(sample, converter.Read(buffer));
        }
    }

    [Fact]
    public void FastPathRejectsInvalidComponents()
    {
        // Invalid date/time components must all return default(DateTime).
        var converter = CreateViaBuilder(14, Format, typeof(DateTime));
        var cases = new[]
        {
            "20240230120000"u8.ToArray(), // Feb 30
            "20231301120000"u8.ToArray(), // month 13
            "20231232120000"u8.ToArray(), // day 32
            "20231201240000"u8.ToArray(), // hour 24
            "20231201236000"u8.ToArray(), // minute 60
            "20231201235960"u8.ToArray() // second 60
        };

        foreach (var invalid in cases)
        {
            Assert.Equal(default(DateTime), converter.Read(invalid));
        }
    }

    [Fact]
    public void BuilderDispatchesFastPathForDateOnly()
    {
        // yyyyMMdd with length = 8 → builder selects fast-path converter.
        var converter = CreateViaBuilder(8, ShortFormat, typeof(DateTime));

        var buffer = new byte[8];
        converter.Write(buffer, new DateTime(2026, 5, 17, 12, 34, 56));
        Assert.Equal("20260517"u8.ToArray(), buffer);

        Assert.Equal(new DateTime(2026, 5, 17), converter.Read(buffer));
    }

    [Fact]
    public void BuilderFallsBackToBclForUnsupportedFormat()
    {
        // 'MMM' (text month) is not supported by the fast path → builder returns BCL converter.
        const string monthNameFormat = "yyyy-MMM-dd";
        var converter = CreateViaBuilder(11, monthNameFormat, typeof(DateTime));

        var sample = new DateTime(2026, 5, 17);
        var buffer = new byte[11];
        converter.Write(buffer, sample);
        Assert.Equal("2026-May-17"u8.ToArray(), buffer);
        Assert.Equal(sample, converter.Read(buffer));
    }

    [Fact]
    public void BuilderFallsBackToBclWhenLengthExceedsFormatWidth()
    {
        // length(14) > format width(8) → fast path disabled; BCL path pads write,
        // and read of "20001231      " fails the format → default.
        var buffer = new byte[Length + Offset];
        shortDecimalConverter.Write(buffer.AsSpan(Offset), Value);
        Assert.Equal(ShortBytes, buffer);

        Assert.Equal(default(DateTime), shortDecimalConverter.Read(buffer.AsSpan(Offset)));
    }

    //--------------------------------------------------------------------------------
    // Separator formats: any non-reserved ASCII char is a literal byte
    //--------------------------------------------------------------------------------

    [Fact]
    public void BuilderFastPathForVariousSeparators()
    {
        // Every non-reserved ASCII character is treated as a literal byte by the fast path.
        // Verify roundtrip for common separator patterns.
        var cases = new (string Format, int Length, DateTime Sample, string Expected)[]
        {
            ("yyyy/MM/dd HH:mm:ss", 19, new DateTime(2026, 5, 17, 12, 34, 56), "2026/05/17 12:34:56"),
            ("yyyy-MM-dd HH:mm:ss", 19, new DateTime(2026, 5, 17, 12, 34, 56), "2026-05-17 12:34:56"),
            ("yyyy.MM.dd HH.mm.ss", 19, new DateTime(2026, 5, 17, 12, 34, 56), "2026.05.17 12.34.56"),
            ("yyyy-MM-ddTHH:mm:ss", 19, new DateTime(2026, 5, 17, 12, 34, 56), "2026-05-17T12:34:56"),
            ("yyyy-MM-dd",          10, new DateTime(2026, 5, 17),              "2026-05-17"),
            ("yyyy/MM/dd",          10, new DateTime(2026, 5, 17),              "2026/05/17"),
            ("yyyy.MM.dd",          10, new DateTime(2026, 5, 17),              "2026.05.17")
        };

        foreach (var (format, len, sample, expected) in cases)
        {
            var converter = CreateViaBuilder(len, format, typeof(DateTime));
            var buffer = new byte[len];
            converter.Write(buffer, sample);
            Assert.Equal(Encoding.ASCII.GetBytes(expected), buffer);
            Assert.Equal(sample, converter.Read(buffer));
        }
    }

    [Fact]
    public void FastPathLiteralMismatchReturnsDefault()
    {
        // A literal byte in the buffer that doesn't match the format returns default.
        var converter = CreateViaBuilder(19, "yyyy/MM/dd HH:mm:ss", typeof(DateTime));

        // '/' expected at offset 4, but '-' present.
        Assert.Equal(default(DateTime), converter.Read("2026-05-17 12:34:56"u8.ToArray()));

        // ':' expected at offset 13, but '.' present.
        Assert.Equal(default(DateTime), converter.Read("2026/05/17 12.34:56"u8.ToArray()));
    }

    //--------------------------------------------------------------------------------
    // 2-digit year (yy)
    //--------------------------------------------------------------------------------

    [Fact]
    public void Year2FastPathRoundTripsAndAppliesCutoff()
    {
        // yy <= 29 → 2000+yy, yy >= 30 → 1900+yy (matches BCL InvariantCulture TwoDigitYearMax=2029).
        var converter = CreateViaBuilder(6, "yyMMdd", typeof(DateTime));
        var buffer = new byte[6];

        // yy=29 → 2029
        converter.Write(buffer, new DateTime(2029, 5, 17));
        Assert.Equal("290517"u8.ToArray(), buffer);
        Assert.Equal(new DateTime(2029, 5, 17), converter.Read(buffer));

        // yy=30 → 1930
        converter.Write(buffer, new DateTime(1930, 8, 15));
        Assert.Equal("300815"u8.ToArray(), buffer);
        Assert.Equal(new DateTime(1930, 8, 15), converter.Read(buffer));

        // yy=00 → 2000
        converter.Write(buffer, new DateTime(2000, 1, 1));
        Assert.Equal("000101"u8.ToArray(), buffer);
        Assert.Equal(new DateTime(2000, 1, 1), converter.Read(buffer));

        // yy=99 → 1999
        converter.Write(buffer, new DateTime(1999, 12, 31));
        Assert.Equal("991231"u8.ToArray(), buffer);
        Assert.Equal(new DateTime(1999, 12, 31), converter.Read(buffer));
    }

    [Fact]
    public void Year2FastPathWithSeparator()
    {
        var converter = CreateViaBuilder(8, "yy-MM-dd", typeof(DateTime));
        var buffer = new byte[8];
        converter.Write(buffer, new DateTime(2026, 5, 17));
        Assert.Equal("26-05-17"u8.ToArray(), buffer);
        Assert.Equal(new DateTime(2026, 5, 17), converter.Read(buffer));
    }

    //--------------------------------------------------------------------------------
    // Width < length: filler appended for remainder
    //--------------------------------------------------------------------------------

    [Fact]
    public void FastPathFillerWhenWidthLessThanLength()
    {
        // yyyyMMdd has width=8; length=14 → builder uses fast path, writes 8 date bytes + 6 filler bytes.
        var converter = CreateViaBuilder(14, ShortFormat, typeof(DateTime));
        var buffer = new byte[14];
        converter.Write(buffer, new DateTime(2026, 5, 17));
        Assert.Equal("20260517"u8.ToArray(), buffer[..8]);
        Assert.Equal("      "u8.ToArray(), buffer[8..]);

        // Read should succeed (fast parser only inspects the first Width=8 bytes).
        Assert.Equal(new DateTime(2026, 5, 17), converter.Read(buffer));
    }

    [Fact]
    public void FastPathFillerNullWritesFull()
    {
        // Null value must fill the entire length, not just Width bytes.
        var converter = CreateViaBuilder(14, ShortFormat, typeof(DateTime?));
        var buffer = new byte[14];
        converter.Write(buffer, null);
        var expected = new byte[14];
        expected.AsSpan().Fill(0x20);
        Assert.Equal(expected, buffer);
    }
}
