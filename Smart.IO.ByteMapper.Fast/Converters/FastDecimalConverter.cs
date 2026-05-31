namespace Smart.IO.ByteMapper.Converters;

using Smart.IO.ByteMapper.Helpers;

#pragma warning disable IDE0032
public sealed class FastDecimalConverter
{
    private readonly int size;
    private readonly byte scale;
    private readonly int groupingSize;
    private readonly Padding padding;
    private readonly bool zerofill;
    private readonly byte filler;

    public int Size => size;

    public FastDecimalConverter(int length, byte scale = 0, int groupingSize = 0, Padding padding = Padding.Left, bool zerofill = false, byte filler = 0x20)
    {
        size = length;
        this.scale = scale;
        this.groupingSize = groupingSize <= 0 ? -1 : groupingSize;
        this.padding = padding;
        this.zerofill = zerofill;
        this.filler = filler;
    }

    public decimal? Read(ReadOnlySpan<byte> buffer)
    {
        return FastNumberByteHelper.TryParseDecimal(buffer, 0, size, filler, out var result) ? result : null;
    }

    public void Write(Span<byte> buffer, decimal? value)
    {
        if (value is null)
        {
            buffer[..size].Fill(filler);
        }
        else
        {
            FastNumberByteHelper.FormatDecimal(buffer, 0, size, value.Value, scale, groupingSize, padding, zerofill, filler);
        }
    }
}
#pragma warning restore IDE0032
