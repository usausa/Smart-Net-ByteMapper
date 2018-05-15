namespace Smart.IO.ByteMapper.Benchmark
{
    using System.Text;

    using BenchmarkDotNet.Attributes;

    using Smart.IO.ByteMapper.Helpers;

    [Config(typeof(BenchmarkConfig))]
    public class DecimalBenchmark2
    {
        private const decimal Value8 = 123456.78m;

        private const decimal Value19 = 1234567890123456.789m;

        private const decimal Value28 = 123456789012345678901234.5678m;

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
            NumberHelper.FormatDecimal(buffer, 0, buffer.Length, Value8, 2, -1, Padding.Left, false, 0x20);
        }

        [Benchmark]
        public void FormatCustom19()
        {
            var buffer = new byte[19];
            NumberHelper.FormatDecimal(buffer, 0, buffer.Length, Value19, 3, -1, Padding.Left, false, 0x20);
        }

        [Benchmark]
        public void FormatCustom28()
        {
            var buffer = new byte[28];
            NumberHelper.FormatDecimal(buffer, 0, buffer.Length, Value28, 4, -1, Padding.Left, false, 0x20);
        }
    }
}
