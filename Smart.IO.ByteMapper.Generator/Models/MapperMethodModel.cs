namespace Smart.IO.ByteMapper.Generator.Models;

using SourceGenerateHelper;

internal sealed record MapperMethodModel(
    // Containing type
    string Namespace,
    EquatableArray<string> ContainingTypes,
    string HintName,
    // Method signature
    string Signature,
    // Mapping target and layout
    MapperShape Shape,
    string TargetTypeFqn,
    int Size,
    string BufferParamName,
    string TargetParamName,
    EquatableArray<MemberMappingModel> Members,
    EquatableArray<TypeMappingModel> TypeMappings,
    // Diagnostics
    EquatableArray<DiagnosticInfo> Diagnostics,
    bool IsFallback = false,
    string TypeName = "",
    bool HasMethodImpl = false);
