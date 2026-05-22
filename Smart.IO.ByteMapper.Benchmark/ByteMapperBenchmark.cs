namespace Smart.IO.ByteMapper.Benchmark;

using System;

using BenchmarkDotNet.Attributes;

using Smart.IO.ByteMapper.Converters;

/// <summary>
/// 複合レコード（int 10フィールド）の直接処理 vs 生成コード比較。
/// </summary>
[Config(typeof(BenchmarkConfig))]
public class ByteMapperBenchmark
{
    private const int N = 1000;

    // Binary: int x10 = 40 bytes
    private const int BinarySize = 40;

    private readonly BinaryConverter<int>[] binaryConverters =
    [
        new(), new(), new(), new(), new(), new(), new(), new(), new(), new(),
    ];

    private readonly byte[] binaryBuffer = new byte[BinarySize];

    private readonly BinaryRecord10 binaryRecord = new()
    {
        V0 = 0, V1 = 1, V2 = 2, V3 = 3, V4 = 4, V5 = 5, V6 = 6, V7 = 7, V8 = 8, V9 = 9
    };

    [GlobalSetup]
    public void Setup()
    {
        // Setup: write initial values so read buffer is valid
        BenchmarkMappers.WriteBinary10(binaryRecord, binaryBuffer);
    }

    //--------------------------------------------------------------------------------
    // Direct: manually call each converter
    //--------------------------------------------------------------------------------

    [Benchmark(OperationsPerInvoke = N)]
    public void DirectRead()
    {
        var cs = binaryConverters;
        var buf = binaryBuffer;
        var rec = binaryRecord;
        for (var i = 0; i < N; i++)
        {
            rec.V0 = cs[0].Read(buf.AsSpan(0, 4));
            rec.V1 = cs[1].Read(buf.AsSpan(4, 4));
            rec.V2 = cs[2].Read(buf.AsSpan(8, 4));
            rec.V3 = cs[3].Read(buf.AsSpan(12, 4));
            rec.V4 = cs[4].Read(buf.AsSpan(16, 4));
            rec.V5 = cs[5].Read(buf.AsSpan(20, 4));
            rec.V6 = cs[6].Read(buf.AsSpan(24, 4));
            rec.V7 = cs[7].Read(buf.AsSpan(28, 4));
            rec.V8 = cs[8].Read(buf.AsSpan(32, 4));
            rec.V9 = cs[9].Read(buf.AsSpan(36, 4));
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void DirectWrite()
    {
        var cs = binaryConverters;
        var buf = binaryBuffer;
        var rec = binaryRecord;
        for (var i = 0; i < N; i++)
        {
            cs[0].Write(buf.AsSpan(0, 4), rec.V0);
            cs[1].Write(buf.AsSpan(4, 4), rec.V1);
            cs[2].Write(buf.AsSpan(8, 4), rec.V2);
            cs[3].Write(buf.AsSpan(12, 4), rec.V3);
            cs[4].Write(buf.AsSpan(16, 4), rec.V4);
            cs[5].Write(buf.AsSpan(20, 4), rec.V5);
            cs[6].Write(buf.AsSpan(24, 4), rec.V6);
            cs[7].Write(buf.AsSpan(28, 4), rec.V7);
            cs[8].Write(buf.AsSpan(32, 4), rec.V8);
            cs[9].Write(buf.AsSpan(36, 4), rec.V9);
        }
    }

    //--------------------------------------------------------------------------------
    // Generated: use source-generator mapper
    //--------------------------------------------------------------------------------

    [Benchmark(OperationsPerInvoke = N)]
    public void GeneratedRead()
    {
        var buf = binaryBuffer;
        var rec = binaryRecord;
        for (var i = 0; i < N; i++)
        {
            BenchmarkMappers.ReadBinary10(buf, rec);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void GeneratedWrite()
    {
        var rec = binaryRecord;
        var buf = binaryBuffer;
        for (var i = 0; i < N; i++)
        {
            BenchmarkMappers.WriteBinary10(rec, buf);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public byte[] GeneratedWriteAlloc()
    {
        var rec = binaryRecord;
        byte[] ret = [];
        for (var i = 0; i < N; i++)
        {
            ret = BenchmarkMappersAlloc.WriteBinary10Alloc(rec);
        }
        return ret;
    }
}
