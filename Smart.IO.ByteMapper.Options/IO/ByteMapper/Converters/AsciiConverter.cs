namespace Smart.IO.ByteMapper.Converters;

using Smart.IO.ByteMapper.Helpers;

internal sealed class AsciiConverter : IMapConverter
{
    private readonly int length;

    private readonly bool trim;

    private readonly Padding padding;

    private readonly byte filler;

    public AsciiConverter(
        int length,
        bool trim,
        Padding padding,
        byte filler)
    {
        this.length = length;
        this.trim = trim;
        this.padding = padding;
        this.filler = filler;
    }

    public object Read(ReadOnlySpan<byte> buffer)
    {
        var start = 0;
        var count = length;
        if (trim)
        {
            BytesHelper.TrimRange(buffer, ref start, ref count, padding, filler);
        }

        return count == 0 ? string.Empty : EncodingByteHelper.GetAsciiString(buffer, start, count);
    }

    public void Write(Span<byte> buffer, object value)
    {
        if (value is null)
        {
            BytesHelper.Fill(buffer[..length], filler);
        }
        else
        {
            BytesHelper.CopyBytes(EncodingByteHelper.GetAsciiBytes((string)value), buffer, length, padding, filler);
        }
    }
}
