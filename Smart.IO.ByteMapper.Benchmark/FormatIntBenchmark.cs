namespace Smart.IO.ByteMapper.Benchmark;

using BenchmarkDotNet.Attributes;

using Smart.IO.ByteMapper.Fast.Helpers;

// FormatInt32 vs FormatInt64 / TryParseInt32 vs TryParseInt64 の性能差を検証します。
[Config(typeof(BenchmarkConfig))]
public class FormatIntBenchmark
{
    private const int N = 1000;
    private const Padding Pad = Padding.Left;

    private readonly byte[] buffer = new byte[20];

    // --- Write: int (FormatInt32) ---

    [Benchmark(OperationsPerInvoke = N, Description = "FormatInt32 (int=1234)")]
    public void FormatInt32_Int()
    {
        for (var i = 0; i < N; i++)
        {
            FastNumberByteHelper.FormatInt32(buffer, 0, 10, 1234, Pad, false, 0x20);
        }
    }

    // --- Write: int via FormatInt64 ---

    [Benchmark(OperationsPerInvoke = N, Description = "FormatInt64 (int=1234)")]
    public void FormatInt64_AsInt()
    {
        for (var i = 0; i < N; i++)
        {
            FastNumberByteHelper.FormatInt64(buffer, 0, 10, 1234L, Pad, false, 0x20);
        }
    }

    // --- Write: short via FormatInt32 (current path) ---

    [Benchmark(OperationsPerInvoke = N, Description = "FormatInt32 (short=1234)")]
    public void FormatInt32_Short()
    {
        for (var i = 0; i < N; i++)
        {
            FastNumberByteHelper.FormatInt32(buffer, 0, 10, (short)1234, Pad, false, 0x20);
        }
    }

    // --- Write: short via FormatInt64 (proposed path) ---

    [Benchmark(OperationsPerInvoke = N, Description = "FormatInt64 (short=1234)")]
    public void FormatInt64_AsShort()
    {
        for (var i = 0; i < N; i++)
        {
            FastNumberByteHelper.FormatInt64(buffer, 0, 10, (short)1234, Pad, false, 0x20);
        }
    }

    // --- Write: long (FormatInt64) ---

    [Benchmark(OperationsPerInvoke = N, Description = "FormatInt64 (long=1234)")]
    public void FormatInt64_Long()
    {
        for (var i = 0; i < N; i++)
        {
            FastNumberByteHelper.FormatInt64(buffer, 0, 10, 1234L, Pad, false, 0x20);
        }
    }

    // --- Write: large int ---

    [Benchmark(OperationsPerInvoke = N, Description = "FormatInt32 (int=Int32.MaxValue)")]
    public void FormatInt32_Max()
    {
        for (var i = 0; i < N; i++)
        {
            FastNumberByteHelper.FormatInt32(buffer, 0, 15, int.MaxValue, Pad, false, 0x20);
        }
    }

    [Benchmark(OperationsPerInvoke = N, Description = "FormatInt64 (int=Int32.MaxValue)")]
    public void FormatInt64_AsIntMax()
    {
        for (var i = 0; i < N; i++)
        {
            FastNumberByteHelper.FormatInt64(buffer, 0, 15, int.MaxValue, Pad, false, 0x20);
        }
    }

    // --- Write: large long ---

    [Benchmark(OperationsPerInvoke = N, Description = "FormatInt64 (long=Int64.MaxValue)")]
    public void FormatInt64_Max()
    {
        for (var i = 0; i < N; i++)
        {
            FastNumberByteHelper.FormatInt64(buffer, 0, 20, long.MaxValue, Pad, false, 0x20);
        }
    }

    // --- Read: int via TryParseInt32 ---

    private readonly byte[] parseShortBuffer = "      1234"u8.ToArray();
    private readonly byte[] parseIntBuffer   = "    123456"u8.ToArray();
    private readonly byte[] parseLongBuffer  = "  12345678901234"u8.ToArray();

    [Benchmark(OperationsPerInvoke = N, Description = "TryParseInt32 (int=1234)")]
    public int TryParseInt32_Short()
    {
        var result = 0;
        for (var i = 0; i < N; i++)
        {
            FastNumberByteHelper.TryParseInt32(parseShortBuffer, 0, 10, 0x20, out var v);
            result = v;
        }
        return result;
    }

    [Benchmark(OperationsPerInvoke = N, Description = "TryParseInt64 (int=1234)")]
    public long TryParseInt64_Short()
    {
        var result = 0L;
        for (var i = 0; i < N; i++)
        {
            FastNumberByteHelper.TryParseInt64(parseShortBuffer, 0, 10, 0x20, out var v);
            result = v;
        }
        return result;
    }

    [Benchmark(OperationsPerInvoke = N, Description = "TryParseInt32 (int=123456)")]
    public int TryParseInt32_Int()
    {
        var result = 0;
        for (var i = 0; i < N; i++)
        {
            FastNumberByteHelper.TryParseInt32(parseIntBuffer, 0, 10, 0x20, out var v);
            result = v;
        }
        return result;
    }

    [Benchmark(OperationsPerInvoke = N, Description = "TryParseInt64 (int=123456)")]
    public long TryParseInt64_Int()
    {
        var result = 0L;
        for (var i = 0; i < N; i++)
        {
            FastNumberByteHelper.TryParseInt64(parseIntBuffer, 0, 10, 0x20, out var v);
            result = v;
        }
        return result;
    }

    [Benchmark(OperationsPerInvoke = N, Description = "TryParseInt64 (long=12345678901234)")]
    public long TryParseInt64_Long()
    {
        var result = 0L;
        for (var i = 0; i < N; i++)
        {
            FastNumberByteHelper.TryParseInt64(parseLongBuffer, 0, 16, 0x20, out var v);
            result = v;
        }
        return result;
    }
}
