namespace Smart.IO.ByteMapper.AspNetCore.Generator.Models;

// Equatable model describing one (entity, profile) binding to emit for a [ByteMapperEndpoint] class.
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
    string FactoryMethodName,
    string ArrayFactoryMethodName);
