namespace Smart.IO.ByteMapper.Converters;

using System.Text;

using Smart.IO.ByteMapper.Mock;

// ReSharper disable StringLiteralTypo
public sealed class DateTimeConverterTest
{
    private const int Offset = 1;

    private const int Length = 17;

    private const string Format = "yyyyMMddHHmmssfff";

    private static readonly DateTime Value = new(2000, 12, 31, 12, 34, 56, 789);

    private static readonly byte[] EmptyBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes(string.Empty.PadRight(Length, ' ')));

    private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, "20001231123456789"u8.ToArray());

    private static readonly byte[] InvalidBytes = TestBytes.Offset(Offset, "xxxxxxxxxxxxxxxxx"u8.ToArray());

    private readonly DateTimeConverter decimalConverter;

    private readonly DateTimeConverter nullableDateTimeConverter;

    public DateTimeConverterTest()
    {
        decimalConverter = CreateConverter(typeof(DateTime), Format);
        nullableDateTimeConverter = CreateConverter(typeof(DateTime?), Format);
    }

    private static DateTimeConverter CreateConverter(Type type, string format)
    {
        return new(format, DateTimeKind.Unspecified, 0x20, type);
    }

    //--------------------------------------------------------------------------------
    // DateTime
    //--------------------------------------------------------------------------------

    [Fact]
    public void ReadToDateTime()
    {
        // Default
        Assert.Equal(default(DateTime), decimalConverter.Read(EmptyBytes, Offset));

        // Invalid
        Assert.Equal(default(DateTime), decimalConverter.Read(InvalidBytes, Offset));

        // Value
        Assert.Equal(Value, decimalConverter.Read(ValueBytes, Offset));
    }

    [Fact]
    public void WriteDateTimeToBuffer()
    {
        var buffer = new byte[Length + Offset];

        // Value
        decimalConverter.Write(buffer, Offset, Value);
        Assert.Equal(ValueBytes, buffer);
    }

    //--------------------------------------------------------------------------------
    // DateTime?
    //--------------------------------------------------------------------------------

    [Fact]
    public void ReadToNullableDateTime()
    {
        // Null
        Assert.Null(nullableDateTimeConverter.Read(EmptyBytes, Offset));

        // Invalid
        Assert.Null(nullableDateTimeConverter.Read(InvalidBytes, Offset));

        // Value
        Assert.Equal(Value, nullableDateTimeConverter.Read(ValueBytes, Offset));
    }

    [Fact]
    public void WriteNullDateTimeToBuffer()
    {
        var buffer = new byte[Length + Offset];

        // Null
        nullableDateTimeConverter.Write(buffer, Offset, null);
        Assert.Equal(EmptyBytes, buffer);
    }
}
