namespace ByteHelperTest.Benchmarks
{
    using System;
    using System.Globalization;
    using System.Text;

    using BenchmarkDotNet.Attributes;

    [Config(typeof(BenchmarkConfig))]
    public class LongBenchmark
    {
        private static readonly byte[] Bytes8 = Encoding.ASCII.GetBytes("12345678");

        private static readonly long Value8 = 12345678;

        private static readonly byte[] Bytes19 = Encoding.ASCII.GetBytes("1234567890123456789");

        private static readonly long Value19 = 1234567890123456789;

        [Benchmark]
        public void ParseDefault8()
        {
            Int64.TryParse(Encoding.ASCII.GetString(Bytes8), NumberStyles.Integer, CultureInfo.InvariantCulture, out var _);
        }

        [Benchmark]
        public void ParseCustom8()
        {
            ByteHelper.TryParseInt64(Bytes8, 0, Bytes8.Length, out var _);
        }

        [Benchmark]
        public void ParseDefault19()
        {
            Int64.TryParse(Encoding.ASCII.GetString(Bytes19), NumberStyles.Integer, CultureInfo.InvariantCulture, out var _);
        }

        [Benchmark]
        public void ParseCustom19()
        {
            ByteHelper.TryParseInt64(Bytes19, 0, Bytes19.Length, out var _);
        }

        [Benchmark]
        public void FormatDefault8()
        {
            Encoding.ASCII.GetBytes(Value8.ToString("D8"));
        }

        [Benchmark]
        public void FormatCustom8()
        {
            var buffer = new byte[8];
            ByteHelper.FormatInt64(buffer, 0, buffer.Length, Value8, Padding.Left, true, 0x30);
        }

        [Benchmark]
        public void FormatDefault19()
        {
            Encoding.ASCII.GetBytes(Value19.ToString("D19"));
        }

        [Benchmark]
        public void FormatCustom19()
        {
            var buffer = new byte[19];
            ByteHelper.FormatInt64(buffer, 0, buffer.Length, Value19, Padding.Left, true, 0x30);
        }
    }
}
