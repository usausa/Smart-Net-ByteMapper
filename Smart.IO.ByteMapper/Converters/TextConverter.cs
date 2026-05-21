namespace Smart.IO.ByteMapper.Converters;

using System.Runtime.CompilerServices;
using System.Text;

public sealed class TextConverter
{
    private readonly Encoding encoding;
    private readonly bool trim;
    private readonly Padding padding;
    private readonly byte filler;

    public int Size { get; }

    public TextConverter(int length, int codePage = 20127, bool trim = true, Padding padding = Padding.Right, byte filler = 0x20)
    {
        Size = length;
        encoding = ResolveEncoding(codePage);
        this.trim = trim;
        this.padding = padding;
        this.filler = filler;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string Read(ReadOnlySpan<byte> source)
    {
        var start = 0;
        var size = Size;
        if (trim)
        {
            ByteMapperHelpers.TrimRange(source, ref start, ref size, padding, filler);
        }
        if (size == 0)
        {
            return string.Empty;
        }

        return encoding.GetString(source.Slice(start, size));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(Span<byte> destination, string value)
    {
        if (String.IsNullOrEmpty(value))
        {
            destination[..Size].Fill(filler);
            return;
        }
        Span<byte> encoded = stackalloc byte[encoding.GetMaxByteCount(value.Length)];
        var count = encoding.GetBytes(value, encoded);
        ByteMapperHelpers.CopyWithPadding(encoded[..count], destination, Size, padding, filler);
    }

    private static Encoding ResolveEncoding(int codePage) => codePage switch
    {
        20127 => Encoding.ASCII,
        65001 => Encoding.UTF8,
        _ => Encoding.GetEncoding(codePage)
    };
}
