namespace Smart.IO.Mapper.Benchmark
{
    using System.Globalization;
    using System.Text;

    using BenchmarkDotNet.Attributes;

    using Smart.IO.Mapper.Mappers;
    using Smart.Reflection;

    [Config(typeof(BenchmarkConfig))]
    public class Benchmark
    {
        private Target target;

        private BigEndianIntBinaryMapper intBinaryMapper;

        private byte[] intBinaryBuffer;

        private StringMapper stringWide10Mapper;

        private byte[] stringWide10Buffer;

        private IntTextMapper intText8Mapper;

        private byte[] intText8Buffer;

        [GlobalSetup]
        public void Setup()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var type = typeof(Target);
            var factory = DelegateFactory.Default;
            var numberEncoding = Encoding.ASCII;
            var stringEncoding = Encoding.GetEncoding(932);

            target = new Target();

            var intBinaryPi = type.GetProperty(nameof(Target.IntBinary));
            intBinaryMapper = new BigEndianIntBinaryMapper(
                0,
                factory.CreateGetter(intBinaryPi),
                factory.CreateSetter(intBinaryPi));
            intBinaryBuffer = new byte[intBinaryMapper.Length];

            var stringWide10Pi = type.GetProperty(nameof(Target.StringWide10));
            stringWide10Mapper = new StringMapper(
                0,
                20,
                factory.CreateGetter(stringWide10Pi),
                factory.CreateSetter(stringWide10Pi),
                stringEncoding,
                true,
                Padding.Right,
                0x20);
            stringWide10Buffer = stringEncoding.GetBytes("あいうえお          ");

            var intText8Pi = type.GetProperty(nameof(Target.IntText8));
            intText8Mapper = new IntTextMapper(
                0,
                8,
                factory.CreateGetter(intText8Pi),
                factory.CreateSetter(intText8Pi),
                numberEncoding,
                true,
                Padding.Left,
                0x20,
                NumberStyles.Integer,
                NumberFormatInfo.InvariantInfo,
                intText8Pi.PropertyType);
            intText8Buffer = numberEncoding.GetBytes("  100000");
        }

        [Benchmark]
        public void ReadIntBinary()
        {
            intBinaryMapper.Read(intBinaryBuffer, 0, target);
        }

        [Benchmark]
        public void ReadStringWide10()
        {
            stringWide10Mapper.Read(stringWide10Buffer, 0, target);
        }

        [Benchmark]
        public void ReadIntText8()
        {
            intText8Mapper.Read(intText8Buffer, 0, target);
        }
    }
}
