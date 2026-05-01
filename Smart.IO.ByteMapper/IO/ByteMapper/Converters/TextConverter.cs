namespace Smart.IO.ByteMapper.Converters;

using System.Text;

using Smart.IO.ByteMapper.Helpers;

internal sealed class TextConverter : IMapConverter
{
    private readonly int length;

    private readonly Encoding encoding;

    private readonly bool trim;

    private readonly Padding padding;

    private readonly byte filler;

    public TextConverter(
        int length,
        Encoding encoding,
        bool trim,
        Padding padding,
        byte filler)
    {
        this.length = length;
        this.encoding = encoding;
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

        return encoding.GetString(buffer.Slice(start, count));
    }

    public void Write(Span<byte> buffer, object value)
    {
        if (value is null)
        {
            BytesHelper.Fill(buffer[..length], filler);
        }
        else
        {
            var text = (string)value;
            if (encoding.GetByteCount(text) <= length)
            {
                var destination = buffer[..length];
                if (padding == Padding.Right)
                {
                    var written = encoding.GetBytes(text, destination);
                    if (written < length)
                    {
                        destination[written..].Fill(filler);
                    }
                }
                else
                {
                    var paddingLength = length - encoding.GetByteCount(text);
                    destination[..paddingLength].Fill(filler);
                    encoding.GetBytes(text, destination[paddingLength..]);
                }
            }
            else
            {
                BytesHelper.CopyBytes(encoding.GetBytes(text), buffer, length, padding, filler);
            }
        }
    }
}
