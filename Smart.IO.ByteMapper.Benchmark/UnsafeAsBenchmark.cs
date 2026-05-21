namespace Smart.IO.ByteMapper.Benchmark;

using System.Runtime.CompilerServices;

using BenchmarkDotNet.Attributes;

using Smart.IO.ByteMapper.Fast.Helpers;

/// <summary>
/// (T)(object)value のボックス化コスト vs Unsafe.As&lt;TFrom,TTo&gt; を比較します。
/// ジェネリックメソッド越しに計測することで実際のコンバーター動作を忠実に再現します。
/// </summary>
[Config(typeof(BenchmarkConfig))]
public class UnsafeAsBenchmark
{
    private const int N = 1000;
    private const Padding Pad = Padding.Left;

    private readonly byte[] writeBuffer = new byte[12];
    private readonly byte[] readBuffer  = "    123456  ".Select(c => (byte)c).ToArray();

    // -----------------------------------------------------------------------
    // Write path
    // -----------------------------------------------------------------------

    [Benchmark(OperationsPerInvoke = N, Description = "Write int – (T)(object)")]
    public void WriteInt_Boxed()
    {
        for (var i = 0; i < N; i++) WriteIntBoxed<int>(writeBuffer, 123456);
    }

    [Benchmark(OperationsPerInvoke = N, Description = "Write int – Unsafe.As")]
    public void WriteInt_Unsafe()
    {
        for (var i = 0; i < N; i++) WriteIntUnsafe<int>(writeBuffer, 123456);
    }

    [Benchmark(OperationsPerInvoke = N, Description = "Write long – (T)(object)")]
    public void WriteLong_Boxed()
    {
        for (var i = 0; i < N; i++) WriteLongBoxed<long>(writeBuffer, 123456L);
    }

    [Benchmark(OperationsPerInvoke = N, Description = "Write long – Unsafe.As")]
    public void WriteLong_Unsafe()
    {
        for (var i = 0; i < N; i++) WriteLongUnsafe<long>(writeBuffer, 123456L);
    }

    // -----------------------------------------------------------------------
    // Read path
    // -----------------------------------------------------------------------

    [Benchmark(OperationsPerInvoke = N, Description = "Read int – (T)(object)")]
    public int ReadInt_Boxed()
    {
        var sum = 0;
        for (var i = 0; i < N; i++) sum += ReadIntBoxed<int>(readBuffer);
        return sum;
    }

    [Benchmark(OperationsPerInvoke = N, Description = "Read int – Unsafe.As")]
    public int ReadInt_Unsafe()
    {
        var sum = 0;
        for (var i = 0; i < N; i++) sum += ReadIntUnsafe<int>(readBuffer);
        return sum;
    }

    [Benchmark(OperationsPerInvoke = N, Description = "Read long – (T)(object)")]
    public long ReadLong_Boxed()
    {
        var sum = 0L;
        for (var i = 0; i < N; i++) sum += ReadLongBoxed<long>(readBuffer);
        return sum;
    }

    [Benchmark(OperationsPerInvoke = N, Description = "Read long – Unsafe.As")]
    public long ReadLong_Unsafe()
    {
        var sum = 0L;
        for (var i = 0; i < N; i++) sum += ReadLongUnsafe<long>(readBuffer);
        return sum;
    }

    // -----------------------------------------------------------------------
    // Generic helpers – (T)(object) pattern
    // -----------------------------------------------------------------------

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void WriteIntBoxed<T>(Span<byte> buf, T value) where T : struct
    {
        FastNumberByteHelper.FormatInt32(buf, 0, 10, (int)(object)value!, Pad, false, 0x20);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void WriteLongBoxed<T>(Span<byte> buf, T value) where T : struct
    {
        FastNumberByteHelper.FormatInt64(buf, 0, 10, (long)(object)value!, Pad, false, 0x20);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int ReadIntBoxed<T>(ReadOnlySpan<byte> buf) where T : struct
    {
        FastNumberByteHelper.TryParseInt32(buf, 0, 10, 0x20, out var result);
        return (int)(object)(T)(object)result;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static long ReadLongBoxed<T>(ReadOnlySpan<byte> buf) where T : struct
    {
        FastNumberByteHelper.TryParseInt64(buf, 0, 10, 0x20, out var result);
        return (long)(object)(T)(object)result;
    }

    // -----------------------------------------------------------------------
    // Generic helpers – Unsafe.As pattern
    // -----------------------------------------------------------------------

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void WriteIntUnsafe<T>(Span<byte> buf, T value) where T : struct
    {
        FastNumberByteHelper.FormatInt32(buf, 0, 10, Unsafe.As<T, int>(ref value), Pad, false, 0x20);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void WriteLongUnsafe<T>(Span<byte> buf, T value) where T : struct
    {
        FastNumberByteHelper.FormatInt64(buf, 0, 10, Unsafe.As<T, long>(ref value), Pad, false, 0x20);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int ReadIntUnsafe<T>(ReadOnlySpan<byte> buf) where T : struct
    {
        FastNumberByteHelper.TryParseInt32(buf, 0, 10, 0x20, out var result);
        return Unsafe.As<int, int>(ref result);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static long ReadLongUnsafe<T>(ReadOnlySpan<byte> buf) where T : struct
    {
        FastNumberByteHelper.TryParseInt64(buf, 0, 10, 0x20, out var result);
        return Unsafe.As<long, long>(ref result);
    }
}
