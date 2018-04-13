﻿namespace ByteHelperTest.Benchmarks
{
    using BenchmarkDotNet.Attributes;

    [Config(typeof(BenchmarkConfig))]
    public class BasicBenchmark
    {
        [Benchmark]
        public void FillDefault()
        {
        }

        [Benchmark]
        public void FillMemoryCopy()
        {
        }
    }
}
