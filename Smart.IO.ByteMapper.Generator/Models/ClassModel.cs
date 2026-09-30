namespace Smart.IO.ByteMapper.Generator.Models;

using SourceGenerateHelper;

internal sealed record ClassModel(
    string HintName,
    EquatableArray<MapperMethodModel> Methods);
