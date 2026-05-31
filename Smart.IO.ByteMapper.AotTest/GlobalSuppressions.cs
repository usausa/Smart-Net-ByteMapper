[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Ignore")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "Ignore")]

// Apply assembly-wide ByteMapper defaults.
// EncodingName accepts any IANA charset registered on the platform
// (call Encoding.RegisterProvider(CodePagesEncodingProvider.Instance) for extended sets).
// Individual [MapText] / [MapNumberText] attributes can still override CodePage per-member.
[assembly: Smart.IO.ByteMapper.ByteMapperDefaults(
    EncodingName = "us-ascii",
    Filler = 0x20,
    TextPadding = Smart.IO.ByteMapper.Padding.Right,
    Endian = Smart.IO.ByteMapper.Endian.Big,
    Trim = true)]
