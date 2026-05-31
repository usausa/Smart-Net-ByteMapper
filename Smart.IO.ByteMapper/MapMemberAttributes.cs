namespace Smart.IO.ByteMapper;

using System;
using System.Globalization;

using Smart.IO.ByteMapper.Converters;

// MapBinary<T>
[ConverterSupportedTypes(typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal))]
public sealed class MapBinaryAttribute<T> : ByteMapperPropertyAttribute<BinaryConverter<T>>
    where T : unmanaged
{
    public Endian Endian { get; init; } = Endian.Big;

    public MapBinaryAttribute(int offset)
        : base(offset)
    {
    }
}

// MapByte
[ConverterSupportedTypes(typeof(byte))]
public sealed class MapByteAttribute : ByteMapperPropertyAttribute<ByteConverter>
{
    public MapByteAttribute(int offset)
        : base(offset)
    {
    }
}

// MapBytes
[ConverterSupportedTypes(typeof(byte[]))]
public sealed class MapBytesAttribute : ByteMapperPropertyAttribute<BytesConverter>
{
    public int Length { get; }

    public byte Filler { get; init; }

    public MapBytesAttribute(int offset, int length)
        : base(offset)
    {
        Length = length;
    }
}

// MapText
[ConverterSupportedTypes(typeof(string))]
public sealed class MapTextAttribute : ByteMapperPropertyAttribute<TextConverter>
{
    public int Length { get; }

    public int CodePage { get; init; } = CodePages.Ascii;

    public bool Trim { get; init; } = true;

    public Padding Padding { get; init; } = Padding.Right;

    public byte Filler { get; init; } = 0x20;

    public MapTextAttribute(int offset, int length)
        : base(offset)
    {
        Length = length;
    }
}

// MapBoolean
[ConverterSupportedTypes(typeof(bool), typeof(bool?))]
public sealed class MapBooleanAttribute : ByteMapperPropertyAttribute<BooleanConverter>
{
    public byte TrueValue { get; init; } = 0x31;

    public byte FalseValue { get; init; } = 0x30;

    public byte NullValue { get; init; } = 0x20;

    public MapBooleanAttribute(int offset)
        : base(offset)
    {
    }
}

// MapNumberText<T>
[ConverterSupportedTypes(typeof(short), typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal))]
public sealed class MapNumberTextAttribute<T> : ByteMapperPropertyAttribute<NumberTextConverter<T>>
    where T : struct
{
    public int Length { get; }

    public string? Format { get; init; }

    public int CodePage { get; init; } = CodePages.Ascii;

    public bool Trim { get; init; } = true;

    public Padding Padding { get; init; } = Padding.Left;

    public byte Filler { get; init; } = 0x20;

    public NumberStyles Style { get; init; } = NumberStyles.Integer;

    public Culture Culture { get; init; } = Culture.Invariant;

    public MapNumberTextAttribute(int offset, int length)
        : base(offset)
    {
        Length = length;
    }
}

// MapDateTimeText<T>
[ConverterSupportedTypes(typeof(DateTime), typeof(DateTime?), typeof(DateTimeOffset), typeof(DateTimeOffset?), typeof(DateOnly), typeof(DateOnly?), typeof(TimeOnly), typeof(TimeOnly?))]
public sealed class MapDateTimeTextAttribute<T> : ByteMapperPropertyAttribute<DateTimeTextConverter<T>>
    where T : struct
{
    public int Length { get; }

    public string Format { get; }

    public int CodePage { get; init; } = CodePages.Ascii;

    public byte Filler { get; init; } = 0x20;

    public DateTimeStyles Style { get; init; } = DateTimeStyles.None;

    public Culture Culture { get; init; } = Culture.Invariant;

    public MapDateTimeTextAttribute(int offset, int length, string format)
        : base(offset)
    {
        Length = length;
        Format = format;
    }
}
