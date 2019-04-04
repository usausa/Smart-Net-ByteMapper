namespace ByteHelperTest.Benchmarks
{
    //using System;
    //using System.Globalization;
    using System.Text;

    using BenchmarkDotNet.Attributes;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Ignore")]
    [Config(typeof(BenchmarkConfig))]
    public class DecimalBenchmark
    {
        //private static readonly byte[] Bytes8 = Encoding.ASCII.GetBytes("123456.78");

        //private static readonly byte[] Bytes19 = Encoding.ASCII.GetBytes("12345678901234567.89");

        private static readonly byte[] Bytes28 = Encoding.ASCII.GetBytes("12345678901234567890123456.78");

        // Parse

        // 8

        //[Benchmark]
        //public void ParseDefault8()
        //{
        //    Decimal.TryParse(Encoding.ASCII.GetString(Bytes8), NumberStyles.Any, CultureInfo.InvariantCulture, out var _);
        //}

        //[Benchmark]
        //public void ParseCustom8()
        //{
        //    ByteHelper.TryParseDecimal(Bytes8, 0, Bytes8.Length, out var _);
        //}

        //[Benchmark]
        //public void ParseCustom8X()
        //{
        //    ByteHelper2.TryParseDecimal(Bytes8, 0, Bytes8.Length, out var _);
        //}

        //[Benchmark]
        //public void ParseCustom8X2()
        //{
        //    ByteHelper2.TryParseDecimal2(Bytes8, 0, Bytes8.Length, out var _);
        //}

        //[Benchmark]
        //public void ParseCustomB8()
        //{
        //    ByteHelper.TryParseDecimal2(Bytes8, 0, Bytes8.Length, out var _);
        //}

        //[Benchmark]
        //public void ParseCustomC8()
        //{
        //    ByteHelper.TryParseDecimal3(Bytes8, 0, Bytes8.Length, out var _);
        //}

        // 19

        //[Benchmark]
        //public void ParseDefault19()
        //{
        //    Decimal.TryParse(Encoding.ASCII.GetString(Bytes19), NumberStyles.Any, CultureInfo.InvariantCulture, out var _);
        //}

        //[Benchmark]
        //public void ParseCustom19()
        //{
        //    ByteHelper.TryParseDecimal(Bytes19, 0, Bytes19.Length, out var _);
        //}

        //[Benchmark]
        //public void ParseCustom19X()
        //{
        //    ByteHelper2.TryParseDecimal(Bytes19, 0, Bytes19.Length, out var _);
        //}

        //[Benchmark]
        //public void ParseCustom19X2()
        //{
        //    ByteHelper2.TryParseDecimal2(Bytes19, 0, Bytes19.Length, out var _);
        //}

        //[Benchmark]
        //public void ParseCustomB19()
        //{
        //    ByteHelper.TryParseDecimal2(Bytes19, 0, Bytes19.Length, out var _);
        //}

        //[Benchmark]
        //public void ParseCustomC19()
        //{
        //    ByteHelper.TryParseDecimal3(Bytes19, 0, Bytes19.Length, out var _);
        //}

        // 28

        //[Benchmark]
        //public void ParseDefault28()
        //{
        //    Decimal.TryParse(Encoding.ASCII.GetString(Bytes28), NumberStyles.Any, CultureInfo.InvariantCulture, out var _);
        //}

        //[Benchmark]
        //public void ParseCustom28()
        //{
        //    ByteHelper.TryParseDecimal(Bytes28, 0, Bytes28.Length, out var _);
        //}

        [Benchmark]
        public void ParseCustom28X()
        {
            ByteHelper2.TryParseDecimal(Bytes28, 0, Bytes28.Length, (byte)' ', out var _);
        }

        //[Benchmark]
        //public void ParseCustomB28()
        //{
        //    // Not supported
        //}

        //[Benchmark]
        //public void ParseCustomC28()
        //{
        //    ByteHelper.TryParseDecimal3(Bytes28, 0, Bytes28.Length, out var _);
        //}
    }
}
