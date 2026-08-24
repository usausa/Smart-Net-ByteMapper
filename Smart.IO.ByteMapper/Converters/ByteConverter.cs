namespace Smart.IO.ByteMapper.Converters;

using System.Runtime.CompilerServices;

public sealed class ByteConverter
{
    public const int Size = 1;

#pragma warning disable CA1822
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte Read(ReadOnlySpan<byte> source) => source[0];
#pragma warning restore CA1822

#pragma warning disable CA1822
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(Span<byte> destination, byte value) => destination[0] = value;
#pragma warning restore CA1822
}
