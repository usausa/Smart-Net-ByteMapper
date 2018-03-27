namespace Smart.IO.Mapper.Benchmark
{
    using System.Globalization;
    using System.Text;

    using BenchmarkDotNet.Attributes;

    using Smart.IO.Mapper.Mappers;

    [Config(typeof(BenchmarkConfig))]
    public class Benchmark
    {
        private BigEndianIntBinaryMapper intBinaryMapper;

        private byte[] intBinaryBuffer;

        private StringMapper stringW10Mapper;

        private byte[] stringW10Buffer;

        private IntTextMapper intText8Mapper;

        private byte[] intText8Buffer;

        [GlobalSetup]
        public void Setup()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var numberEncoding = Encoding.ASCII;
            var stringEncoding = Encoding.GetEncoding(932);

            intBinaryMapper = new BigEndianIntBinaryMapper();
            intBinaryBuffer = new byte[sizeof(int)];

            stringW10Mapper = new StringMapper(
                20,
                stringEncoding,
                true,
                Padding.Right,
                0x20);
            stringW10Buffer = stringEncoding.GetBytes("あいうえお          ");

            intText8Mapper = new IntTextMapper(
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
            intBinaryMapper.Read(intBinaryBuffer, 0);
        }

        [Benchmark]
        public void ReadStringW10()
        {
            stringW10Mapper.Read(stringW10Buffer, 0);
        }

        [Benchmark]
        public void ReadIntText8()
        {
            intText8Mapper.Read(intText8Buffer, 0);
        }
    }
}
