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
        public void FormatDefault8()
        {
            var buffer = new byte[8];
            ByteHelper2.FormatDecimal(buffer, 0, buffer.Length, Value8);
        }

        [Benchmark]
        public void FormatDefault19()
        {
            var buffer = new byte[19];
            ByteHelper2.FormatDecimal(buffer, 0, buffer.Length, Value19);
        }

        [Benchmark]
        public void FormatDefault28()
        {
            var buffer = new byte[28];
            ByteHelper2.FormatDecimal(buffer, 0, buffer.Length, Value28);
        }

        [Benchmark]
        public void FormatDefault8WithTable()
        {
            var buffer = new byte[8];
            ByteHelper2.FormatDecimal(buffer, 0, buffer.Length, Value8);
        }

        [Benchmark]
        public void FormatDefault19WithTable()
        {
            var buffer = new byte[19];
            ByteHelper2.FormatDecimal(buffer, 0, buffer.Length, Value19);
        }

        [Benchmark]
        public void FormatDefault28WithTable()
        {
            var buffer = new byte[28];
            ByteHelper2.FormatDecimal(buffer, 0, buffer.Length, Value28);
        }
    }
}
