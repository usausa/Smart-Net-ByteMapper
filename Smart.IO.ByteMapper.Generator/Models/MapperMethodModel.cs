namespace Smart.IO.ByteMapper.Generator.Models;

using Microsoft.CodeAnalysis;

using SourceGenerateHelper;

internal sealed record MapperMethodModel(
    string Namespace,
    string ClassName,
    bool IsValueType,
    Accessibility MethodAccessibility,
    string MethodName,
    MapperShape Shape,
    string TargetTypeFqn,
    int Size,
    string BufferParamName,
    string TargetParamName,
    EquatableArray<MemberMappingModel> Members,
    EquatableArray<TypeMappingModel> TypeMappings,
    EquatableArray<DiagnosticInfo> Errors);
