namespace Smart.IO.ByteMapper;

using System;

using Smart.IO.ByteMapper.Converters;

// Member mapping attributes for the Fast converters. Mirrors the core Map... attributes: each converter
// is a triple of an abstract base (Map...AttributeBase), the sealed property form (Map...Attribute, for
// an entity's own [Map] layout) and the sealed member form (Map...MemberAttribute, for a [MapProfile]
// layout, taking the target member name first). The Fast numeric/date-time converters read and write
// Nullable<T>, so the mapped property must be the nullable type (int?, decimal?, DateTime?, ...).
// Fast コンバーター用のメンバーマッピング属性。Core の Map... 属性に倣い、各コンバーターを
// 抽象ベース (Map...AttributeBase)・sealed なプロパティ形式 (Map...Attribute、[Map] 用)・
// sealed なメンバー形式 (Map...MemberAttribute、[MapProfile] 用、先頭に対象メンバー名) の3点で定義する。
// Fast の数値/日時コンバーターは Nullable<T> を読み書きするため、対応プロパティは nullable 型
// (int?, decimal?, DateTime? ...) になる。

// MapFastInteger<T>
[ConverterSupportedTypes(typeof(short?), typeof(int?), typeof(long?))]
public abstract class MapFastIntegerAttributeBase<T> : ByteMapperPropertyAttribute<FastIntegerConverter<T>>
    where T : struct
{
    public int Length { get; }

    public Padding Padding { get; init; } = Padding.Left;

    public bool Zerofill { get; init; }

    public byte Filler { get; init; } = 0x20;

    protected MapFastIntegerAttributeBase(int offset, int length)
        : base(offset)
    {
        Length = length;
    }
}

public sealed class MapFastIntegerAttribute<T> : MapFastIntegerAttributeBase<T>
    where T : struct
{
    public MapFastIntegerAttribute(int offset, int length)
        : base(offset, length)
    {
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public sealed class MapFastIntegerMemberAttribute<T> : MapFastIntegerAttributeBase<T>
    where T : struct
{
    public string Member { get; }

    public MapFastIntegerMemberAttribute(string member, int offset, int length)
        : base(offset, length)
    {
        Member = member;
    }
}

// MapFastDecimal
[ConverterSupportedTypes(typeof(decimal?))]
public abstract class MapFastDecimalAttributeBase : ByteMapperPropertyAttribute<FastDecimalConverter>
{
    public int Length { get; }

    public byte Scale { get; init; }

    public int GroupingSize { get; init; }

    public Padding Padding { get; init; } = Padding.Left;

    public bool Zerofill { get; init; }

    public byte Filler { get; init; } = 0x20;

    protected MapFastDecimalAttributeBase(int offset, int length)
        : base(offset)
    {
        Length = length;
    }
}

public sealed class MapFastDecimalAttribute : MapFastDecimalAttributeBase
{
    public MapFastDecimalAttribute(int offset, int length)
        : base(offset, length)
    {
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public sealed class MapFastDecimalMemberAttribute : MapFastDecimalAttributeBase
{
    public string Member { get; }

    public MapFastDecimalMemberAttribute(string member, int offset, int length)
        : base(offset, length)
    {
        Member = member;
    }
}

// MapFastDateTime
[ConverterSupportedTypes(typeof(DateTime?))]
public abstract class MapFastDateTimeAttributeBase : ByteMapperPropertyAttribute<FastDateTimeConverter>
{
    public string Format { get; }

    public DateTimeKind Kind { get; init; } = DateTimeKind.Unspecified;

    public byte Filler { get; init; } = 0x20;

    protected MapFastDateTimeAttributeBase(int offset, string format)
        : base(offset)
    {
        Format = format;
    }
}

public sealed class MapFastDateTimeAttribute : MapFastDateTimeAttributeBase
{
    public MapFastDateTimeAttribute(int offset, string format)
        : base(offset, format)
    {
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public sealed class MapFastDateTimeMemberAttribute : MapFastDateTimeAttributeBase
{
    public string Member { get; }

    public MapFastDateTimeMemberAttribute(string member, int offset, string format)
        : base(offset, format)
    {
        Member = member;
    }
}

// MapFastDateTimeOffset
[ConverterSupportedTypes(typeof(DateTimeOffset?))]
public abstract class MapFastDateTimeOffsetAttributeBase : ByteMapperPropertyAttribute<FastDateTimeOffsetConverter>
{
    public string Format { get; }

    public DateTimeKind Kind { get; init; } = DateTimeKind.Unspecified;

    public byte Filler { get; init; } = 0x20;

    protected MapFastDateTimeOffsetAttributeBase(int offset, string format)
        : base(offset)
    {
        Format = format;
    }
}

public sealed class MapFastDateTimeOffsetAttribute : MapFastDateTimeOffsetAttributeBase
{
    public MapFastDateTimeOffsetAttribute(int offset, string format)
        : base(offset, format)
    {
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public sealed class MapFastDateTimeOffsetMemberAttribute : MapFastDateTimeOffsetAttributeBase
{
    public string Member { get; }

    public MapFastDateTimeOffsetMemberAttribute(string member, int offset, string format)
        : base(offset, format)
    {
        Member = member;
    }
}

// MapFastUnicode
[ConverterSupportedTypes(typeof(string))]
public abstract class MapFastUnicodeAttributeBase : ByteMapperPropertyAttribute<FastUnicodeConverter>
{
    public int Length { get; }

    public bool Trim { get; init; } = true;

    public Padding Padding { get; init; } = Padding.Right;

    public char Filler { get; init; } = ' ';

    protected MapFastUnicodeAttributeBase(int offset, int length)
        : base(offset)
    {
        Length = length;
    }
}

public sealed class MapFastUnicodeAttribute : MapFastUnicodeAttributeBase
{
    public MapFastUnicodeAttribute(int offset, int length)
        : base(offset, length)
    {
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public sealed class MapFastUnicodeMemberAttribute : MapFastUnicodeAttributeBase
{
    public string Member { get; }

    public MapFastUnicodeMemberAttribute(string member, int offset, int length)
        : base(offset, length)
    {
        Member = member;
    }
}
