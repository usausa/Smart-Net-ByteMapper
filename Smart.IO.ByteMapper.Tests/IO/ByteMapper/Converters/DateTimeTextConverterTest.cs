namespace Smart.IO.ByteMapper.Converters;

using System.Globalization;
using System.Text;

using Smart.IO.ByteMapper.Mock;

using Xunit;

// ReSharper disable StringLiteralTypo
public class DateTimeTextConverterTest
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

        // Short
        shortDecimalConverter.Write(buffer, Offset, Value);
        Assert.Equal(ShortBytes, buffer);
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
