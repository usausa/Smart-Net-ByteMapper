namespace ByteHelperTest.Benchmarks
{
    using System;
    using System.Globalization;
    using System.Text;

    using BenchmarkDotNet.Attributes;

    [Config(typeof(BenchmarkConfig))]
    public class IntegerBenchmark
    {
        private static readonly byte[] Bytes = Encoding.ASCII.GetBytes("12345678");

        //private static readonly int Value = 12345678;

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
    }
}
