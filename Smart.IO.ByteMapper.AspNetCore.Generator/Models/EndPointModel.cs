namespace Smart.IO.ByteMapper.AspNetCore.Generator.Models;

using SourceGenerateHelper;

internal sealed record EndPointModel(
    // Containing type
    string Namespace,
    string ClassName,
    EquatableArray<string> ContainingTypes,
    string ClassFullName,
    string HintName,
    // Mapper methods
    string ReaderMethodName,
    string WriterMethodName,
    bool WriterReturnsArray,
    // Mapped entity
    string EntityTypeFqn,
    bool IsEntityPublic,
    string? ProfileTypeFqn,
    int Size,
    // Options
    bool GenerateArrayBinding,
    string RootNamespace,
    string NameSuffix);
