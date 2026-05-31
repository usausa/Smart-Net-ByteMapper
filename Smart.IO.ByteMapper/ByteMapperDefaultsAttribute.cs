namespace Smart.IO.ByteMapper;

using System;

/// <summary>
/// Assembly-level defaults for ByteMapper converters.
/// Apply this attribute once per assembly to configure encoding and fill behavior
/// for all mappers defined in that assembly.
/// </summary>
/// <remarks>
/// <para>
/// <b>EncodingName</b> specifies the IANA character-set name (e.g. <c>"shift_jis"</c>,
/// <c>"utf-8"</c>, <c>"us-ascii"</c>) used as the default code page for text converters
/// (<see cref="Converters.TextConverter"/>, <see cref="Converters.NumberTextConverter"/>,
/// <see cref="Converters.DateTimeTextConverter"/>).  Internally the value is passed to
/// <see cref="System.Text.Encoding.GetEncoding(string)"/>, so any encoding name
/// registered on the platform is valid.  On .NET the full set is available after
/// calling <c>Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)</c>.
/// </para>
/// <para>
/// <b>Current consumption:</b> This attribute is intentionally left for runtime
/// frameworks or custom mapper factories to read; the source generator does <em>not</em>
/// currently read it.  Individual attributes override their own defaults via the
/// <c>CodePage</c> named argument instead.
/// </para>
/// <example>
/// <code>
/// // AssemblyInfo.cs (or any .cs file in the assembly)
/// [assembly: ByteMapperDefaults(
///     EncodingName = "shift_jis",
///     Filler       = 0x20,
///     TextPadding  = Padding.Right,
///     Endian       = Endian.Big,
///     Trim         = true)]
/// </code>
/// </example>
/// </remarks>
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class ByteMapperDefaultsAttribute : Attribute
{
    /// <summary>IANA character-set name used as the default encoding (e.g. <c>"shift_jis"</c>, <c>"utf-8"</c>).</summary>
    public string EncodingName { get; set; } = "us-ascii";

    /// <summary>Default fill byte for binary/byte-array converters.</summary>
    public byte Filler { get; set; } = 0x20;

    /// <summary>Default fill byte for text converters.</summary>
    public byte TextFiller { get; set; } = 0x20;

    /// <summary>Default padding direction for text converters.</summary>
    public Padding TextPadding { get; set; } = Padding.Right;

    /// <summary>Default byte order for binary numeric converters.</summary>
    public Endian Endian { get; set; } = Endian.Big;

    /// <summary>Byte value written/expected for <c>true</c> in boolean converters.</summary>
    public byte TrueValue { get; set; } = 0x31;

    /// <summary>Byte value written/expected for <c>false</c> in boolean converters.</summary>
    public byte FalseValue { get; set; } = 0x30;

    /// <summary>Optional record delimiter appended after each record (e.g. <c>new byte[] { 0x0D, 0x0A }</c>).</summary>
    public byte[]? Delimiter { get; set; }

    /// <summary>When <c>true</c>, strip filler bytes from text on read.</summary>
    public bool Trim { get; set; } = true;
}
