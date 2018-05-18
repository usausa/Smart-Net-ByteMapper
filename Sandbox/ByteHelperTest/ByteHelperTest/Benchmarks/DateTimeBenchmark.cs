namespace ByteHelperTest.Benchmarks
{
    using System;
    using System.Globalization;
    using System.Text;

    using BenchmarkDotNet.Attributes;

    [Config(typeof(BenchmarkConfig))]
    public class DateTimeBenchmark
    {
        private static readonly byte[] Bytes17 = Encoding.ASCII.GetBytes("20001231235959");

        private static readonly DateTime Date = new DateTime(2199, 12, 31, 23, 59, 59, 123);

        // Parse

        //[Benchmark]
        //public void ParseDefault17()
        //{
        //    DateTime.TryParseExact(Encoding.ASCII.GetString(Bytes), "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var _);
        //}

        //[Benchmark]
        //public void ParseCustom17()
        //{
        //    ByteHelper4.TryParseDateTime(Bytes, 0, "yyyyMMddHHmmss", DateTimeKind.Unspecified, out _);
        //}

        // Format

        //[Benchmark]
        //public void FormatDefault17()
        //{
        //    Encoding.ASCII.GetBytes(Date.ToString("yyyyMMddHHmmssfff"));
        //}

        [Benchmark]
        public void FormatCustom8()
        {
            var buffer = new byte[8];
            ByteHelper4.FormatDateTime(buffer, 0, "yyyyMMdd", Date);
        }

        [Benchmark]
        public void FormatCustom17()
        {
            var buffer = new byte[17];
            ByteHelper4.FormatDateTime(buffer, 0, "yyyyMMddHHmmssfff", Date);
        }
    }
}
