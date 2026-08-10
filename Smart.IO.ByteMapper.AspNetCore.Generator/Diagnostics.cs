namespace Smart.IO.ByteMapper.AspNetCore.Generator;

using Microsoft.CodeAnalysis;

// Diagnostics for the AspNetCore endpoint binding generator.
// The SBM1xxx band is reserved for this package; the core ByteMapper generator owns SBM0xxx
// (the same split Smart.Data.Accessor uses between its core and builder generators).
internal static class Diagnostics
{
    public static DiagnosticDescriptor ReaderWithoutWriter { get; } = new(
        id: "SBM1001",
        title: "Reader has no matching writer",
        messageFormat: "An endpoint binding needs both a [ByteReader] and a [ByteWriter] for the same entity and profile. reader=[{0}], entity=[{1}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor UnknownEntitySize { get; } = new(
        id: "SBM1002",
        title: "Entity size cannot be resolved",
        messageFormat: "The entity has no [Map] or [MapProfile] declaring a positive size, so no binding can be generated. entity=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}
