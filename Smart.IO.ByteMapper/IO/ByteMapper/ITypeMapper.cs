namespace Smart.IO.ByteMapper;

public interface ITypeMapper
{
    Type TargetType { get; }

    int Size { get; }

    void FromByte(ReadOnlySpan<byte> buffer, object target);

    void ToByte(Span<byte> buffer, object target);
}

public interface ITypeMapper<in T> : ITypeMapper
{
    void FromByte(ReadOnlySpan<byte> buffer, T target);

    void ToByte(Span<byte> buffer, T target);
}
