namespace Smart.IO.ByteMapper.Converters;

using Smart.IO.ByteMapper.Helpers;

internal sealed class BytesConverter : IMapConverter
{
    private readonly int length;

    private readonly byte filler;

    public BytesConverter(int length, byte filler)
    {
        this.length = length;
        this.filler = filler;
    }

    public object Read(ReadOnlySpan<byte> buffer)
    {
        return buffer[..length].ToArray();
    }

    public void Write(Span<byte> buffer, object value)
    {
        if (value is null)
        {
            BytesHelper.Fill(buffer[..length], filler);
        }
        else
        {
            var bytes = (byte[])value;
            BytesHelper.CopyBytes(bytes, buffer, length, Padding.Right, filler);
        }
    }
}
