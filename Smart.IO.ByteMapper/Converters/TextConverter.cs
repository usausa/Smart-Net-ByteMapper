namespace Smart.IO.ByteMapper.Converters;

using System.Buffers;
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

        var maxByteCount = encoding.GetMaxByteCount(value.Length);
        if (maxByteCount <= ByteMapperHelpers.StackallocByteThreshold)
        {
            WriteCore(destination, value, stackalloc byte[maxByteCount]);
        }
        else
        {
            WriteWithPooledBuffer(destination, value, maxByteCount);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteCore(Span<byte> destination, string value, Span<byte> encoded)
    {
        var count = encoding.GetBytes(value, encoded);
        ByteMapperHelpers.CopyWithPadding(encoded[..count], destination, size, padding, filler);
    }

    // Keep the rare large-buffer path out of the inlined fast path (try/finally blocks inlining).
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void WriteWithPooledBuffer(Span<byte> destination, string value, int maxByteCount)
    {
        var encoded = ArrayPool<byte>.Shared.Rent(maxByteCount);
        try
        {
            WriteCore(destination, value, encoded.AsSpan(0, maxByteCount));
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(encoded);
        }
    }
}
#pragma warning restore IDE0032
