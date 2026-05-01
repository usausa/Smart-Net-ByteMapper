namespace Smart.IO.ByteMapper.Converters;

internal sealed class ByteConverter : IMapConverter
{
    public static ByteConverter Default { get; } = new();

    public object Read(ReadOnlySpan<byte> buffer)
    {
        return buffer[0];
    }

    public void Write(Span<byte> buffer, object value)
    {
        buffer[0] = (byte)value;
    }
}
