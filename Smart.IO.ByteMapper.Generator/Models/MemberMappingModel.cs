namespace Smart.IO.ByteMapper.Generator.Models;

// ReadValueOrDefault: the converter's Read returns Nullable<T> while the property is the
// non-nullable T, so the emitted read appends .GetValueOrDefault() (e.g. BooleanConverter on bool).
// ReadValueOrDefault: コンバーターの Read が Nullable<T> を返しプロパティが非 nullable の T の場合、
// 生成する読み取りに .GetValueOrDefault() を付与する（例: bool プロパティへの BooleanConverter）。
internal sealed record MemberMappingModel(
    string PropertyName,
    int Offset,
    int Size,
    ConverterCallModel Converter,
    bool ReadValueOrDefault = false);
