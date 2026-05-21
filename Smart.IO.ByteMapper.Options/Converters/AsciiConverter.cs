namespace Smart.IO.ByteMapper.Options.Converters;

using System.Text;

using Smart.IO.ByteMapper.Options.Helpers;

/// <summary>ASCII 固定長文字列コンバーター。</summary>
public sealed class AsciiConverter
{
    private readonly bool trim;
    private readonly Padding padding;
    private readonly byte filler;

    /// <summary>フィールドのバイト長を取得します。</summary>
    public int Size { get; }

    /// <summary><see cref="AsciiConverter"/> の新しいインスタンスを初期化します。</summary>
    public AsciiConverter(int length, bool trim = true, Padding padding = Padding.Right, byte filler = 0x20)
    {
        Size = length;
        this.trim = trim;
        this.padding = padding;
        this.filler = filler;
    }

    /// <summary>バッファーから ASCII 文字列を読み取ります。</summary>
    public string Read(ReadOnlySpan<byte> buffer)
    {
        var start = 0;
        var count = Size;
        if (trim)
        {
            ByteMapperHelpers.TrimRange(buffer, ref start, ref count, padding, filler);
        }

        return count == 0 ? string.Empty : EncodingByteHelper.GetAsciiString(buffer, start, count);
    }

    /// <summary>バッファーへ ASCII 文字列を書き込みます。</summary>
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
