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

        private StringMapper stringW10Mapper;

        private byte[] stringW10Buffer;

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

            var stringW10Pi = type.GetProperty(nameof(Target.StringW10));
            stringW10Mapper = new StringMapper(
                0,
                20,
                factory.CreateGetter(stringW10Pi),
                factory.CreateSetter(stringW10Pi),
                stringEncoding,
                true,
                Padding.Right,
                0x20);
            stringW10Buffer = stringEncoding.GetBytes("あいうえお          ");

            var intText8Pi = type.GetProperty(nameof(Target.Int8));
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
        public void ReadStringW10()
        {
            stringW10Mapper.Read(stringW10Buffer, 0, target);
        }

        [Benchmark]
        public void ReadIntText8()
        {
            intText8Mapper.Read(intText8Buffer, 0, target);
        }
    }
}
