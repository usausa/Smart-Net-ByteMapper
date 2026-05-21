namespace Smart.IO.ByteMapper;

using System;
using System.Globalization;

using Smart.IO.ByteMapper.Converters;

// MapBinary<T>
public sealed class MapBinaryAttribute<T> : ByteMapperConverterAttribute<BinaryConverter<T>>
    where T : unmanaged
{
    public Endian Endian { get; init; } = Endian.Big;

    public MapBinaryAttribute(int offset)
        : base(offset)
    {
    }
}

// MapByte
public sealed class MapByteAttribute : ByteMapperConverterAttribute<ByteConverter>
{
    public MapByteAttribute(int offset)
        : base(offset)
    {
    }
}

// MapBytes
public sealed class MapBytesAttribute : ByteMapperConverterAttribute<BytesConverter>
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
public sealed class MapTextAttribute : ByteMapperConverterAttribute<TextConverter>
{
    public int Length { get; }

    public int CodePage { get; init; } = 20127;

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
public sealed class MapBooleanAttribute : ByteMapperConverterAttribute<BooleanConverter>
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
public sealed class MapNumberTextAttribute<T> : ByteMapperConverterAttribute<NumberTextConverter<T>>
{
    public int Length { get; }

    public string? Format { get; init; }

    public int CodePage { get; init; } = 20127;

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
public sealed class MapDateTimeTextAttribute<T> : ByteMapperConverterAttribute<DateTimeTextConverter<T>>
{
    public int Length { get; }

    public string Format { get; }

    public int CodePage { get; init; } = 20127;

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

// MapArray<TElementAttribute>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class MapArrayAttribute<TElementAttribute> : ByteMapperPropertyAttribute
    where TElementAttribute : ByteMapperPropertyAttribute, new()
{
    public int Count { get; }

    public byte Filler { get; init; }

    public MapArrayAttribute(int offset, int count)
        : base(offset)
    {
        Count = count;
    }
}
