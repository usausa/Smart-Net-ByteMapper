namespace Smart.IO.ByteMapper.AspNetCore.Generator.Models;

internal sealed record EndPointModel(
    // Containing type
    string Namespace,
    string ClassName,
    // Mapper methods
    string ReaderMethodName,
    string WriterMethodName,
    // Mapped entity
    string EntityTypeFqn,
    string? ProfileTypeFqn,
    int Size,
    // Options
    bool GenerateArrayBinding,
    string RootNamespace,
    string NameSuffix);
