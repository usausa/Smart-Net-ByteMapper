namespace Smart.IO.ByteMapper.AspNetCore.Generator.Models;

// One reader/writer pair on a [ByteMapperEndpoint] class. A class declaring mappers for several
// entity/profile combinations yields one model per combination, disambiguated by NameSuffix.
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
    // Emit options
    bool GenerateArrayBinding,
    string RootNamespace,
    string NameSuffix);
