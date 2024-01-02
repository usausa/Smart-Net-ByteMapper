namespace Smart.IO.ByteMapper.Benchmark;

using BenchmarkDotNet.Attributes;

using Smart.IO.ByteMapper.Attributes;

[Config(typeof(BenchmarkConfig))]
public class BinaryObjectBenchmark
{
    private const int N = 1000;

    private readonly byte[] allocatedBuffer = new byte[40];

    private readonly BinaryObject allocatedData = new();

    private ITypeMapper<BinaryObject> mapper;

    [GlobalSetup]
    public void Setup()
    {
        var mapperFactory = new MapperFactoryConfig()
            .CreateMapByAttribute<BinaryObject>()
            .ToMapperFactory();

        mapper = mapperFactory.Create<BinaryObject>();
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void ReadBinaryObject()
    {
        var m = mapper;
        var buffer = allocatedBuffer;
        var data = allocatedData;
        for (var i = 0; i < N; i++)
        {
            m.FromByte(buffer, data);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void WriteBinaryObject()
    {
        var m = mapper;
        var buffer = allocatedBuffer;
        var data = allocatedData;
        for (var i = 0; i < N; i++)
        {
            m.ToByte(buffer, data);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public byte[] WriteBinaryObjectWithAllocate()
    {
        var m = mapper;
        var data = allocatedData;
        var ret = default(byte[]);
        for (var i = 0; i < N; i++)
        {
            ret = m.ToByte(data);
        }
        return ret;
    }

    [Map(40, UseDelimiter = false, AutoFiller = false)]
    public class BinaryObject
    {
        [MapBinary(0)]
        public int Value0 { get; set; }

        [MapBinary(4)]
        public int Value1 { get; set; }

        [MapBinary(8)]
        public int Value2 { get; set; }

        [MapBinary(12)]
        public int Value3 { get; set; }

        [MapBinary(16)]
        public int Value4 { get; set; }

        [MapBinary(20)]
        public int Value5 { get; set; }

        [MapBinary(24)]
        public int Value6 { get; set; }

        [MapBinary(28)]
        public int Value7 { get; set; }

        [MapBinary(32)]
        public int Value8 { get; set; }

        [MapBinary(36)]
        public int Value9 { get; set; }
    }
}
