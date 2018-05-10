namespace NumberHelperTest
{
    using System;

    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Configs;
    using BenchmarkDotNet.Diagnosers;
    using BenchmarkDotNet.Exporters;
    using BenchmarkDotNet.Jobs;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args)
        {
            var b = new Benchmark();
            b.Setup();
            b.ULongUseTable();

            BenchmarkRunner.Run<Benchmark>();
        }
    }

    public class BenchmarkConfig : ManualConfig
    {
        public BenchmarkConfig()
        {
            Add(MarkdownExporter.Default, MarkdownExporter.GitHub);
            Add(MemoryDiagnoser.Default);
            Add(Job.ShortRun);
        }
    }

    [Config(typeof(BenchmarkConfig))]
    public class Benchmark
    {
        private static readonly byte[] Table = new byte[10000 * 4];

        private readonly byte[] work = new byte[20];

        [GlobalSetup]
        public void Setup()
        {
            for (var i = 0; i < 10000; i++)
            {
                var offset = i * 4;
                var value = i;
                Table[offset++] = (byte)(value % 10);
                value /= 10;
                Table[offset++] = (byte)(value % 10);
                value /= 10;
                Table[offset++] = (byte)(value % 10);
                value /= 10;
                Table[offset] = (byte)(value % 10);
            }
        }

        // Core 2.0 : 275s
        // Core 2.1 :  40s
        [Benchmark]
        public void ULongDivMode10()
        {
            DivMod10(work, UInt64.MaxValue);
        }

        private static void DivMod10(byte[] buffer, ulong value)
        {
            var index = 0;
            while (value > 0)
            {
                buffer[index++] = (byte)(value % 10);
                value = value / 10;
            }
        }

        // Core 2.0 : 70s
        // Core 2.1 : 20s
        [Benchmark]
        public void ULongUseTable()
        {
            UseTable(work, UInt64.MaxValue);
        }

        private static unsafe void UseTable(byte[] buffer, ulong value)
        {
            fixed (byte* pDst = &buffer[0])
            fixed (byte* pSrc = &Table[0])
            {
                // 1
                var mod = value % 10000;
                value = value / 10000;
                Buffer.MemoryCopy(pSrc + (mod * 4), pDst, 4, 4);
                if (value > 0)
                {
                    // 2
                    mod = value % 10000;
                    value = value / 10000;
                    Buffer.MemoryCopy(pSrc + (mod * 4), pDst, 4, 4);
                    if (value > 0)
                    {
                        // 3
                        mod = value % 10000;
                        value = value / 10000;
                        Buffer.MemoryCopy(pSrc + (mod * 4), pDst, 4, 4);
                        if (value > 0)
                        {
                            // 4
                            mod = value % 10000;
                            value = value / 10000;
                            Buffer.MemoryCopy(pSrc + (mod * 4), pDst, 4, 4);
                            if (value > 0)
                            {
                                // 5
                                mod = value % 10000;
                                Buffer.MemoryCopy(pSrc + (mod * 4), pDst, 4, 4);
                            }
                        }
                    }
                }
            }
        }
    }
}
