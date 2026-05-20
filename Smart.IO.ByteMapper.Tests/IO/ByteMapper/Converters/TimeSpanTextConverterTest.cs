namespace Smart.IO.ByteMapper.Converters;

using System.Text;

using Smart.IO.ByteMapper.Builders;
using Smart.IO.ByteMapper.Mock;

// ReSharper disable StringLiteralTypo
public sealed class TimeSpanTextConverterTest
{
    private const int Offset = 1;

    private const int Length = 8;

    private const string Format = @"hh\:mm\:ss";

    private static readonly TimeSpan Value = new(1, 23, 45);

    private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, "01:23:45"u8.ToArray());

    private static readonly byte[] EmptyBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes(string.Empty.PadRight(Length, ' ')));

    private static readonly byte[] InvalidBytes = TestBytes.Offset(Offset, "xxxxxxxx"u8.ToArray());

    private static TimeSpanTextConverter CreateConverter(Type type)
        => new(Length, Format, Encoding.ASCII, 0x20, type);

    //--------------------------------------------------------------------------------
    // TimeSpan (BCL converter)
    //--------------------------------------------------------------------------------

    [Fact]
    public void ReadToTimeSpan()
    {
        Assert.Equal(default(TimeSpan), CreateConverter(typeof(TimeSpan)).Read(EmptyBytes.AsSpan(Offset)));
        Assert.Equal(default(TimeSpan), CreateConverter(typeof(TimeSpan)).Read(InvalidBytes.AsSpan(Offset)));
        Assert.Equal(Value, CreateConverter(typeof(TimeSpan)).Read(ValueBytes.AsSpan(Offset)));
    }

    [Fact]
    public void WriteTimeSpanToBuffer()
    {
        var buffer = new byte[Length + Offset];
        CreateConverter(typeof(TimeSpan)).Write(buffer.AsSpan(Offset), Value);
        Assert.Equal(ValueBytes, buffer);
    }

    //--------------------------------------------------------------------------------
    // TimeSpan? (BCL converter)
    //--------------------------------------------------------------------------------

    [Fact]
    public void ReadToNullableTimeSpan()
    {
        Assert.Null(CreateConverter(typeof(TimeSpan?)).Read(EmptyBytes.AsSpan(Offset)));
        Assert.Null(CreateConverter(typeof(TimeSpan?)).Read(InvalidBytes.AsSpan(Offset)));
        Assert.Equal(Value, CreateConverter(typeof(TimeSpan?)).Read(ValueBytes.AsSpan(Offset)));
    }

    [Fact]
    public void WriteNullTimeSpanToBuffer()
    {
        var buffer = new byte[Length + Offset];
        CreateConverter(typeof(TimeSpan?)).Write(buffer.AsSpan(Offset), null);
        Assert.Equal(EmptyBytes, buffer);
    }
}

public sealed class TimeSpanTextFastConverterTest
{
    private static IMapConverter CreateViaBuilder(int length, string format, Type type)
        => new DateTimeTextConverterBuilder
        {
            Length = length,
            Format = format,
            Encoding = Encoding.ASCII,
            Filler = 0x20
        }.CreateConverter(new MockBuilderContext(), type);

    //--------------------------------------------------------------------------------
    // Basic formats
    //--------------------------------------------------------------------------------

    [Fact]
    public void RoundtripHhmmss()
    {
        var value = new TimeSpan(1, 23, 45);
        var converter = CreateViaBuilder(8, "hh:mm:ss", typeof(TimeSpan));
        var buffer = new byte[8];
        converter.Write(buffer, value);
        Assert.Equal("01:23:45"u8.ToArray(), buffer);
        Assert.Equal(value, converter.Read(buffer));
    }

    [Fact]
    public void RoundtripEscapedSeparators()
    {
        // hh\:mm\:ss and hh:mm:ss produce identical block arrays
        var value = new TimeSpan(1, 23, 45);
        var converter = CreateViaBuilder(8, "hh\\:mm\\:ss", typeof(TimeSpan));
        var buffer = new byte[8];
        converter.Write(buffer, value);
        Assert.Equal("01:23:45"u8.ToArray(), buffer);
        Assert.Equal(value, converter.Read(buffer));
    }

    [Fact]
    public void RoundtripWithDays()
    {
        var value = new TimeSpan(3, 1, 23, 45);
        var converter = CreateViaBuilder(11, "dd\\.hh\\:mm\\:ss", typeof(TimeSpan));
        var buffer = new byte[11];
        converter.Write(buffer, value);
        Assert.Equal("03.01:23:45"u8.ToArray(), buffer);
        Assert.Equal(value, converter.Read(buffer));
    }

    [Fact]
    public void RoundtripSingleDigitFields()
    {
        // h:m:s — hours/minutes/seconds must be 0-9
        var value = new TimeSpan(0, 1, 2, 3);
        var converter = CreateViaBuilder(5, "h:m:s", typeof(TimeSpan));
        var buffer = new byte[5];
        converter.Write(buffer, value);
        Assert.Equal("1:2:3"u8.ToArray(), buffer);
        Assert.Equal(value, converter.Read(buffer));
    }

    [Fact]
    public void RoundtripWithMilliseconds()
    {
        var value = new TimeSpan(0, 1, 23, 45, 678);
        var converter = CreateViaBuilder(12, "hh:mm:ss.fff", typeof(TimeSpan));
        var buffer = new byte[12];
        converter.Write(buffer, value);
        Assert.Equal("01:23:45.678"u8.ToArray(), buffer);
        Assert.Equal(value, converter.Read(buffer));
    }

    [Fact]
    public void RoundtripWithFullFraction()
    {
        // fffffff = all 7 fraction digits (ticks precision)
        var value = new TimeSpan(0, 1, 23, 45, 678) + TimeSpan.FromTicks(9);
        var converter = CreateViaBuilder(16, "hh:mm:ss.fffffff", typeof(TimeSpan));
        var buffer = new byte[16];
        converter.Write(buffer, value);
        Assert.Equal("01:23:45.6780009"u8.ToArray(), buffer);
        Assert.Equal(value, converter.Read(buffer));
    }

    //--------------------------------------------------------------------------------
    // Overflow and negative
    //--------------------------------------------------------------------------------

    [Fact]
    public void NegativeValueFillsFiller()
    {
        var converter = CreateViaBuilder(8, "hh:mm:ss", typeof(TimeSpan));
        var buffer = new byte[8];
        converter.Write(buffer, TimeSpan.FromSeconds(-1));
        Assert.Equal("        "u8.ToArray(), buffer);
    }

    [Fact]
    public void DaysOverflowFillsFiller()
    {
        // dd allows 0-99; 100 days overflows
        var converter = CreateViaBuilder(11, "dd\\.hh\\:mm\\:ss", typeof(TimeSpan));
        var buffer = new byte[11];
        converter.Write(buffer, TimeSpan.FromDays(100));
        Assert.Equal("           "u8.ToArray(), buffer);
    }

    [Fact]
    public void SingleDigitHourOverflowFillsFiller()
    {
        // h allows 0-9; 10 hours overflows → fast path returns false → filler
        var converter = CreateViaBuilder(7, "h:mm:ss", typeof(TimeSpan));
        var buffer = new byte[7];
        converter.Write(buffer, new TimeSpan(0, 10, 0, 0));
        Assert.Equal("       "u8.ToArray(), buffer);
    }

    //--------------------------------------------------------------------------------
    // Invalid input on read
    //--------------------------------------------------------------------------------

    [Fact]
    public void InvalidBytesReturnsDefault()
    {
        var converter = CreateViaBuilder(8, "hh:mm:ss", typeof(TimeSpan));
        var buffer = "xxxxxxxx"u8.ToArray();
        Assert.Equal(default(TimeSpan), converter.Read(buffer));
    }

    [Fact]
    public void InvalidComponentRangeReturnsDefault()
    {
        var converter = CreateViaBuilder(8, "hh:mm:ss", typeof(TimeSpan));
        Assert.Equal(default(TimeSpan), converter.Read("25:00:00"u8.ToArray())); // hour 25
        Assert.Equal(default(TimeSpan), converter.Read("00:60:00"u8.ToArray())); // minute 60
        Assert.Equal(default(TimeSpan), converter.Read("00:00:60"u8.ToArray())); // second 60
    }

    [Fact]
    public void LiteralMismatchReturnsDefault()
    {
        var converter = CreateViaBuilder(8, "hh:mm:ss", typeof(TimeSpan));
        Assert.Equal(default(TimeSpan), converter.Read("01-23-45"u8.ToArray())); // '-' instead of ':'
    }

    //--------------------------------------------------------------------------------
    // Nullable
    //--------------------------------------------------------------------------------

    [Fact]
    public void NullableRoundtrip()
    {
        var value = new TimeSpan(1, 23, 45);
        var converter = CreateViaBuilder(8, "hh:mm:ss", typeof(TimeSpan?));
        var buffer = new byte[8];
        converter.Write(buffer, (TimeSpan?)value);
        Assert.Equal(value, converter.Read(buffer));
    }

    [Fact]
    public void NullableWriteNullFillsFiller()
    {
        var converter = CreateViaBuilder(8, "hh:mm:ss", typeof(TimeSpan?));
        var buffer = new byte[8];
        converter.Write(buffer, null);
        Assert.Equal("        "u8.ToArray(), buffer);
    }

    [Fact]
    public void NullableReadInvalidReturnsNull()
    {
        var converter = CreateViaBuilder(8, "hh:mm:ss", typeof(TimeSpan?));
        Assert.Null(converter.Read("xxxxxxxx"u8.ToArray()));
    }

    //--------------------------------------------------------------------------------
    // Filler when width < length
    //--------------------------------------------------------------------------------

    [Fact]
    public void FillerWhenWidthLessThanLength()
    {
        // hh:mm:ss width=8, field length=12
        var value = new TimeSpan(1, 23, 45);
        var converter = CreateViaBuilder(12, "hh:mm:ss", typeof(TimeSpan));
        var buffer = new byte[12];
        converter.Write(buffer, value);
        Assert.Equal("01:23:45"u8.ToArray(), buffer[..8]);
        Assert.Equal("    "u8.ToArray(), buffer[8..]);
        Assert.Equal(value, converter.Read(buffer));
    }

    //--------------------------------------------------------------------------------
    // BCL fallback
    //--------------------------------------------------------------------------------

    [Fact]
    public void BuilderFallsBackToBclForUnsupportedFormat()
    {
        // Standard format "c" — no h/m/s specifiers → TryCreate returns null → BCL fallback
        var value = new TimeSpan(1, 23, 45);
        var converter = CreateViaBuilder(8, "c", typeof(TimeSpan));
        var buffer = new byte[8];
        converter.Write(buffer, value);
        Assert.Equal("01:23:45"u8.ToArray(), buffer);
        Assert.Equal(value, converter.Read(buffer));
    }

    [Fact]
    public void BuilderFallsBackToBclForStandardFormat()
    {
        // 'g' is a standard TimeSpan format — variable width → BCL fallback
        var converter = CreateViaBuilder(7, "g", typeof(TimeSpan));
        Assert.NotNull(converter);
    }
}
