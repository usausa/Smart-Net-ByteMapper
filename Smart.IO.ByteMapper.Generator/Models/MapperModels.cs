namespace Smart.IO.ByteMapper.Generator.Models;

using Microsoft.CodeAnalysis;

using SourceGenerateHelper;

internal enum MapperKind
{
    Reader,
    Writer
}

internal enum MapperShape
{
    InPlace,      // void Read(ReadOnlySpan<byte> buffer, T target)
    NewInstance,  // T Read(ReadOnlySpan<byte> buffer)
    WriteSpan,    // void Write(T source, Span<byte> buffer)
    WriteAlloc    // byte[] Write(T source)
}

internal enum SizeKind
{
    Const,
    Instance,
    StaticMember  // static readonly Size on the converter type (e.g. BinaryConverter<T>.Size)
}

internal enum TypeMappingKind
{
    Constant,
    Filler
}

internal sealed record ConverterCallModel(
    string ConverterTypeFqn,
    string FieldName,
    EquatableArray<string> CtorArgExpressions,
    SizeKind SizeKind,
    int? ConstSize);

internal sealed record MemberMappingModel(
    string PropertyName,
    string PropertyTypeFqn,
    bool IsNullable,
    int Offset,
    int Size,
    int PropertyIndex,
    ConverterCallModel Converter);

internal sealed record TypeMappingModel(
    int Offset,
    int Size,
    TypeMappingKind Kind,
    EquatableArray<byte> Constant,
    byte Filler);

internal sealed record LayoutModel(
    int Size,
    byte Filler,
    byte NullFiller,
    bool UseDelimiter,
    EquatableArray<byte> Delimiter,
    bool AutoFiller,
    bool Validation);

internal sealed record MapperMethodModel(
    string Namespace,
    string ClassName,
    bool IsValueType,
    Accessibility MethodAccessibility,
    string MethodName,
    MapperKind Kind,
    MapperShape Shape,
    string TargetTypeFqn,
    string? ProfileTypeFqn,
    int MethodIndex,
    LayoutModel Layout,
    EquatableArray<MemberMappingModel> Members,
    EquatableArray<TypeMappingModel> TypeMappings,
    EquatableArray<DiagnosticInfo> Errors);
