namespace Smart.IO.ByteMapper.Converters;

using System.Globalization;
using System.Text;

using Smart.IO.ByteMapper.Builders;
using Smart.IO.ByteMapper.Mock;

// ReSharper disable StringLiteralTypo
public sealed class DateTimeOffsetTextConverterTest
{
    private const int Offset = 1;

    private const int Length = 14;

    private const string Format = "yyyyMMddHHmmss";

    private const string ShortFormat = "yyyyMMdd";

    private static readonly DateTimeOffset Value = new(new DateTime(2000, 12, 31, 12, 34, 56));

    private static readonly byte[] EmptyBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes(string.Empty.PadRight(Length, ' ')));

    private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, "20001231123456"u8.ToArray());

    private static readonly byte[] ShortBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes("20001231".PadRight(Length, ' ')));

    private static readonly byte[] InvalidBytes = TestBytes.Offset(Offset, "xxxxxxxxxxxxxx"u8.ToArray());

    private readonly DateTimeOffsetTextConverter decimalConverter;

    private readonly DateTimeOffsetTextConverter nullableDateTimeOffsetConverter;

    private readonly DateTimeOffsetTextConverter shortDecimalConverter;

    public DateTimeOffsetTextConverterTest()
    {
        decimalConverter = CreateConverter(typeof(DateTimeOffset), Format);
        nullableDateTimeOffsetConverter = CreateConverter(typeof(DateTimeOffset?), Format);
        shortDecimalConverter = CreateConverter(typeof(DateTimeOffset), ShortFormat);
    }

    private static DateTimeOffsetTextConverter CreateConverter(Type type, string format)
    {
        return new(14, format, Encoding.ASCII, 0x20, DateTimeStyles.None, DateTimeFormatInfo.InvariantInfo, type);
    }

    //--------------------------------------------------------------------------------
    // DateTimeOffset
    //--------------------------------------------------------------------------------

    [Fact]
    public void ReadToDateTimeOffset()
    {
        // Default
        Assert.Equal(default(DateTimeOffset), decimalConverter.Read(EmptyBytes.AsSpan(Offset)));

        // Invalid
        Assert.Equal(default(DateTimeOffset), decimalConverter.Read(InvalidBytes.AsSpan(Offset)));

        // Value
        Assert.Equal(Value, decimalConverter.Read(ValueBytes.AsSpan(Offset)));
    }

    [Fact]
    public void WriteDateTimeOffsetToBuffer()
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
    // DateTimeOffset?
    //--------------------------------------------------------------------------------

    [Fact]
    public void ReadToNullableDateTimeOffset()
    {
        // Null
        Assert.Null(nullableDateTimeOffsetConverter.Read(EmptyBytes.AsSpan(Offset)));

        // Invalid
        Assert.Null(nullableDateTimeOffsetConverter.Read(InvalidBytes.AsSpan(Offset)));

        // Value
        Assert.Equal(Value, nullableDateTimeOffsetConverter.Read(ValueBytes.AsSpan(Offset)));
    }

    [Fact]
    public void WriteNullDateTimeOffsetToBuffer()
    {
        var buffer = new byte[Length + Offset];

        // Null
        nullableDateTimeOffsetConverter.Write(buffer.AsSpan(Offset), null);
        Assert.Equal(EmptyBytes, buffer);
    }

    //--------------------------------------------------------------------------------
    // Builder dispatch: fast-path vs BCL fallback
    //--------------------------------------------------------------------------------

    private static IMapConverter CreateViaBuilder(int length, string format, Type type)
        => new DateTimeTextConverterBuilder
        {
            Length = length,
            Format = format,
            Encoding = Encoding.ASCII,
            Filler = 0x20,
            Style = DateTimeStyles.None,
            Provider = DateTimeFormatInfo.InvariantInfo
        }.CreateConverter(new MockBuilderContext(), type);

    [Fact]
    public void BuilderDispatchesFastPathAndRoundTrips()
    {
        // Fast path writes local date/time components; read reconstructs with local offset.
        var converter = CreateViaBuilder(14, Format, typeof(DateTimeOffset));
        var buffer = new byte[14];

        // Value was constructed with local-time DateTime → roundtrip must match.
        converter.Write(buffer, Value);
        Assert.Equal("20001231123456"u8.ToArray(), buffer);
        Assert.Equal(Value, converter.Read(buffer));
    }

    [Fact]
    public void FastPathRejectsInvalidComponents()
    {
        var converter = CreateViaBuilder(14, Format, typeof(DateTimeOffset));
        var cases = new[]
        {
            "20240230120000"u8.ToArray(), // Feb 30
            "20231301120000"u8.ToArray(), // month 13
            "20231201240000"u8.ToArray(), // hour 24
            "20231201236000"u8.ToArray() // minute 60
        };
        foreach (var invalid in cases)
        {
            Assert.Equal(default(DateTimeOffset), converter.Read(invalid));
        }
    }

    [Fact]
    public void BuilderFastPathForVariousSeparators()
    {
        var cases = new (string Format, int Length, string Expected)[]
        {
            ("yyyy-MM-dd HH:mm:ss", 19, "2000-12-31 12:34:56"),
            ("yyyy/MM/dd HH:mm:ss", 19, "2000/12/31 12:34:56"),
            ("yyyyMMddHHmmss", 14, "20001231123456")
        };

        foreach (var (format, len, expected) in cases)
        {
            var converter = CreateViaBuilder(len, format, typeof(DateTimeOffset));
            var buffer = new byte[len];
            converter.Write(buffer, Value);
            Assert.Equal(Encoding.ASCII.GetBytes(expected), buffer);
            Assert.Equal(Value, converter.Read(buffer));
        }
    }

    [Fact]
    public void BuilderFallsBackToBclForUnsupportedFormat()
    {
        // 'MMM' is not a supported specifier → BCL fallback
        const string monthNameFormat = "yyyy-MMM-dd HH:mm:ss";
        var converter = CreateViaBuilder(20, monthNameFormat, typeof(DateTimeOffset));
        var buffer = new byte[20];
        converter.Write(buffer, Value);
        Assert.Equal("2000-Dec-31 12:34:56"u8.ToArray(), buffer);
        Assert.Equal(Value, converter.Read(buffer));
    }

    [Fact]
    public void FastPathFillerWhenWidthLessThanLength()
    {
        // yyyyMMdd (width=8) with length=14 → fast path: 8 bytes + 6 filler bytes
        var converter = CreateViaBuilder(14, ShortFormat, typeof(DateTimeOffset));
        var buffer = new byte[14];
        converter.Write(buffer, Value);
        Assert.Equal("20001231"u8.ToArray(), buffer[..8]);
        Assert.Equal("      "u8.ToArray(), buffer[8..]);
        Assert.Equal(new DateTimeOffset(new DateTime(2000, 12, 31)), converter.Read(buffer));
    }

    //--------------------------------------------------------------------------------
    // zzz offset specifier
    //--------------------------------------------------------------------------------

    [Fact]
    public void FastPathZzzWritesPositiveOffset()
    {
        // +09:00 (Tokyo)
        var dto = new DateTimeOffset(2000, 12, 31, 12, 34, 56, TimeSpan.FromHours(9));
        var converter = CreateViaBuilder(25, "yyyy-MM-ddTHH:mm:sszzz", typeof(DateTimeOffset));
        var buffer = new byte[25];
        converter.Write(buffer, dto);
        Assert.Equal("2000-12-31T12:34:56+09:00"u8.ToArray(), buffer);
    }

    [Fact]
    public void FastPathZzzWritesNegativeHalfHourOffset()
    {
        // -05:30
        var dto = new DateTimeOffset(2000, 12, 31, 12, 34, 56, TimeSpan.FromMinutes(-330));
        var converter = CreateViaBuilder(25, "yyyy-MM-ddTHH:mm:sszzz", typeof(DateTimeOffset));
        var buffer = new byte[25];
        converter.Write(buffer, dto);
        Assert.Equal("2000-12-31T12:34:56-05:30"u8.ToArray(), buffer);
    }

    [Fact]
    public void FastPathZzzWritesUtcOffset()
    {
        var dto = new DateTimeOffset(2000, 12, 31, 12, 34, 56, TimeSpan.Zero);
        var converter = CreateViaBuilder(25, "yyyy-MM-ddTHH:mm:sszzz", typeof(DateTimeOffset));
        var buffer = new byte[25];
        converter.Write(buffer, dto);
        Assert.Equal("2000-12-31T12:34:56+00:00"u8.ToArray(), buffer);
    }

    [Fact]
    public void FastPathZzzRoundtrip()
    {
        var dto = new DateTimeOffset(2000, 12, 31, 12, 34, 56, TimeSpan.FromHours(9));
        var converter = CreateViaBuilder(25, "yyyy-MM-ddTHH:mm:sszzz", typeof(DateTimeOffset));
        var buffer = new byte[25];
        converter.Write(buffer, dto);
        var result = (DateTimeOffset)converter.Read(buffer);
        Assert.Equal(dto, result);
        Assert.Equal(TimeSpan.FromHours(9), result.Offset);
    }

    [Fact]
    public void FastPathZzzRoundtripNegativeOffset()
    {
        var dto = new DateTimeOffset(2000, 12, 31, 12, 34, 56, TimeSpan.FromMinutes(-330));
        var converter = CreateViaBuilder(25, "yyyy-MM-ddTHH:mm:sszzz", typeof(DateTimeOffset));
        var buffer = new byte[25];
        converter.Write(buffer, dto);
        var result = (DateTimeOffset)converter.Read(buffer);
        Assert.Equal(dto, result);
        Assert.Equal(TimeSpan.FromMinutes(-330), result.Offset);
    }

    [Fact]
    public void FastPathZzzRejectsInvalidSign()
    {
        var converter = CreateViaBuilder(25, "yyyy-MM-ddTHH:mm:sszzz", typeof(DateTimeOffset));
        var buffer = "2000-12-31T12:34:56x09:00"u8.ToArray();
        Assert.Equal(default(DateTimeOffset), converter.Read(buffer));
    }

    [Fact]
    public void FastPathZzzNullableRoundtrip()
    {
        var dto = new DateTimeOffset(2000, 12, 31, 12, 34, 56, TimeSpan.FromHours(9));
        var converter = CreateViaBuilder(25, "yyyy-MM-ddTHH:mm:sszzz", typeof(DateTimeOffset?));
        var buffer = new byte[25];
        converter.Write(buffer, (DateTimeOffset?)dto);
        var result = (DateTimeOffset?)converter.Read(buffer);
        Assert.Equal(dto, result);
    }
}
