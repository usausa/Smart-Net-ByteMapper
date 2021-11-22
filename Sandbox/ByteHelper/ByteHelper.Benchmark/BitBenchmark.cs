namespace ByteHelper;

using BenchmarkDotNet.Attributes;

[Config(typeof(BenchmarkConfig))]
public class BitBenchmark
{
    private const float FloatValue = 0f;

    private const uint UIntValue = 0u;

    private const double DoubleValue = 0d;

    private const ulong ULongValue = 0u;

    [Benchmark]
    public uint FloatToUIntBit1()
    {
        return ByteHelper.FloatToUIntBit1(FloatValue);
    }

    [Benchmark]
    public uint FloatToUIntBit2()
    {
        return ByteHelper.FloatToUIntBit2(FloatValue);
    }

    [Benchmark]
    public float UIntToFloatBit1()
    {
        return ByteHelper.UIntToFloatBit1(UIntValue);
    }

    [Benchmark]
    public float UIntToFloatBit2()
    {
        return ByteHelper.FloatToUIntBit2(UIntValue);
    }

    [Benchmark]
    public ulong DoubleToULongBit1()
    {
        return ByteHelper.DoubleToULongBit1(DoubleValue);
    }

    [Benchmark]
    public ulong DoubleToULongBit2()
    {
        return ByteHelper.DoubleToULongBit2(DoubleValue);
    }

    [Benchmark]
    public double ULongToDoubleBit1()
    {
        return ByteHelper.ULongToDoubleBit1(ULongValue);
    }

    [Benchmark]
    public double ULongToDoubleBit2()
    {
        return ByteHelper.DoubleToULongBit2(ULongValue);
    }
}
