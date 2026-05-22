namespace Smart.IO.ByteMapper.Fast.Converters;

using Smart.IO.ByteMapper.Fast.Helpers;

// ASCII バイト表現の decimal コンバーター。
public sealed class FastDecimalConverter
{
    private readonly byte scale;
    private readonly int groupingSize;
    private readonly Padding padding;
    private readonly bool zerofill;
    private readonly byte filler;

    // フィールドのバイト長を取得します。
    public int Size { get; }

    // FastDecimalConverter の新しいインスタンスを初期化します。
    public FastDecimalConverter(int length, byte scale = 0, int groupingSize = 0, Padding padding = Padding.Left, bool zerofill = false, byte filler = 0x20)
    {
        Size = length;
        this.scale = scale;
        this.groupingSize = groupingSize <= 0 ? -1 : groupingSize;
        this.padding = padding;
        this.zerofill = zerofill;
        this.filler = filler;
    }

    // バッファーから decimal 値を読み取ります。
    public decimal? Read(ReadOnlySpan<byte> buffer)
    {
        return FastNumberByteHelper.TryParseDecimal(buffer, 0, Size, filler, out var result) ? result : null;
    }

    // バッファーへ decimal 値を書き込みます。
    public void Write(Span<byte> buffer, decimal? value)
    {
        if (value is null)
        {
            buffer[..Size].Fill(filler);
        }
        else
        {
            FastNumberByteHelper.FormatDecimal(buffer, 0, Size, value.Value, scale, groupingSize, padding, zerofill, filler);
        }
    }
}
