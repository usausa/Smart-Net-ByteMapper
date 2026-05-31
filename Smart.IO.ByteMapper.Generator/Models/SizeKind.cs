namespace Smart.IO.ByteMapper.Generator.Models;

internal enum SizeKind
{
    Const,
    Instance,
    StaticMember // static readonly Size on the converter type (e.g. BinaryConverter<T>.Size)
}
