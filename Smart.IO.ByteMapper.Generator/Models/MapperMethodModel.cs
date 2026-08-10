namespace Smart.IO.ByteMapper.Generator.Models;

using Microsoft.CodeAnalysis;

using SourceGenerateHelper;

internal sealed record MapperMethodModel(
    // Containing type
    string Namespace,
    string ClassName,
    bool IsValueType,
    // Method signature
    Accessibility MethodAccessibility,
    string MethodName,
    // Mapping target and layout
    MapperShape Shape,
    string TargetTypeFqn,
    int Size,
    string BufferParamName,
    string TargetParamName,
    EquatableArray<MemberMappingModel> Members,
    EquatableArray<TypeMappingModel> TypeMappings,
    // Diagnostics
    EquatableArray<DiagnosticInfo> Diagnostics);
