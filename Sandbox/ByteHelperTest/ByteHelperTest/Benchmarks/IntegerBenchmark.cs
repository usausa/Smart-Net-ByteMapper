namespace ByteHelperTest.Benchmarks;

using System;
using System.Globalization;
using System.Text;

using BenchmarkDotNet.Attributes;

[Config(typeof(BenchmarkConfig))]
public class IntegerBenchmark
{
    private static readonly byte[] Bytes = Encoding.ASCII.GetBytes("12345678");

    private static readonly int Value = 12345678;

    [Benchmark]
    public void ParseDefault()
    {
        Int32.TryParse(Encoding.ASCII.GetString(Bytes), NumberStyles.Integer, CultureInfo.InvariantCulture, out var _);
    }

    [Benchmark]
    public void ParseCustom()
    {
        ByteHelper.TryParseInt32(Bytes, 0, Bytes.Length, out var _);
    }

    [Benchmark]
    public void FormatDefault()
    {
        Encoding.ASCII.GetBytes(Value.ToString("D8"));
    }

    [Benchmark]
    public void FormatCustom()
    {
        var buffer = new byte[8];
        ByteHelper.FormatInt32(buffer, 0, buffer.Length, Value, Padding.Left, true);
    }
}
