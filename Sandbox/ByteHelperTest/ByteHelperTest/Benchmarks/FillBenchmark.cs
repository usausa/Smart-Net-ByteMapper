namespace ByteHelperTest.Benchmarks
{
    using BenchmarkDotNet.Attributes;

    [Config(typeof(BenchmarkConfig))]
    public class FillBenchmark
    {
        private readonly byte[] buffer32 = new byte[32];

        private readonly byte[] buffer64 = new byte[64];

        [Benchmark]
        public void Fill32()
        {
            buffer32.Fill(0, buffer32.Length, 0xFF);
        }

        [Benchmark]
        public void FillUnsafe32()
        {
            buffer32.FillUnsafe(0, buffer32.Length, 0xFF);
        }

        [Benchmark]
        public void FillMemoryCopy32()
        {
            buffer32.FillMemoryCopy(0, buffer32.Length, 0xFF);
        }

        [Benchmark]
        public void Fill64()
        {
            buffer64.Fill(0, buffer64.Length, 0xFF);
        }

        [Benchmark]
        public void FillUnsafe64()
        {
            buffer64.FillUnsafe(0, buffer64.Length, 0xFF);
        }

        [Benchmark]
        public void FillMemoryCopy64()
        {
            buffer64.FillMemoryCopy(0, buffer64.Length, 0xFF);
        }
    }
}
