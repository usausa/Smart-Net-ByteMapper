namespace Smart.IO.ByteMapper.Benchmark;

using System.Text;

using BenchmarkDotNet.Attributes;

using Smart.Text.Japanese;

#pragma warning disable IDE0320
// ReSharper disable StringLiteralTypo
[Config(typeof(BenchmarkConfig))]
public class ComplexBenchmark
{
    private const int N = 1000;

    private byte[] allocatedBuffer;

    private ComplexData allocatedData;

    private ITypeMapper<ComplexData> mapper;

    [GlobalSetup]
    public void Setup()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        var mapperFactory = new MapperFactoryConfig()
            .UseOptionsDefault()
            .DefaultEncoding(SjisEncoding.Instance)
            .CreateMapByExpression<ComplexData>(144, config => config
                .ForMember(x => x.StringValue1, m => m.Text(20))
                .ForMember(x => x.StringValue2, m => m.Text(20))
                .ForMember(x => x.StringValue3, m => m.Text(20))
                .ForMember(x => x.IntValue1, m => m.Integer(8))
                .ForMember(x => x.IntValue2, m => m.Integer(8))
                .ForMember(x => x.IntValue3, m => m.Integer(8))
                .ForMember(x => x.IntValue4, m => m.Integer(8))
                .ForMember(x => x.DecimalValue1, m => m.Decimal(10, 2))
                .ForMember(x => x.DecimalValue2, m => m.Decimal(10, 2))
                .ForMember(x => x.BoolValue1, m => m.Boolean())
                .ForMember(x => x.BoolValue2, m => m.Boolean())
                .ForMember(x => x.DateTimeValue1, m => m.DateTime("yyyyMMddHHmmss"))
                .ForMember(x => x.DateTimeValue2, m => m.DateTime("yyyyMMddHHmmss")))
            .ToMapperFactory();

        mapper = mapperFactory.Create<ComplexData>();

        allocatedBuffer = new byte[mapper.Size];
        allocatedData = new ComplexData
        {
            StringValue1 = "XXXXXXXXXXXXXXXXXXXX",
            StringValue2 = "あああああ",
            StringValue3 = string.Empty,
            IntValue1 = 1,
            IntValue2 = 0,
            IntValue3 = 1,
            IntValue4 = null,
            BoolValue1 = true,
            BoolValue2 = null,
            DecimalValue1 = 1.23m,
            DecimalValue2 = null,
            DateTimeValue1 = new DateTime(2000, 12, 31, 23, 59, 59, 999),
            DateTimeValue2 = null
        };

        mapper.ToByte(allocatedBuffer, 0, allocatedData);
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void FromByte()
    {
        var m = mapper;
        var buffer = allocatedBuffer;
        var data = allocatedData;
        for (var i = 0; i < N; i++)
        {
            m.FromByte(buffer, 0, data);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void FromByteWithAllocate()
    {
        var m = mapper;
        var buffer = allocatedBuffer;
        for (var i = 0; i < N; i++)
        {
            m.FromByte(buffer, 0, new ComplexData());
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void ToByte()
    {
        var m = mapper;
        var buffer = allocatedBuffer;
        var data = allocatedData;
        for (var i = 0; i < N; i++)
        {
            m.ToByte(buffer, 0, data);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void ToByteWithAllocate()
    {
        var m = mapper;
        var data = allocatedData;
        for (var i = 0; i < N; i++)
        {
            var buffer = new byte[mapper.Size];
            m.ToByte(buffer, 0, data);
        }
    }
}

public sealed class ComplexData
{
    public string StringValue1 { get; set; }

    public string StringValue2 { get; set; }

    public string StringValue3 { get; set; }

    public int IntValue1 { get; set; }

    public int IntValue2 { get; set; }

    public int? IntValue3 { get; set; }

    public int? IntValue4 { get; set; }

    public decimal DecimalValue1 { get; set; }

    public decimal? DecimalValue2 { get; set; }

    public bool BoolValue1 { get; set; }

    public bool? BoolValue2 { get; set; }

    public DateTime DateTimeValue1 { get; set; }

    public DateTime? DateTimeValue2 { get; set; }
}
