namespace ByteHelperTest.Benchmarks
{
    using BenchmarkDotNet.Attributes;

    [Config(typeof(BenchmarkConfig))]
    public class ReverseBenchmark
    {
        private readonly byte[] buffer4 = new byte[4];

        private readonly byte[] buffer8 = new byte[8];

        private readonly byte[] buffer16 = new byte[16];

        [Benchmark]
        public void Reverse4()
        {
            ByteHelper.Reverse(buffer4, 0, buffer4.Length);
        }

        [Benchmark]
        public void ReverseUnsafe4()
        {
            ByteHelper.ReverseUnsafe(buffer4, 0, buffer4.Length);
        }

        [Benchmark]
        public void Reverse8()
        {
            ByteHelper.Reverse(buffer8, 0, buffer8.Length);
        }

        [Benchmark]
        public void ReverseUnsafe8()
        {
            ByteHelper.ReverseUnsafe(buffer8, 0, buffer8.Length);
        }

        [Benchmark]
        public void Reverse16()
        {
            ByteHelper.Reverse(buffer16, 0, buffer16.Length);
        }

        [Benchmark]
        public void ReverseUnsafe16()
        {
            ByteHelper.ReverseUnsafe(buffer16, 0, buffer16.Length);
        }
    }
}
