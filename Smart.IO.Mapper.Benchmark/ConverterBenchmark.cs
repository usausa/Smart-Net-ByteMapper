namespace Smart.IO.Mapper.Benchmark
{
    using System.Globalization;
    using System.Text;

    using BenchmarkDotNet.Attributes;

    using Smart.IO.Mapper;
    using Smart.IO.Mapper.Converters;

    [Config(typeof(BenchmarkConfig))]
    public class ConverterBenchmark
    {
        private BigEndianIntBinaryConverter intBinaryConverter;

        private byte[] intBinaryBuffer;

        private StringConverter stringW10Converter;

        private byte[] stringW10Buffer;

        private IntTextConverter intText8Converter;

        private byte[] intText8Buffer;

        [GlobalSetup]
        public void Setup()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var numberEncoding = Encoding.ASCII;
            var stringEncoding = Encoding.GetEncoding(932);

            intBinaryConverter = new BigEndianIntBinaryConverter();
            intBinaryBuffer = new byte[sizeof(int)];

            stringW10Converter = new StringConverter(
                20,
                stringEncoding,
                true,
                Padding.Right,
                0x20);
            stringW10Buffer = stringEncoding.GetBytes("あいうえお          ");

            intText8Converter = new IntTextConverter(
                8,
                numberEncoding,
                true,
                Padding.Left,
                0x20,
                NumberStyles.Integer,
                NumberFormatInfo.InvariantInfo,
                typeof(int));
            intText8Buffer = numberEncoding.GetBytes("  100000");
        }

        [Benchmark]
        public void ReadIntBinary()
        {
            intBinaryConverter.Read(intBinaryBuffer, 0);
        }

        [Benchmark]
        public void ReadStringW10()
        {
            stringW10Converter.Read(stringW10Buffer, 0);
        }

        [Benchmark]
        public void ReadIntText8()
        {
            intText8Converter.Read(intText8Buffer, 0);
        }
    }
}
