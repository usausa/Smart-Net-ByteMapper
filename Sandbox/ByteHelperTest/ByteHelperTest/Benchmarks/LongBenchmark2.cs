namespace ByteHelperTest.Benchmarks;

using BenchmarkDotNet.Attributes;

[Config(typeof(BenchmarkConfig))]
public class LongBenchmark2
{
    private static readonly long Value8 = 12345678;

    private static readonly long Value19 = 1234567890123456789;

    [Params(Padding.Left, Padding.Right)]
    public Padding Padding { get; set; }

    [Params(true, false)]
    public bool ZeroFill { get; set; }

    [Benchmark]
    public void Format8()
    {
        var buffer = new byte[8];
        ByteHelper2.FormatInt64(buffer, 0, buffer.Length, Value8, Padding, ZeroFill, 0x30);
    }

    [Benchmark]
    public void Format19()
    {
        var buffer = new byte[19];
        ByteHelper2.FormatInt64(buffer, 0, buffer.Length, Value19, Padding, ZeroFill, 0x30);
    }
}
