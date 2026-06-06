namespace Smart.IO.ByteMapper;

using System;
using System.Globalization;

using Smart.IO.ByteMapper.Converters;

// Member mapping attributes. Each converter is defined as a triple:
//   * an abstract base (Map...AttributeBase) that declares the converter binding, the supported
//     property types ([ConverterSupportedTypes]) and the converter options;
//   * the sealed property form (Map...Attribute) applied to a property for an entity's own layout ([Map]);
//   * the sealed member form (Map...MemberAttribute) applied to a class for a profile layout ([MapProfile]),
//     taking the target member name as its first argument (use nameof(Target.Member)).
// Both forms derive from the same base, so the source generator resolves the converter and its option
// defaults through the same base regardless of which form is used.
// メンバーマッピング属性。各コンバーターは次の3点セットで定義する:
//   * 抽象ベース (Map...AttributeBase): コンバーター束縛・対応プロパティ型 ([ConverterSupportedTypes])・
//     コンバーターオプションを宣言する。
//   * sealed なプロパティ形式 (Map...Attribute): エンティティ自身のレイアウト ([Map]) でプロパティに付与する。
//   * sealed なメンバー形式 (Map...MemberAttribute): プロファイルのレイアウト ([MapProfile]) でクラスに付与し、
//     先頭引数に対象メンバー名をとる (nameof(Target.Member) を使用)。
// 両形式は同じベースを継承するため、ジェネレーターはどちらの形式でも同じベースを通じてコンバーターと
// オプション既定値を解決する。

// MapBinary<T>
[ConverterSupportedTypes(typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal))]
public abstract class MapBinaryAttributeBase<T> : ByteMapperPropertyAttribute<BinaryConverter<T>>
    where T : unmanaged
{
    public Endian Endian { get; init; } = Endian.Big;

    protected MapBinaryAttributeBase(int offset)
        : base(offset)
    {
    }
}

public sealed class MapBinaryAttribute<T> : MapBinaryAttributeBase<T>
    where T : unmanaged
{
    public MapBinaryAttribute(int offset)
        : base(offset)
    {
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public sealed class MapBinaryMemberAttribute<T> : MapBinaryAttributeBase<T>
    where T : unmanaged
{
    public string Member { get; }

    public MapBinaryMemberAttribute(string member, int offset)
        : base(offset)
    {
        Member = member;
    }
}

// MapByte
[ConverterSupportedTypes(typeof(byte))]
public abstract class MapByteAttributeBase : ByteMapperPropertyAttribute<ByteConverter>
{
    protected MapByteAttributeBase(int offset)
        : base(offset)
    {
    }
}

public sealed class MapByteAttribute : MapByteAttributeBase
{
    public MapByteAttribute(int offset)
        : base(offset)
    {
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public sealed class MapByteMemberAttribute : MapByteAttributeBase
{
    public string Member { get; }

    public MapByteMemberAttribute(string member, int offset)
        : base(offset)
    {
        Member = member;
    }
}

// MapBytes
[ConverterSupportedTypes(typeof(byte[]))]
public abstract class MapBytesAttributeBase : ByteMapperPropertyAttribute<BytesConverter>
{
    public int Length { get; }

    public byte Filler { get; init; }

    protected MapBytesAttributeBase(int offset, int length)
        : base(offset)
    {
        Length = length;
    }
}

public sealed class MapBytesAttribute : MapBytesAttributeBase
{
    public MapBytesAttribute(int offset, int length)
        : base(offset, length)
    {
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public sealed class MapBytesMemberAttribute : MapBytesAttributeBase
{
    public string Member { get; }

    public MapBytesMemberAttribute(string member, int offset, int length)
        : base(offset, length)
    {
        Member = member;
    }
}

// MapText
[ConverterSupportedTypes(typeof(string))]
public abstract class MapTextAttributeBase : ByteMapperPropertyAttribute<TextConverter>
{
    public int Length { get; }

    public bool Trim { get; init; } = true;

    public Padding Padding { get; init; } = Padding.Right;

    public byte Filler { get; init; } = 0x20;

    public int CodePage { get; init; } = CodePages.Ascii;

    protected MapTextAttributeBase(int offset, int length)
        : base(offset)
    {
        Length = length;
    }
}

public sealed class MapTextAttribute : MapTextAttributeBase
{
    public MapTextAttribute(int offset, int length)
        : base(offset, length)
    {
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public sealed class MapTextMemberAttribute : MapTextAttributeBase
{
    public string Member { get; }

    public MapTextMemberAttribute(string member, int offset, int length)
        : base(offset, length)
    {
        Member = member;
    }
}

// MapBoolean
[ConverterSupportedTypes(typeof(bool), typeof(bool?))]
public abstract class MapBooleanAttributeBase : ByteMapperPropertyAttribute<BooleanConverter>
{
    public byte TrueValue { get; init; } = 0x31;

    public byte FalseValue { get; init; } = 0x30;

    public byte NullValue { get; init; } = 0x20;

    protected MapBooleanAttributeBase(int offset)
        : base(offset)
    {
    }
}

public sealed class MapBooleanAttribute : MapBooleanAttributeBase
{
    public MapBooleanAttribute(int offset)
        : base(offset)
    {
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public sealed class MapBooleanMemberAttribute : MapBooleanAttributeBase
{
    public string Member { get; }

    public MapBooleanMemberAttribute(string member, int offset)
        : base(offset)
    {
        Member = member;
    }
}

// MapNumberText<T>
[ConverterSupportedTypes(typeof(short), typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal))]
public abstract class MapNumberTextAttributeBase<T> : ByteMapperPropertyAttribute<NumberTextConverter<T>>
    where T : struct
{
    public int Length { get; }

    public bool Trim { get; init; } = true;

    public Padding Padding { get; init; } = Padding.Left;

    public byte Filler { get; init; } = 0x20;

    public int CodePage { get; init; } = CodePages.Ascii;

    public string? Format { get; init; }

    public NumberStyles Style { get; init; } = NumberStyles.Integer;

    public Culture Culture { get; init; } = Culture.Invariant;

    protected MapNumberTextAttributeBase(int offset, int length)
        : base(offset)
    {
        Length = length;
    }
}

public sealed class MapNumberTextAttribute<T> : MapNumberTextAttributeBase<T>
    where T : struct
{
    public MapNumberTextAttribute(int offset, int length)
        : base(offset, length)
    {
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public sealed class MapNumberTextMemberAttribute<T> : MapNumberTextAttributeBase<T>
    where T : struct
{
    public string Member { get; }

    public MapNumberTextMemberAttribute(string member, int offset, int length)
        : base(offset, length)
    {
        Member = member;
    }
}

// MapDateTimeText<T>
[ConverterSupportedTypes(typeof(DateTime), typeof(DateTime?), typeof(DateTimeOffset), typeof(DateTimeOffset?), typeof(DateOnly), typeof(DateOnly?), typeof(TimeOnly), typeof(TimeOnly?))]
public abstract class MapDateTimeTextAttributeBase<T> : ByteMapperPropertyAttribute<DateTimeTextConverter<T>>
    where T : struct
{
    public int Length { get; }

    public string Format { get; }

    public byte Filler { get; init; } = 0x20;

    public int CodePage { get; init; } = CodePages.Ascii;

    public DateTimeStyles Style { get; init; } = DateTimeStyles.None;

    public Culture Culture { get; init; } = Culture.Invariant;

    protected MapDateTimeTextAttributeBase(int offset, int length, string format)
        : base(offset)
    {
        Length = length;
        Format = format;
    }
}

public sealed class MapDateTimeTextAttribute<T> : MapDateTimeTextAttributeBase<T>
    where T : struct
{
    public MapDateTimeTextAttribute(int offset, int length, string format)
        : base(offset, length, format)
    {
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public sealed class MapDateTimeTextMemberAttribute<T> : MapDateTimeTextAttributeBase<T>
    where T : struct
{
    public string Member { get; }

    public MapDateTimeTextMemberAttribute(string member, int offset, int length, string format)
        : base(offset, length, format)
    {
        Member = member;
    }
}
