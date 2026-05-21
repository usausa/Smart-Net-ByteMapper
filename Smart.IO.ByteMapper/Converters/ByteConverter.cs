namespace Smart.IO.ByteMapper.Converters;

using System.Runtime.CompilerServices;

public sealed class ByteConverter
{
    public static readonly ByteConverter Instance = new();

    public const int Size = 1;

    public ByteConverter()
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte Read(ReadOnlySpan<byte> source) => source[0];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(Span<byte> destination, byte value) => destination[0] = value;
}
