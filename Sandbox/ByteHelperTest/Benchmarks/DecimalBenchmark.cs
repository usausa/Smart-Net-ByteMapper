namespace ByteHelperTest.Benchmarks
{
    using System;
    using System.Globalization;
    using System.Text;

    using BenchmarkDotNet.Attributes;

    [Config(typeof(BenchmarkConfig))]
    public class DecimalBenchmark
    {
        private static readonly byte[] Bytes8 = Encoding.ASCII.GetBytes("123456.78");

        private static readonly byte[] Bytes19 = Encoding.ASCII.GetBytes("12345678901234567.89");

        private static readonly byte[] Bytes28 = Encoding.ASCII.GetBytes("12345678901234567890123456.78");

        private static readonly decimal Value8 = 123456.78m;

        private static readonly decimal Value19 = 12345678901234567.89m;

        private static readonly decimal Value28 = 12345678901234567890123456.78m;

        // Parse

        // 8

        [Benchmark]
        public void ParseDefault8()
        {
            Decimal.TryParse(Encoding.ASCII.GetString(Bytes8), NumberStyles.Any, CultureInfo.InvariantCulture, out var _);
        }

        [Benchmark]
        public void ParseCustom8()
        {
            ByteHelper.TryParseDecimal(Bytes8, 0, Bytes8.Length, out var _);
        }

        [Benchmark]
        public void ParseCustomB8()
        {
            ByteHelper.TryParseDecimal2(Bytes8, 0, Bytes8.Length, out var _);
        }

        // 19

        [Benchmark]
        public void ParseDefault19()
        {
            Decimal.TryParse(Encoding.ASCII.GetString(Bytes19), NumberStyles.Any, CultureInfo.InvariantCulture, out var _);
        }

        [Benchmark]
        public void ParseCustom19()
        {
            ByteHelper.TryParseDecimal(Bytes19, 0, Bytes19.Length, out var _);
        }

        [Benchmark]
        public void ParseCustomB19()
        {
            ByteHelper.TryParseDecimal2(Bytes19, 0, Bytes19.Length, out var _);
        }

        // 28

        [Benchmark]
        public void ParseDefault28()
        {
            Decimal.TryParse(Encoding.ASCII.GetString(Bytes28), NumberStyles.Any, CultureInfo.InvariantCulture, out var _);
        }

        [Benchmark]
        public void ParseCustom28()
        {
            ByteHelper.TryParseDecimal(Bytes28, 0, Bytes28.Length, out var _);
        }

        //[Benchmark]
        //public void ParseCustomB28()
        //{
        //     Not supported
        //}

        // Format

        [Benchmark]
        public void FormatDefault8()
        {
            Encoding.ASCII.GetBytes(Value8.ToString("0000000.000"));
        }

        //[Benchmark]
        //public void FormatCustom8()
        //{
        //    var buffer = new byte[11];
        //    ByteHelper.FormatDecimal(buffer, 0, buffer.Length, Value8, Padding.Left, true);
        //}

        //[Benchmark]
        //public void FormatDefault28()
        //{
        //    Encoding.ASCII.GetBytes(Value28.ToString("000000000000000000000000000.000"));
        //}


        //[Benchmark]
        //public void FormatCustom28()
        //{
        //    var buffer = new byte[31];
        //    ByteHelper.FormatDecimal(buffer, 0, buffer.Length, Value28, Padding.Left, true);
        //}
    }
}
