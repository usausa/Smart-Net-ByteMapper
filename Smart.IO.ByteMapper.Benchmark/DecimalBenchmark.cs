namespace Smart.IO.ByteMapper.Benchmark;

using System.Text;

using BenchmarkDotNet.Attributes;

using Smart.IO.ByteMapper.Helpers;

#pragma warning disable CA1305
[Config(typeof(BenchmarkConfig))]
public sealed class DecimalBenchmark2
{
    private const int N = 1000;

    private const decimal Value8 = 123456.78m;

    private const decimal Value19 = 1234567890123456.789m;

    private const decimal Value28 = 123456789012345678901234.5678m;

    // Format

    [Benchmark(OperationsPerInvoke = N)]
    public void FormatDefault8()
    {
        var value = Value8;
        for (var i = 0; i < N; i++)
        {
            Encoding.ASCII.GetBytes(value.ToString("000000.00"));
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void FormatDefault19()
    {
        var value = Value19;
        for (var i = 0; i < N; i++)
        {
            Encoding.ASCII.GetBytes(value.ToString("000000000000000000.000"));
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void FormatDefault28()
    {
        var value = Value28;
        for (var i = 0; i < N; i++)
        {
            Encoding.ASCII.GetBytes(value.ToString("000000000000000000000000000.000"));
        }
    }

    // Custom

    [Benchmark(OperationsPerInvoke = N)]
    public void FormatCustom8()
    {
        var buffer = new byte[8];
        var value = Value8;
        for (var i = 0; i < N; i++)
        {
            NumberByteHelper.FormatDecimal(buffer, 0, buffer.Length, value, 2, -1, Padding.Left, false, 0x20);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void FormatCustom19()
    {
        var buffer = new byte[19];
        var value = Value19;
        for (var i = 0; i < N; i++)
        {
            NumberByteHelper.FormatDecimal(buffer, 0, buffer.Length, value, 3, -1, Padding.Left, false, 0x20);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void FormatCustom28()
    {
        var buffer = new byte[28];
        var value = Value28;
        for (var i = 0; i < N; i++)
        {
            NumberByteHelper.FormatDecimal(buffer, 0, buffer.Length, value, 4, -1, Padding.Left, false, 0x20);
        }
    }
}
