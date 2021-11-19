namespace Smart.IO.ByteMapper.Mock;

using System;

public enum IntEnum
{
    Zero,
    One,
    Two
}

[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32", Justification = "Ignore")]
public enum LongEnum : long
{
    Zero,
    One,
    Two
}

[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32", Justification = "Ignore")]
public enum ShortEnum : short
{
    Zero,
    One,
    Two
}

public class Target
{
    // int

    public int IntProperty { get; set; }

    public int? NullableIntProperty { get; set; }

    public IntEnum IntEnumProperty { get; set; }

    public IntEnum? NullableIntEnumProperty { get; set; }

    // long

    public long LongProperty { get; set; }

    public long? NullableLongProperty { get; set; }

    public LongEnum LongEnumProperty { get; set; }

    public LongEnum? NullableLongEnumProperty { get; set; }

    // short

    public short ShortProperty { get; set; }

    public short? NullableShortProperty { get; set; }

    public ShortEnum ShortEnumProperty { get; set; }

    public ShortEnum? NullableShortEnumProperty { get; set; }

    // decimal

    public decimal DecimalProperty { get; set; }

    public decimal? NullableDecimalProperty { get; set; }

    // DateTime

    public DateTime DateTimeProperty { get; set; }

    public DateTime? NullableDateTimeProperty { get; set; }

    // DateTimeOffset

    public DateTimeOffset DateTimeOffsetProperty { get; set; }

    public DateTimeOffset? NullableDateTimeOffsetProperty { get; set; }
}
