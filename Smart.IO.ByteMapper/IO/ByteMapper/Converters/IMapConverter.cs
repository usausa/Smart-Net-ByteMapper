namespace Smart.IO.ByteMapper.Converters;

public interface IMapConverter
{
    object Read(ReadOnlySpan<byte> buffer);

    void Write(Span<byte> buffer, object value);
}
