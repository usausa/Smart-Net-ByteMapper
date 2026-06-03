namespace Smart.IO.ByteMapper.AspNetCore.Generator.Models;

// Equatable model describing one (entity, profile) binding to emit for a [ByteMapperEndpoint] class.
// NameSuffix disambiguates factory method names and the generated file name when a class declares
// multiple bindings (multiple entities and/or profiles). It is empty for a single default binding.
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
