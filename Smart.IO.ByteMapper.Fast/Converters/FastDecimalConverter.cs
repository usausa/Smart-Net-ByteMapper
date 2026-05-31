namespace Smart.IO.ByteMapper.Converters;

using Smart.IO.ByteMapper.Helpers;

public sealed class FastDecimalConverter
{
    private readonly byte scale;
    private readonly int groupingSize;
    private readonly Padding padding;
    private readonly bool zerofill;
    private readonly byte filler;

    public int Size { get; }

    public FastDecimalConverter(int length, byte scale = 0, int groupingSize = 0, Padding padding = Padding.Left, bool zerofill = false, byte filler = 0x20)
    {
        Size = length;
        this.scale = scale;
        this.groupingSize = groupingSize <= 0 ? -1 : groupingSize;
        this.padding = padding;
        this.zerofill = zerofill;
        this.filler = filler;
    }

    public decimal? Read(ReadOnlySpan<byte> buffer)
    {
        return FastNumberByteHelper.TryParseDecimal(buffer, 0, Size, filler, out var result) ? result : null;
    }

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
