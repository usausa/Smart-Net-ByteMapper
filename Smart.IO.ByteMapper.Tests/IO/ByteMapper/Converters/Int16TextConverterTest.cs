namespace Smart.IO.ByteMapper.Converters;

using System.Globalization;
using System.Text;

using Smart.IO.ByteMapper.Mock;

using Xunit;

public class Int16TextConverterTest
{
    private const int Offset = 1;

    private const int Length = 4;

    private static readonly byte[] EmptyBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes(string.Empty.PadLeft(Length, ' ')));

    private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes("1".PadLeft(Length, ' ')));

    private static readonly byte[] MinusBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes("-1".PadLeft(Length, ' ')));

    private readonly Int16TextConverter int16Converter;

    private readonly Int16TextConverter nullableInt16Converter;

    private readonly Int16TextConverter enumConverter;

    private readonly Int16TextConverter nullableEnumConverter;

    public Int16TextConverterTest()
    {
        int16Converter = CreateConverter(typeof(short));
        nullableInt16Converter = CreateConverter(typeof(short?));
        enumConverter = CreateConverter(typeof(ShortEnum));
        nullableEnumConverter = CreateConverter(typeof(ShortEnum?));
    }

    private static Int16TextConverter CreateConverter(Type type)
    {
        return new(Length, null, Encoding.ASCII, true, Padding.Left, 0x20, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, type);
    }

    //--------------------------------------------------------------------------------
    // short
    //--------------------------------------------------------------------------------

    [Fact]
    public void ReadToShort()
    {
        // Default
        Assert.Equal((short)0, int16Converter.Read(EmptyBytes, Offset));

        // Value
        Assert.Equal((short)1, int16Converter.Read(ValueBytes, Offset));
    }

    [Fact]
    public void WriteShortToBuffer()
    {
        var buffer = new byte[Length + Offset];

        // Value
        int16Converter.Write(buffer, Offset, (short)1);
        Assert.Equal(ValueBytes, buffer);
    }

    //--------------------------------------------------------------------------------
    // short?
    //--------------------------------------------------------------------------------

    [Fact]
    public void ReadToNullableShort()
    {
        // Null
        Assert.Null(nullableInt16Converter.Read(EmptyBytes, Offset));

        // Value
        Assert.Equal((short)1, nullableInt16Converter.Read(ValueBytes, Offset));
    }

    [Fact]
    public void WriteNullShortToBuffer()
    {
        var buffer = new byte[Length + Offset];

        // Null
        nullableInt16Converter.Write(buffer, Offset, null);
        Assert.Equal(EmptyBytes, buffer);
    }

    //--------------------------------------------------------------------------------
    // enum
    //--------------------------------------------------------------------------------

    [Fact]
    public void ReadToShortEnum()
    {
        // Default
        Assert.Equal(ShortEnum.Zero, enumConverter.Read(EmptyBytes, Offset));

        // Value
        Assert.Equal(ShortEnum.One, enumConverter.Read(ValueBytes, Offset));

        // Undefined
        Assert.Equal((ShortEnum)(-1), enumConverter.Read(MinusBytes, Offset));
    }

    [Fact]
    public void WriteShortEnumToBuffer()
    {
        var buffer = new byte[Length + Offset];

        // Value
        enumConverter.Write(buffer, Offset, ShortEnum.One);
        Assert.Equal(ValueBytes, buffer);

        // Undefined
        enumConverter.Write(buffer, Offset, (ShortEnum)(-1));
        Assert.Equal(MinusBytes, buffer);
    }

    //--------------------------------------------------------------------------------
    // enum?
    //--------------------------------------------------------------------------------

    [Fact]
    public void ReadToNullableShortEnum()
    {
        // Null
        Assert.Null(nullableEnumConverter.Read(EmptyBytes, Offset));

        // Value
        Assert.Equal(ShortEnum.One, nullableEnumConverter.Read(ValueBytes, Offset));

        // Undefined
        Assert.Equal((ShortEnum)(-1), nullableEnumConverter.Read(MinusBytes, Offset));
    }

    [Fact]
    public void WriteNullableShortEnumToBuffer()
    {
        var buffer = new byte[Length + Offset];

        // Null
        nullableEnumConverter.Write(buffer, Offset, null);
        Assert.Equal(EmptyBytes, buffer);
    }
}
