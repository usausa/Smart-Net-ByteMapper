namespace Smart.IO.ByteMapper.Converters;

using System.Text;

using Smart.IO.ByteMapper.Mock;

// ReSharper disable StringLiteralTypo
public sealed class DateTimeOffsetConverterTest
{
    private const int Offset = 1;

    private const int Length = 17;

    private const string Format = "yyyyMMddHHmmssfff";

    private static readonly DateTimeOffset Value = new(new DateTime(2000, 12, 31, 12, 34, 56, 789), TimeSpan.Zero);

    private static readonly byte[] EmptyBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes(string.Empty.PadRight(Length, ' ')));

    private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, "20001231123456789"u8.ToArray());

    private static readonly byte[] MinValueBytes = TestBytes.Offset(Offset, "00010101000000000"u8.ToArray());
    private static readonly byte[] MaxValueBytes = TestBytes.Offset(Offset, "99991232235959999"u8.ToArray());

    private static readonly byte[] InvalidBytes = TestBytes.Offset(Offset, "xxxxxxxxxxxxxxxxx"u8.ToArray());

    private readonly DateTimeOffsetConverter unspecifiedDateTimeConverter;

    private readonly DateTimeOffsetConverter utcDateTimeConverter;

    private readonly DateTimeOffsetConverter localDateTimeConverter;

    private readonly DateTimeOffsetConverter nullableDateTimeOffsetConverter;

    public DateTimeOffsetConverterTest()
    {
        unspecifiedDateTimeConverter = CreateConverter(typeof(DateTimeOffset), Format, DateTimeKind.Unspecified);
        utcDateTimeConverter = CreateConverter(typeof(DateTimeOffset), Format, DateTimeKind.Utc);
        localDateTimeConverter = CreateConverter(typeof(DateTimeOffset), Format, DateTimeKind.Local);
        nullableDateTimeOffsetConverter = CreateConverter(typeof(DateTimeOffset?), Format, DateTimeKind.Unspecified);
    }

    private static DateTimeOffsetConverter CreateConverter(Type type, string format, DateTimeKind kind)
    {
        return new(format, kind, 0x20, type);
    }

    //--------------------------------------------------------------------------------
    // DateTimeOffset
    //--------------------------------------------------------------------------------

    [Fact]
    public void ReadToDateTimeOffset()
    {
        var offset = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).Ticks;

        // Default
        Assert.Equal(default(DateTimeOffset), unspecifiedDateTimeConverter.Read(EmptyBytes.AsSpan(Offset)));
        Assert.Equal(default(DateTimeOffset), utcDateTimeConverter.Read(EmptyBytes.AsSpan(Offset)));
        Assert.Equal(default(DateTimeOffset), localDateTimeConverter.Read(EmptyBytes.AsSpan(Offset)));

        // Invalid
        Assert.Equal(default(DateTimeOffset), unspecifiedDateTimeConverter.Read(InvalidBytes.AsSpan(Offset)));
        Assert.Equal(default(DateTimeOffset), utcDateTimeConverter.Read(InvalidBytes.AsSpan(Offset)));
        Assert.Equal(default(DateTimeOffset), localDateTimeConverter.Read(InvalidBytes.AsSpan(Offset)));

        // Value
        Assert.Equal(Value, unspecifiedDateTimeConverter.Read(ValueBytes.AsSpan(Offset)));
        Assert.Equal(Value, utcDateTimeConverter.Read(ValueBytes.AsSpan(Offset)));
        Assert.Equal(new DateTimeOffset(Value.DateTime, new TimeSpan(offset)), localDateTimeConverter.Read(ValueBytes.AsSpan(Offset)));

        if (offset > 0)
        {
            Assert.Equal(DateTimeOffset.MinValue, localDateTimeConverter.Read(MinValueBytes.AsSpan(Offset)));
        }
        else if (offset < 0)
        {
            Assert.Equal(DateTimeOffset.MaxValue, localDateTimeConverter.Read(MaxValueBytes.AsSpan(Offset)));
        }
    }

    [Fact]
    public void WriteDateTimeOffsetToBuffer()
    {
        var buffer = new byte[Length + Offset];

        // Value
        unspecifiedDateTimeConverter.Write(buffer.AsSpan(Offset), Value);
        Assert.Equal(ValueBytes, buffer);
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
}
