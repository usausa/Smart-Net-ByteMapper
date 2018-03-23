namespace Smart.IO.Mapper.Benchmark
{
    using BenchmarkDotNet.Attributes;

    [Config(typeof(BenchmarkConfig))]
    public class Benchmark
    {
        [GlobalSetup]
        public void Setup()
        {
        }

        //[Benchmark]
        //public void ReadStringWide10()
        //{
        //}
    }
}
