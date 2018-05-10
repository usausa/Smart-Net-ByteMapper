namespace ByteHelperTest.Benchmarks
{
    using BenchmarkDotNet.Attributes;

    [Config(typeof(BenchmarkConfig))]
    public class DecimalBenchmark2
    {
        private static readonly decimal Value8 = 123456.78m;

        private static readonly decimal Value19 = 12345678901234567.89m;

        private static readonly decimal Value28 = 12345678901234567890123456.78m;

        // Format

        [Benchmark]
        public void FormatDefaultA8()
        {
            var buffer = new byte[8];
            ByteHelper2.FormatDecimal0(buffer, 0, buffer.Length, Value8);
        }

        [Benchmark]
        public void FormatDefaultA19()
        {
            var buffer = new byte[19];
            ByteHelper2.FormatDecimal0(buffer, 0, buffer.Length, Value19);
        }

        [Benchmark]
        public void FormatDefaultA28()
        {
            var buffer = new byte[28];
            ByteHelper2.FormatDecimal0(buffer, 0, buffer.Length, Value28);
        }
    }
}
