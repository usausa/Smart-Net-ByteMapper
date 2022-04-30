namespace Smart.IO.ByteMapper.Converters;

using System.Globalization;
using System.Text;

using Smart.IO.ByteMapper.Mock;

using Xunit;

public class DateTimeOffsetTextConverterTest
{
    private const int Offset = 1;

    private const int Length = 14;

    private const string Format = "yyyyMMddHHmmss";

    private const string ShortFormat = "yyyyMMdd";

    private static readonly DateTimeOffset Value = new(new DateTime(2000, 12, 31, 12, 34, 56));

    private static readonly byte[] EmptyBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes(string.Empty.PadRight(Length, ' ')));

    private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes("20001231123456"));

    private static readonly byte[] ShortBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes("20001231".PadRight(Length, ' ')));

    private static readonly byte[] InvalidBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes("xxxxxxxxxxxxxx"));

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
        Assert.Equal(default(DateTimeOffset), decimalConverter.Read(EmptyBytes, Offset));

        // Invalid
        Assert.Equal(default(DateTimeOffset), decimalConverter.Read(InvalidBytes, Offset));

        // Value
        Assert.Equal(Value, decimalConverter.Read(ValueBytes, Offset));
    }

    [Fact]
    public void WriteDateTimeOffsetToBuffer()
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
