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

        private static readonly DateTime Date = new DateTime(2199, 12, 31, 23, 59, 59, 123);

        [Benchmark]
        public void ParseDefault()
        {
            DateTime.TryParseExact(Encoding.ASCII.GetString(Bytes), "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var _);
        }

        [Benchmark]
        public void ParseCustom()
        {
            ByteHelper.TryParseDateTime(Bytes, 0, "yyyyMMddHHmmss", out var _);
        }

        [Benchmark]
        public void FormatDefault()
        {
            Encoding.ASCII.GetBytes(Date.ToString("yyyyMMddHHmmssfff"));
        }

        [Benchmark]
        public void FormatCustom()
        {
            var buffer = new byte[17];
            ByteHelper.FormatDateTime(buffer, 0, "yyyyMMddHHmmssfff", Date);
        }
    }
}
