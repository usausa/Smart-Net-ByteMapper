namespace Smart.IO.ByteMapper.AspNetCore.Generator;

using Microsoft.CodeAnalysis;

internal static class Diagnostics
{
    public static DiagnosticDescriptor ReaderWithoutWriter { get; } = new(
        id: "SBM1001",
        title: "Reader has no matching writer",
        messageFormat: "No [ByteWriter] matches this [ByteReader]. reader=[{0}], entity=[{1}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor UnknownEntitySize { get; } = new(
        id: "SBM1002",
        title: "Entity size cannot be resolved",
        messageFormat: "Entity has no [Map] or [MapProfile] size. entity=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}
