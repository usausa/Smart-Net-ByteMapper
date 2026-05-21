namespace Smart.IO.ByteMapper.Benchmark;

using BenchmarkDotNet.Attributes;

using Smart.IO.ByteMapper.Converters;

/// <summary>
/// 個別コンバーターを直接呼ぶ場合と生成コード経由の場合を比較します。
/// </summary>
[Config(typeof(BenchmarkConfig))]
public class ConverterBenchmark
{
    private const int N = 1000;

    // --- Binary (int, big-endian) ---

    private readonly BinaryConverter<int> binaryConverter = new(Endian.Big);

    private readonly byte[] binaryReadBuffer = [0x00, 0x00, 0x04, 0xD2]; // 1234

    private readonly byte[] binaryWriteBuffer = new byte[4];

    // --- Text (ASCII, 20 bytes) ---

    private readonly TextConverter textConverter = new(20);

    private readonly byte[] textReadBuffer = new byte[20];

    private readonly byte[] textWriteBuffer = new byte[20];

    private const string TextValue = "Hello World";

    // --- Boolean ---

    private readonly BooleanConverter boolConverter = new(0x31, 0x30, 0x20);

    private readonly byte[] boolReadTrueBuffer = [0x31];

    private readonly byte[] boolWriteBuffer = new byte[1];

    [GlobalSetup]
    public void Setup()
    {
        System.Text.Encoding.ASCII.GetBytes("Hello World         ").CopyTo(textReadBuffer, 0);
    }

    //--------------------------------------------------------------------------------
    // Binary read
    //--------------------------------------------------------------------------------

    [Benchmark(OperationsPerInvoke = N)]
    public int DirectBinaryRead()
    {
        var c = binaryConverter;
        var buffer = binaryReadBuffer;
        var result = 0;
        for (var i = 0; i < N; i++)
        {
            result = c.Read(buffer);
        }
        return result;
    }

    [Benchmark(OperationsPerInvoke = N)]
    public int GeneratedBinaryRead()
    {
        var buffer = binaryReadBuffer;
        var target = new BinaryRecord();
        var result = 0;
        for (var i = 0; i < N; i++)
        {
            BenchmarkMappers.ReadBinary(buffer, target);
            result = target.Value;
        }
        return result;
    }

    //--------------------------------------------------------------------------------
    // Binary write
    //--------------------------------------------------------------------------------

    [Benchmark(OperationsPerInvoke = N)]
    public void DirectBinaryWrite()
    {
        var c = binaryConverter;
        var buffer = binaryWriteBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer, 1234);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void GeneratedBinaryWrite()
    {
        var record = new BinaryRecord { Value = 1234 };
        var buffer = binaryWriteBuffer;
        for (var i = 0; i < N; i++)
        {
            BenchmarkMappers.WriteBinary(record, buffer);
        }
    }

    //--------------------------------------------------------------------------------
    // Text read
    //--------------------------------------------------------------------------------

    [Benchmark(OperationsPerInvoke = N)]
    public string? DirectTextRead()
    {
        var c = textConverter;
        var buffer = textReadBuffer;
        string? result = null;
        for (var i = 0; i < N; i++)
        {
            result = c.Read(buffer);
        }
        return result;
    }

    [Benchmark(OperationsPerInvoke = N)]
    public string? GeneratedTextRead()
    {
        var buffer = textReadBuffer;
        var target = new TextRecord();
        string? result = null;
        for (var i = 0; i < N; i++)
        {
            BenchmarkMappers.ReadText(buffer, target);
            result = target.Name;
        }
        return result;
    }

    //--------------------------------------------------------------------------------
    // Text write
    //--------------------------------------------------------------------------------

    [Benchmark(OperationsPerInvoke = N)]
    public void DirectTextWrite()
    {
        var c = textConverter;
        var buffer = textWriteBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer, TextValue);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void GeneratedTextWrite()
    {
        var record = new TextRecord { Name = TextValue };
        var buffer = textWriteBuffer;
        for (var i = 0; i < N; i++)
        {
            BenchmarkMappers.WriteText(record, buffer);
        }
    }

    //--------------------------------------------------------------------------------
    // Boolean read
    //--------------------------------------------------------------------------------

    [Benchmark(OperationsPerInvoke = N)]
    public bool? DirectBoolRead()
    {
        var c = boolConverter;
        var buffer = boolReadTrueBuffer;
        bool? result = null;
        for (var i = 0; i < N; i++)
        {
            result = c.Read(buffer);
        }
        return result;
    }

    [Benchmark(OperationsPerInvoke = N)]
    public bool? GeneratedBoolRead()
    {
        var buffer = boolReadTrueBuffer;
        var target = new BoolRecord();
        bool? result = null;
        for (var i = 0; i < N; i++)
        {
            BenchmarkMappers.ReadBool(buffer, target);
            result = target.Flag;
        }
        return result;
    }

    //--------------------------------------------------------------------------------
    // Boolean write
    //--------------------------------------------------------------------------------

    [Benchmark(OperationsPerInvoke = N)]
    public void DirectBoolWrite()
    {
        var c = boolConverter;
        var buffer = boolWriteBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer, true);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void GeneratedBoolWrite()
    {
        var record = new BoolRecord { Flag = true };
        var buffer = boolWriteBuffer;
        for (var i = 0; i < N; i++)
        {
            BenchmarkMappers.WriteBool(record, buffer);
        }
    }
}
