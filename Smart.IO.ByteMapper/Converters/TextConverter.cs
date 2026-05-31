namespace Smart.IO.ByteMapper.Converters;

using System.Runtime.CompilerServices;
using System.Text;

#pragma warning disable IDE0032
public sealed class TextConverter
{
    private readonly int size;
    private readonly bool trim;
    private readonly Padding padding;
    private readonly byte filler;
    private readonly Encoding encoding;

    public int Size => size;

    public TextConverter(int length, bool trim = true, Padding padding = Padding.Right, byte filler = 0x20, int codePage = CodePages.Ascii)
    {
        size = length;
        encoding = ByteMapperHelpers.ResolveEncoding(codePage);
        this.trim = trim;
        this.padding = padding;
        this.filler = filler;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string Read(ReadOnlySpan<byte> source)
    {
        var start = 0;
        var count = size;
        if (trim)
        {
            ByteMapperHelpers.TrimRange(source, ref start, ref count, padding, filler);
        }
        if (count == 0)
        {
            return string.Empty;
        }

        return encoding.GetString(source.Slice(start, count));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(Span<byte> destination, string value)
    {
        if (String.IsNullOrEmpty(value))
        {
            destination[..size].Fill(filler);
            return;
        }
        Span<byte> encoded = stackalloc byte[encoding.GetMaxByteCount(value.Length)];
        var count = encoding.GetBytes(value, encoded);
        ByteMapperHelpers.CopyWithPadding(encoded[..count], destination, size, padding, filler);
    }
}
#pragma warning restore IDE0032
