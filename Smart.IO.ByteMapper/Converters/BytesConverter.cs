namespace Smart.IO.ByteMapper.Converters;

using System.Runtime.CompilerServices;

#pragma warning disable IDE0032
public sealed class BytesConverter
{
    private readonly int size;
    private readonly byte filler;

    public int Size => size;

    public BytesConverter(int length, byte filler)
    {
        size = length;
        this.filler = filler;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte[] Read(ReadOnlySpan<byte> source) => source[..size].ToArray();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(Span<byte> destination, byte[]? value)
    {
        if ((value is null) || (value.Length == 0))
        {
            destination[..size].Fill(filler);
            return;
        }
        ByteMapperHelpers.CopyWithPadding(value, destination, size, Padding.Right, filler);
    }
}
#pragma warning restore IDE0032
