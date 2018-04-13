namespace ByteHelperTest.Benchmarks
{
    using System.Text;

    using BenchmarkDotNet.Attributes;

    [Config(typeof(BenchmarkConfig))]
    public class EncodingBenchmark
    {
        private const string Text = "0123456789";

        private static readonly byte[] Bytes = Encoding.ASCII.GetBytes(Text);

        private Encoding ascii;

        [GlobalSetup]
        public void Setup()
        {
            ascii = Encoding.ASCII;
        }

        [Benchmark]
        public void GetBytesByEncoding()
        {
            ascii.GetBytes(Text);
        }

        [Benchmark]
        public void GetBytesByCustom()
        {
            ByteHelper.GetAsciiBytes(Text);
        }

        [Benchmark]
        public void GetStringByEncoding()
        {
            ascii.GetString(Bytes);
        }

        [Benchmark]
        public void GetStringByCustom()
        {
            ByteHelper.GetAsciiString(Bytes);
        }

        [Benchmark]
        public void GetStringByCustom2()
        {
            ByteHelper.GetAsciiString2(Bytes);
        }
    }
}
