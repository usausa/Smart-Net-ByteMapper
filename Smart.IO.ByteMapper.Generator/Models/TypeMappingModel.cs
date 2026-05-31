namespace Smart.IO.ByteMapper.Generator.Models;

using SourceGenerateHelper;

internal sealed record TypeMappingModel(
    int Offset,
    int Size,
    TypeMappingKind Kind,
    EquatableArray<byte> Constant,
    byte Filler);
