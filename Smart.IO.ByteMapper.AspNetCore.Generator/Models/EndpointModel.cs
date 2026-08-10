namespace Smart.IO.ByteMapper.AspNetCore.Generator.Models;

internal sealed record EndpointModel(
    string Namespace,
    string ClassName,
    string EntityTypeFqn,
    string ReaderMethodName,
    string WriterMethodName,
    int Size,
    string? ProfileTypeFqn,
    bool GenerateArrayBinding,
    string RootNamespace,
    string NameSuffix);
