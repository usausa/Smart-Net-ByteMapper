namespace Smart.IO.ByteMapper.Converters;

using Smart.IO.ByteMapper.Helpers;

internal sealed class UnicodeConverter : IMapConverter
{
    private readonly int length;

    private readonly bool trim;

    private readonly Padding padding;

    private readonly char filler;

    public UnicodeConverter(
        int length,
        bool trim,
        Padding padding,
        char filler)
    {
        this.length = length;
        this.trim = trim;
        this.padding = padding;
        this.filler = filler;
    }

    public object Read(ReadOnlySpan<byte> buffer)
    {
        return EncodingByteHelper.GetUnicodeString(buffer, 0, length, trim, padding, filler);
    }

    public void Write(Span<byte> buffer, object value)
    {
        if (value is null)
        {
            EncodingByteHelper.FillUnicode(buffer[..length], filler);
        }
        else
        {
            EncodingByteHelper.CopyUnicodeBytes((string)value, buffer, 0, length, padding, filler);
        }
    }
}
