namespace Smart.IO.ByteMapper.Converters;

using System.Text;

// ASCII 固定長文字列コンバーター。
public sealed class AsciiConverter
{
    private readonly bool trim;
    private readonly Padding padding;
    private readonly byte filler;

    // フィールドのバイト長を取得します。
    public int Size { get; }

    // AsciiConverter の新しいインスタンスを初期化します。
    public AsciiConverter(int length, bool trim = true, Padding padding = Padding.Right, byte filler = 0x20)
    {
        Size = length;
        this.trim = trim;
        this.padding = padding;
        this.filler = filler;
    }

    // バッファーから ASCII 文字列を読み取ります。
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public unsafe string Read(ReadOnlySpan<byte> buffer)
    {
        var start = 0;
        var count = Size;
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

    // バッファーへ ASCII 文字列を書き込みます。
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public void Write(Span<byte> buffer, string? value)
    {
        if (String.IsNullOrEmpty(value))
        {
            buffer[..Size].Fill(filler);
            return;
        }

        var destination = buffer[..Size];
        if (value.Length >= Size)
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
            var pad = Size - value.Length;
            destination[..pad].Fill(filler);
            Ascii.FromUtf16(value, destination[pad..], out _);
        }
    }
}
