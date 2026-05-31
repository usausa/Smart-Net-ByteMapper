namespace Smart.IO.ByteMapper.Generator.Models;

using SourceGenerateHelper;

internal sealed record ConverterCallModel(
    string ConverterTypeFqn,
    string FieldName,
    EquatableArray<string> CtorArgExpressions,
    SizeKind SizeKind,
    int? ConstSize);
