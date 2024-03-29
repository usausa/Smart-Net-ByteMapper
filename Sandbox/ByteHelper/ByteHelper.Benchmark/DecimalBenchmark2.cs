namespace ByteHelper;

using System.Text;

using BenchmarkDotNet.Attributes;

[Config(typeof(BenchmarkConfig))]
public class DecimalBenchmark2
{
    private const decimal Value8 = 123456.78m;

    private const decimal Value19 = 12345678901234567.89m;

    private const decimal Value28 = 12345678901234567890123456.78m;

    // Format

    [Benchmark]
    public void FormatDefault8()
    {
        Encoding.ASCII.GetBytes(Value8.ToString("000000.00"));
    }

    [Benchmark]
    public void FormatDefault19()
    {
        Encoding.ASCII.GetBytes(Value19.ToString("000000000000000000.000"));
    }

    [Benchmark]
    public void FormatDefault28()
    {
        Encoding.ASCII.GetBytes(Value28.ToString("000000000000000000000000000.000"));
    }

    // Custom

    [Benchmark]
    public void FormatCustom8()
    {
        var buffer = new byte[8];
        ByteHelper2.FormatDecimal(buffer, 0, buffer.Length, Value8, 2, -1, Padding.Left, false, 0x20);
    }

    [Benchmark]
    public void FormatCustom19()
    {
        var buffer = new byte[19];
        ByteHelper2.FormatDecimal(buffer, 0, buffer.Length, Value19, 2, -1, Padding.Left, false, 0x20);
    }

    [Benchmark]
    public void FormatCustom28()
    {
        var buffer = new byte[28];
        ByteHelper2.FormatDecimal(buffer, 0, buffer.Length, Value28, 3, -1, Padding.Left, false, 0x20);
    }
}
