namespace ByteHelperTest.Benchmarks
{
    using BenchmarkDotNet.Attributes;

    [Config(typeof(BenchmarkConfig))]
    public class DivBenchmark
    {
        private const int Value = 123456789;

        [Benchmark]
        public int Div10()
        {
            var value = Value;
            for (var i = 0; i < 8; i++)
            {
                value = value / 10;
            }

            return value;
        }

        [Benchmark]
        public int FastDiv10()
        {
            var value = Value;
            for (var i = 0; i < 8; i++)
            {
                value = ByteHelper.Div10Signed(value);
            }

            return value;
        }
    }
}
