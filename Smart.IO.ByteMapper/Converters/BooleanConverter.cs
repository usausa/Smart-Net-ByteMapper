namespace Smart.IO.ByteMapper.Converters;

using System.Runtime.CompilerServices;

public sealed class BooleanConverter
{
    private readonly byte trueValue;
    private readonly byte falseValue;
    private readonly byte nullValue;

    public const int Size = 1;

    public BooleanConverter(byte trueValue = 0x31, byte falseValue = 0x30, byte nullValue = 0x20)
    {
        this.trueValue = trueValue;
        this.falseValue = falseValue;
        this.nullValue = nullValue;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool? Read(ReadOnlySpan<byte> source)
    {
        var b = source[0];
        if (b == trueValue) return true;
        if (b == falseValue) return false;
        return null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(Span<byte> destination, bool? value)
    {
        destination[0] = value.HasValue ? (value.Value ? trueValue : falseValue) : nullValue;
    }
}
