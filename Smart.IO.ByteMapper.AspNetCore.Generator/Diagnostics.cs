namespace Smart.IO.ByteMapper.AspNetCore.Generator;

using Microsoft.CodeAnalysis;

using SourceGenerateHelper;

internal static class Diagnostics
{
    public static DiagnosticDescriptor ReaderWithoutWriter { get; } = new(
        id: "SBM1001",
        title: "Reader has no matching writer",
        messageFormat: "No [ByteWriter] matches this [ByteReader]. reader=[{0}], entity=[{1}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: DiagnosticTags.NotSuppressible);

    public static DiagnosticDescriptor UnknownEntitySize { get; } = new(
        id: "SBM1002",
        title: "Entity size cannot be resolved",
        messageFormat: "Entity has no [Map] or [MapProfile] size. entity=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: DiagnosticTags.NotSuppressible);

    public static DiagnosticDescriptor WriterWithoutReader { get; } = new(
        id: "SBM1003",
        title: "Writer has no matching reader",
        messageFormat: "No [ByteReader] of the form void Read(ReadOnlySpan<byte>, T) matches this [ByteWriter]. writer=[{0}], entity=[{1}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: DiagnosticTags.NotSuppressible);

    public static DiagnosticDescriptor InvalidEndpointClass { get; } = new(
        id: "SBM1004",
        title: "Invalid endpoint class",
        messageFormat: "[ByteMapperEndpoint] class must be partial, and a nested one must be in partial non-generic types and not private or protected; generic and file-local classes are not supported. class=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: DiagnosticTags.NotSuppressible);

    public static DiagnosticDescriptor HintNameCollision { get; } = new(
        id: "SBM1006",
        title: "Endpoint class name differs only in case",
        messageFormat: "Endpoint class name differs only in case from another class, and its binding is not generated. class=[{0}], other=[{1}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: DiagnosticTags.NotSuppressible);

    public static DiagnosticDescriptor EntityNotCreatable { get; } = new(
        id: "SBM1005",
        title: "Entity cannot be created",
        messageFormat: "The binding cannot create the entity (it needs a parameterless constructor the endpoint class can call, [SetsRequiredMembers] for required members, and a non-abstract class or a struct). reader=[{0}], entity=[{1}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: DiagnosticTags.NotSuppressible);
}
