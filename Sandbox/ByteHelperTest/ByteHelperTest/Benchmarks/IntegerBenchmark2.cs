namespace ByteHelperTest.Benchmarks;

using BenchmarkDotNet.Attributes;

[Config(typeof(BenchmarkConfig))]
public class IntegerBenchmark2
{
    private static readonly int Value0 = 12345678;

    private static readonly int Value8 = 12345678;

    [Params(Padding.Left, Padding.Right)]
    public Padding Padding { get; set; }

    [Params(true, false)]
    public bool ZeroFill { get; set; }

    [Benchmark]
    public void FormatOld0()
    {
        var buffer = new byte[8];
        ByteHelper2.FormatInt32(buffer, 0, buffer.Length, Value0, Padding, ZeroFill, 0x30);
    }

    [Benchmark]
    public void Format0()
    {
        var buffer = new byte[8];
        ByteHelper2.FormatInt32(buffer, 0, buffer.Length, Value0, Padding, ZeroFill, 0x30);
    }

    [Benchmark]
    public void FormatOld8()
    {
        var buffer = new byte[8];
        ByteHelper2.FormatInt32(buffer, 0, buffer.Length, Value8, Padding, ZeroFill, 0x30);
    }

    [Benchmark]
    public void Format8()
    {
        var buffer = new byte[8];
        ByteHelper2.FormatInt32(buffer, 0, buffer.Length, Value8, Padding, ZeroFill, 0x30);
    }
}
