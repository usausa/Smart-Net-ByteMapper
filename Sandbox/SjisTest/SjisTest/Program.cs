namespace SjisTest
{
    using System.Text;
    using System.Reflection;

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
            BenchmarkSwitcher.FromAssembly(typeof(Program).GetTypeInfo().Assembly).Run(args);
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
    public class ParseBenchmark
    {
        private Encoding sjis;

        private byte[] bytes;

        [GlobalSetup]
        public void Setup()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            sjis = Encoding.GetEncoding(932);
            bytes = sjis.GetBytes("123456789");
        }

        [Benchmark]
        public int Parse()
        {
            // < 7ms
            return SjisHelper.ParseInteger(bytes, 0, bytes.Length);
        }
    }

    public static class SjisHelper
    {
        public static int ParseInteger(byte[] bytes, int index, int count)
        {
            var end = index + count;

            // No check, simple version
            var value = 0;
            for (var i = index; i < end; i++)
            {
                value *= 10;
                value += bytes[i] - '0';
            }

            return value;
        }

        // TODO check, minus, trim ?
    }
}