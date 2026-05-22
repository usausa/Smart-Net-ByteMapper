namespace Smart.IO.ByteMapper;

using System;

[AttributeUsage(AttributeTargets.Assembly)]
public sealed class ByteMapperDefaultsAttribute : Attribute
{
    public string EncodingName { get; set; } = "us-ascii";

    public byte Filler { get; set; } = 0x20;

    public byte TextFiller { get; set; } = 0x20;

    public Padding TextPadding { get; set; } = Padding.Right;

    public Endian Endian { get; set; } = Endian.Big;

    public byte TrueValue { get; set; } = 0x31;

    public byte FalseValue { get; set; } = 0x30;

    public byte[]? Delimiter { get; set; }

    public bool Trim { get; set; } = true;
}
