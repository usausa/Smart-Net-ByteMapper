namespace Smart.IO.ByteMapper.Converters;

using System.Runtime.CompilerServices;

public sealed class BytesConverter
{
    private readonly byte filler;

    public int Size { get; }

    public BytesConverter(int length, byte filler)
    {
        Size = length;
        this.filler = filler;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte[] Read(ReadOnlySpan<byte> source) => source[..Size].ToArray();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(Span<byte> destination, byte[] value)
    {
        if (value is null || value.Length == 0)
        {
            destination[..Size].Fill(filler);
            return;
        }
        ByteMapperHelpers.CopyWithPadding(value, destination, Size, Padding.Right, filler);
    }
}
