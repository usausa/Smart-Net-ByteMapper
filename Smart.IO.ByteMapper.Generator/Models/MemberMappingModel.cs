namespace Smart.IO.ByteMapper.Generator.Models;

internal sealed record MemberMappingModel(
    string PropertyName,
    int Offset,
    int Size,
    ConverterCallModel Converter);
