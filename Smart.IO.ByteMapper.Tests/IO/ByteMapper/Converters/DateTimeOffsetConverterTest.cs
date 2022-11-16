namespace Smart.IO.ByteMapper.Converters;

using System.Text;

using Smart.IO.ByteMapper.Mock;

using Xunit;

// ReSharper disable StringLiteralTypo
public class DateTimeOffsetConverterTest
{
    private const int Offset = 1;

    private const int Length = 17;

    private const string Format = "yyyyMMddHHmmssfff";

    private static readonly DateTimeOffset Value = new(new DateTime(2000, 12, 31, 12, 34, 56, 789), TimeSpan.Zero);

    private static readonly byte[] EmptyBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes(string.Empty.PadRight(Length, ' ')));

    private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes("20001231123456789"));

    private static readonly byte[] MinValueBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes("00010101000000000"));
    private static readonly byte[] MaxValueBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes("99991232235959999"));

    private static readonly byte[] InvalidBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes("xxxxxxxxxxxxxxxxx"));

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
        Assert.Equal(default(DateTimeOffset), unspecifiedDateTimeConverter.Read(EmptyBytes, Offset));
        Assert.Equal(default(DateTimeOffset), utcDateTimeConverter.Read(EmptyBytes, Offset));
        Assert.Equal(default(DateTimeOffset), localDateTimeConverter.Read(EmptyBytes, Offset));

        // Invalid
        Assert.Equal(default(DateTimeOffset), unspecifiedDateTimeConverter.Read(InvalidBytes, Offset));
        Assert.Equal(default(DateTimeOffset), utcDateTimeConverter.Read(InvalidBytes, Offset));
        Assert.Equal(default(DateTimeOffset), localDateTimeConverter.Read(InvalidBytes, Offset));

        // Value
        Assert.Equal(Value, unspecifiedDateTimeConverter.Read(ValueBytes, Offset));
        Assert.Equal(Value, utcDateTimeConverter.Read(ValueBytes, Offset));
        Assert.Equal(new DateTimeOffset(Value.DateTime, new TimeSpan(offset)), localDateTimeConverter.Read(ValueBytes, Offset));

        if (offset > 0)
        {
            Assert.Equal(DateTimeOffset.MinValue, localDateTimeConverter.Read(MinValueBytes, Offset));
        }
        else if (offset < 0)
        {
            Assert.Equal(DateTimeOffset.MaxValue, localDateTimeConverter.Read(MaxValueBytes, Offset));
        }
    }

    [Fact]
    public void WriteDateTimeOffsetToBuffer()
    {
        var buffer = new byte[Length + Offset];

        // Value
        unspecifiedDateTimeConverter.Write(buffer, Offset, Value);
        Assert.Equal(ValueBytes, buffer);
    }

    //--------------------------------------------------------------------------------
    // DateTimeOffset?
    //--------------------------------------------------------------------------------

    [Fact]
    public void ReadToNullableDateTimeOffset()
    {
        // Null
        Assert.Null(nullableDateTimeOffsetConverter.Read(EmptyBytes, Offset));

        // Invalid
        Assert.Null(nullableDateTimeOffsetConverter.Read(InvalidBytes, Offset));

        // Value
        Assert.Equal(Value, nullableDateTimeOffsetConverter.Read(ValueBytes, Offset));
    }

    [Fact]
    public void WriteNullDateTimeOffsetToBuffer()
    {
        var buffer = new byte[Length + Offset];

        // Null
        nullableDateTimeOffsetConverter.Write(buffer, Offset, null);
        Assert.Equal(EmptyBytes, buffer);
    }
}
