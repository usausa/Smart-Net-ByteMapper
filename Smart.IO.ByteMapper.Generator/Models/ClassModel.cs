namespace Smart.IO.ByteMapper.Generator.Models;

using SourceGenerateHelper;

internal sealed record ClassModel(
    string Namespace,
    string ClassName,
    EquatableArray<MapperMethodModel> Methods);
