namespace Smart.IO.ByteMapper.Options.Converters;

using Smart.IO.ByteMapper.Options.Helpers;

/// <summary>UTF-16LE（.NET 内部表現）文字列コンバーター。</summary>
public sealed class UnicodeConverter
{
    private readonly bool trim;
    private readonly Padding padding;
    private readonly char filler;

    /// <summary>フィールドのバイト長を取得します。</summary>
    public int Size { get; }

    /// <summary><see cref="UnicodeConverter"/> の新しいインスタンスを初期化します。</summary>
    public UnicodeConverter(int length, bool trim = true, Padding padding = Padding.Right, char filler = ' ')
    {
        Size = length;
        this.trim = trim;
        this.padding = padding;
        this.filler = filler;
    }

    /// <summary>バッファーから Unicode 文字列を読み取ります。</summary>
    public string Read(ReadOnlySpan<byte> buffer)
    {
        return EncodingByteHelper.GetUnicodeString(buffer, 0, Size, trim, padding, filler);
    }

    /// <summary>バッファーへ Unicode 文字列を書き込みます。</summary>
    public void Write(Span<byte> buffer, string? value)
    {
        if (String.IsNullOrEmpty(value))
        {
            EncodingByteHelper.FillUnicode(buffer[..Size], filler);
        }
        else
        {
            EncodingByteHelper.CopyUnicodeBytes(value, buffer, 0, Size, padding, filler);
        }
    }
}
