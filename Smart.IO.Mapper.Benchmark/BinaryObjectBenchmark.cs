namespace Smart.IO.Mapper.Benchmark
{
    using BenchmarkDotNet.Attributes;

    using Smart.IO.Mapper.Attributes;

    [Config(typeof(BenchmarkConfig))]
    public class BinaryObjectBenchmark
    {
        private readonly byte[] buffer = new byte[40];

        private readonly BinaryObject obj = new BinaryObject();

        private ITypeMapper<BinaryObject> mapper;

        [GlobalSetup]
        public void Setup()
        {
            var byteMapper = new ByteMapperConfig()
                .MapByAttribute<BinaryObject>()
                .ToByteMapper();

            mapper = byteMapper.Create<BinaryObject>();
        }

        [Benchmark]
        public void ReadBinaryObject()
        {
            mapper.FromByte(buffer, obj);
        }

        [Benchmark]
        public BinaryObject ReadBinaryObjectWithNew()
        {
            return mapper.FromByte(buffer);
        }

        [Benchmark]
        public void WriteBinaryObject()
        {
            mapper.ToByte(buffer, obj);
        }

        [Benchmark]
        public byte[] WriteBinaryObjectWithAllocate()
        {
            return mapper.ToByte(obj);
        }

        [Map(40, AutoDelimitter = false, AutoFiller = false)]
        public class BinaryObject
        {
            [Binary(0)]
            public int Value0 { get; set; }

            [Binary(4)]
            public int Value1 { get; set; }

            [Binary(8)]
            public int Value2 { get; set; }

            [Binary(12)]
            public int Value3 { get; set; }

            [Binary(16)]
            public int Value4 { get; set; }

            [Binary(20)]
            public int Value5 { get; set; }

            [Binary(24)]
            public int Value6 { get; set; }

            [Binary(28)]
            public int Value7 { get; set; }

            [Binary(32)]
            public int Value8 { get; set; }

            [Binary(36)]
            public int Value9 { get; set; }
        }
    }
}
