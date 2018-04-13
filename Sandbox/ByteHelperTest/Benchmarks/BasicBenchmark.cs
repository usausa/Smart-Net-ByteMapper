namespace ByteHelperTest.Benchmarks
{
    using BenchmarkDotNet.Attributes;

    [Config(typeof(BenchmarkConfig))]
    public class BasicBenchmark
    {
        private readonly byte[] buffer = new byte[20];

        [Benchmark]
        public void FillDefault()
        {
            buffer.Fill(0, buffer.Length, 0xFF);
        }

        [Benchmark]
        public void FillMemoryCopy()
        {
            buffer.FillUnsafe(0, buffer.Length, 0xFF);
        }
    }
}
