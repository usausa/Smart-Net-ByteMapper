namespace ByteHelperTest.Benchmarks
{
    using System;
    using System.Globalization;
    using System.Text;

    using BenchmarkDotNet.Attributes;

    [Config(typeof(BenchmarkConfig))]
    public class DateTimeBenchmark
    {
        private static readonly byte[] Bytes = Encoding.ASCII.GetBytes("20001231235959");

        [Benchmark]
        public void ParseDefault()
        {
            DateTime.TryParseExact(Encoding.ASCII.GetString(Bytes), "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var _);
        }

        [Benchmark]
        public void ParseCustom()
        {
            ByteHelper.TryParse(Bytes, 0, "yyyyMMddHHmmss", out var _);
        }
    }
}
