namespace Smart.IO.ByteMapper.Fast.Converters;

using Smart.IO.ByteMapper.Fast.Helpers;

/// <summary>ASCII バイト表現の decimal コンバーター。</summary>
public sealed class DecimalConverter
{
    private readonly byte scale;
    private readonly int groupingSize;
    private readonly Padding padding;
    private readonly bool zerofill;
    private readonly byte filler;

    /// <summary>フィールドのバイト長を取得します。</summary>
    public int Size { get; }

    /// <summary><see cref="DecimalConverter"/> の新しいインスタンスを初期化します。</summary>
    public DecimalConverter(int length, byte scale = 0, int groupingSize = 0, Padding padding = Padding.Left, bool zerofill = false, byte filler = 0x20)
    {
        Size = length;
        this.scale = scale;
        this.groupingSize = groupingSize <= 0 ? -1 : groupingSize;
        this.padding = padding;
        this.zerofill = zerofill;
        this.filler = filler;
    }

    /// <summary>バッファーから decimal 値を読み取ります。</summary>
    public decimal? Read(ReadOnlySpan<byte> buffer)
    {
        return NumberByteHelper.TryParseDecimal(buffer, 0, Size, filler, out var result) ? result : null;
    }

    /// <summary>バッファーへ decimal 値を書き込みます。</summary>
    public void Write(Span<byte> buffer, decimal? value)
    {
        if (value is null)
        {
            buffer[..Size].Fill(filler);
        }
        else
        {
            NumberByteHelper.FormatDecimal(buffer, 0, Size, value.Value, scale, groupingSize, padding, zerofill, filler);
        }
    }
}
