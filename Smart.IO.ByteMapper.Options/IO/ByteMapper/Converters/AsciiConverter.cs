namespace Smart.IO.ByteMapper.Converters;

using System.Text;

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
            return;
        }

        var text = (string)value;
        var destination = buffer[..length];
        if (text.Length >= length)
        {
            Ascii.FromUtf16(text.AsSpan(0, length), destination, out _);
        }
        else if (padding == Padding.Right)
        {
            Ascii.FromUtf16(text, destination[..text.Length], out _);
            if (text.Length < length)
            {
                destination[text.Length..].Fill(filler);
            }
        }
        else
        {
            var pad = length - text.Length;
            destination[..pad].Fill(filler);
            Ascii.FromUtf16(text, destination[pad..], out _);
        }
    }
}
