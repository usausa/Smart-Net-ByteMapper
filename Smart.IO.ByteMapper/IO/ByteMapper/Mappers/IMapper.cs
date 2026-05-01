namespace Smart.IO.ByteMapper.Mappers;

public interface IMapper
{
    bool CanRead { get; }

    bool CanWrite { get; }

    void Read(ReadOnlySpan<byte> buffer, object target);

    void Write(Span<byte> buffer, object target);
}
