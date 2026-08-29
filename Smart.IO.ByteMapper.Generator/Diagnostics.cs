namespace Smart.IO.ByteMapper.Generator;

using Microsoft.CodeAnalysis;

internal static class Diagnostics
{
    public static DiagnosticDescriptor InvalidMethodDefinition { get; } = new(
        id: "SBM0001",
        title: "Invalid method definition",
        messageFormat: "[ByteReader]/[ByteWriter] method must be static partial. method=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor InvalidMethodSignature { get; } = new(
        id: "SBM0002",
        title: "Unsupported method signature",
        messageFormat: "[ByteReader]/[ByteWriter] method signature is not supported. method=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor MissingMapAttribute { get; } = new(
        id: "SBM0003",
        title: "Target type missing [Map] attribute",
        messageFormat: "Target type has no [Map] attribute. method=[{0}]",
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

    public static DiagnosticDescriptor RangeOverlap { get; } = new(
        id: "SBM0005",
        title: "Range overlap",
        messageFormat: "Layout ranges overlap. type=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor LayoutExceedsSize { get; } = new(
        id: "SBM0006",
        title: "Layout exceeds Map size",
        messageFormat: "Layout exceeds [Map] size. type=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor UnsupportedBinaryType { get; } = new(
        id: "SBM0007",
        title: "Unsupported MapBinary type",
        messageFormat: "Type is not supported for [MapBinary]. method=[{0}], property=[{1}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor ConverterContractMismatch { get; } = new(
        id: "SBM0008",
        title: "Converter contract mismatch",
        messageFormat: "Converter does not satisfy the contract. method=[{0}], property=[{1}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor ProfilePropertyNotFound { get; } = new(
        id: "SBM0009",
        title: "Profile property not found in target",
        messageFormat: "Property is not found in the target type. method=[{0}], property=[{1}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor ProfileMissingMapAttribute { get; } = new(
        id: "SBM0010",
        title: "Profile type missing [Map] attribute",
        messageFormat: "Profile type must have [Map] attribute. method=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor TargetNotInstantiatable { get; } = new(
        id: "SBM0011",
        title: "Target type is not instantiatable",
        messageFormat: "Target type has no parameterless constructor. method=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor MemberAttributeRequiresProfile { get; } = new(
        id: "SBM0012",
        title: "Member mapping requires [MapProfile]",
        messageFormat: "Member-mapping attributes are ignored under [Map]. type=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor PropertyMappingIgnoredUnderProfile { get; } = new(
        id: "SBM0013",
        title: "Property mapping is ignored",
        messageFormat: "Property-level mapping is ignored under [MapProfile]. type=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor ConflictingMapAttributes { get; } = new(
        id: "SBM0014",
        title: "Conflicting map attributes",
        messageFormat: "[Map] and [MapProfile] cannot be combined. type=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor UnknownMemberSize { get; } = new(
        id: "SBM0015",
        title: "Unknown member size",
        messageFormat: "Member size is not statically known. member=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);
}
