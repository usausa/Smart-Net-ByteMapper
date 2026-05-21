namespace Smart.IO.ByteMapper.Generator;

using Microsoft.CodeAnalysis;

internal static class Diagnostics
{
    public static DiagnosticDescriptor InvalidMethodDefinition { get; } = new(
        id: "SBM0001",
        title: "Method must be static partial",
        messageFormat: "Method must be static partial. method=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor InvalidMethodSignature { get; } = new(
        id: "SBM0002",
        title: "Unsupported method signature",
        messageFormat: "Method signature is not supported. method=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor MissingMapAttribute { get; } = new(
        id: "SBM0003",
        title: "Target type missing [Map] attribute",
        messageFormat: "Target type must have [Map] attribute or Profile must be specified. method=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor InvalidOffset { get; } = new(
        id: "SBM0004",
        title: "Offset or length is negative",
        messageFormat: "Offset or length must not be negative. method=[{0}], property=[{1}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor TypeMismatch { get; } = new(
        id: "SBM0005",
        title: "Attribute and property type mismatch",
        messageFormat: "Attribute and property type do not match. method=[{0}], property=[{1}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor RangeOverlap { get; } = new(
        id: "SBM0006",
        title: "Range overlap detected",
        messageFormat: "Range overlap detected. type=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor LayoutExceedsSize { get; } = new(
        id: "SBM0007",
        title: "Layout exceeds Map size",
        messageFormat: "Layout exceeds Map(size). type=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor UnsupportedBinaryType { get; } = new(
        id: "SBM0008",
        title: "Unsupported type for MapBinary",
        messageFormat: "Unsupported type for MapBinary. method=[{0}], property=[{1}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor InvalidEncodingOrCulture { get; } = new(
        id: "SBM0009",
        title: "Invalid Encoding or Culture name",
        messageFormat: "Invalid encoding or culture name. method=[{0}], property=[{1}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor ConverterContractMismatch { get; } = new(
        id: "SBM0010",
        title: "Custom Converter does not satisfy contract",
        messageFormat: "Custom Converter does not satisfy contract. method=[{0}], property=[{1}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor ProfilePropertyNotFound { get; } = new(
        id: "SBM0011",
        title: "Profile property not found in target",
        messageFormat: "Profile property not found in target type. method=[{0}], property=[{1}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor ProfilePropertyTypeMismatch { get; } = new(
        id: "SBM0012",
        title: "Profile property type mismatch",
        messageFormat: "Profile and target property type mismatch. method=[{0}], property=[{1}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor ProfileMissingMapAttribute { get; } = new(
        id: "SBM0013",
        title: "Profile type missing [Map] attribute",
        messageFormat: "Profile type must have [Map] attribute. method=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor TargetNotInstantiatable { get; } = new(
        id: "SBM0014",
        title: "Target type is not instantiatable",
        messageFormat: "Target type must have a public parameterless constructor for return-value Read method. method=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}
