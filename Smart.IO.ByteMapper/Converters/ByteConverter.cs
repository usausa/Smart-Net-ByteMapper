namespace Smart.IO.ByteMapper.Converters;

using System.Runtime.CompilerServices;

public sealed class ByteConverter
{
    public const int Size = 1;

    // Read/Write are instance methods to match the converter calling convention used by the source
    // generator (it instantiates the converter and invokes the members on the instance).
    // ByteConverter is stateless, so CA1822 (mark as static) is suppressed here.
#pragma warning disable CA1822
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte Read(ReadOnlySpan<byte> source) => source[0];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(Span<byte> destination, byte value) => destination[0] = value;
#pragma warning restore CA1822
}
