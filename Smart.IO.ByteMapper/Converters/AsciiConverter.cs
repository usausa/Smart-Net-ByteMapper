namespace Smart.IO.ByteMapper.Converters;

using System.Text;

#pragma warning disable IDE0032
public sealed class AsciiConverter
{
    private readonly int size;
    private readonly bool trim;
    private readonly Padding padding;
    private readonly byte filler;

    public int Size => size;

    public AsciiConverter(int length, bool trim = true, Padding padding = Padding.Right, byte filler = 0x20)
    {
        size = length;
        this.trim = trim;
        this.padding = padding;
        this.filler = filler;
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public unsafe string Read(ReadOnlySpan<byte> buffer)
    {
        var start = 0;
        var count = size;
        if (trim)
        {
            ByteMapperHelpers.TrimRange(buffer, ref start, ref count, padding, filler);
        }

        if (count == 0)
        {
            return string.Empty;
        }

        var slice = buffer.Slice(start, count);
        var result = new string('\0', count);
        fixed (char* dest = result)
        {
            Ascii.ToUtf16(slice, new Span<char>(dest, count), out _);
        }
        return result;
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public void Write(Span<byte> buffer, string? value)
    {
        if (String.IsNullOrEmpty(value))
        {
            buffer[..size].Fill(filler);
            return;
        }

        var destination = buffer[..size];
        if (value.Length >= size)
        {
            Ascii.FromUtf16(value.AsSpan(0, Size), destination, out _);
        }
        else if (padding == Padding.Right)
        {
            Ascii.FromUtf16(value, destination[..value.Length], out _);
            destination[value.Length..].Fill(filler);
        }
        else
        {
            var pad = size - value.Length;
            destination[..pad].Fill(filler);
            Ascii.FromUtf16(value, destination[pad..], out _);
        }
    }
}
#pragma warning restore IDE0032
