namespace Smart.IO.ByteMapper.Converters;

internal sealed class BooleanConverter : IMapConverter
{
    private readonly byte trueValue;

    private readonly byte falseValue;

    public BooleanConverter(byte trueValue, byte falseValue)
    {
        this.trueValue = trueValue;
        this.falseValue = falseValue;
    }

    public object Read(ReadOnlySpan<byte> buffer)
    {
        return buffer[0] == trueValue;
    }

    public void Write(Span<byte> buffer, object value)
    {
        buffer[0] = (bool)value ? trueValue : falseValue;
    }
}

internal sealed class NullableBooleanConverter : IMapConverter
{
    private readonly byte trueValue;

    private readonly byte falseValue;

    private readonly byte nullValue;

    public NullableBooleanConverter(byte trueValue, byte falseValue, byte nullValue)
    {
        this.trueValue = trueValue;
        this.falseValue = falseValue;
        this.nullValue = nullValue;
    }

    public object Read(ReadOnlySpan<byte> buffer)
    {
        var b = buffer[0];
        return b == trueValue ? true : b == nullValue ? null : false;
    }

    public void Write(Span<byte> buffer, object value)
    {
        var b = (bool?)value;
        buffer[0] = b.HasValue ? (b.Value ? trueValue : falseValue) : nullValue;
    }
}
