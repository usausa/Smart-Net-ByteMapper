namespace Smart.IO.ByteMapper.Benchmark;

using System.Buffers;
using System.Buffers.Text;
using System.Text;

using BenchmarkDotNet.Attributes;

// BCL (char-buffer) vs Utf8Parser/Utf8Formatter (direct bytes) for Guid.
// Formats: "N" (32 bytes, no hyphens), "D" (36 bytes, with hyphens).
[Config(typeof(BenchmarkConfig))]
#pragma warning disable CA1822
public class GuidFormatBenchmark
{
    private const int N = 1000;

    private static readonly Guid GuidSample = new("550e8400-e29b-41d4-a716-446655440000");

    // "N" format: 32 hex chars, no hyphens
    private readonly byte[] guidNReadBuffer = "550e8400e29b41d4a716446655440000"u8.ToArray();
    private readonly byte[] guidNWriteBuffer = new byte[32];

    // "D" format: 36 chars with hyphens
    private readonly byte[] guidDReadBuffer = "550e8400-e29b-41d4-a716-446655440000"u8.ToArray();
    private readonly byte[] guidDWriteBuffer = new byte[36];

    // ----- "N" format -----

    [Benchmark(OperationsPerInvoke = N, Description = "Read: Guid N (BCL char)")]
    public void ReadGuidNBcl()
    {
        var buffer = guidNReadBuffer;
        Span<char> chars = stackalloc char[32];
        for (var i = 0; i < N; i++)
        {
            var n = Encoding.ASCII.GetChars(buffer, chars);
            _ = Guid.TryParseExact(chars[..n], "N", out _);
        }
    }

    [Benchmark(OperationsPerInvoke = N, Description = "Read: Guid N (Utf8Parser)")]
    public void ReadGuidNUtf8Parser()
    {
        var buffer = guidNReadBuffer;
        for (var i = 0; i < N; i++)
        {
            _ = Utf8Parser.TryParse(buffer, out Guid _, out _, 'N');
        }
    }

    [Benchmark(OperationsPerInvoke = N, Description = "Write: Guid N (BCL char)")]
    public void WriteGuidNBcl()
    {
        var buffer = guidNWriteBuffer;
        var value = GuidSample;
        Span<char> chars = stackalloc char[40];
        for (var i = 0; i < N; i++)
        {
            value.TryFormat(chars, out var w, "N");
            Encoding.ASCII.GetBytes(chars[..w], buffer);
        }
    }

    [Benchmark(OperationsPerInvoke = N, Description = "Write: Guid N (Utf8Formatter)")]
    public void WriteGuidNUtf8Formatter()
    {
        var buffer = guidNWriteBuffer;
        var value = GuidSample;
        for (var i = 0; i < N; i++)
        {
            Utf8Formatter.TryFormat(value, buffer, out _, new StandardFormat('N'));
        }
    }

    // ----- "D" format -----

    [Benchmark(OperationsPerInvoke = N, Description = "Read: Guid D (BCL char)")]
    public void ReadGuidDBcl()
    {
        var buffer = guidDReadBuffer;
        Span<char> chars = stackalloc char[36];
        for (var i = 0; i < N; i++)
        {
            var n = Encoding.ASCII.GetChars(buffer, chars);
            _ = Guid.TryParseExact(chars[..n], "D", out _);
        }
    }

    [Benchmark(OperationsPerInvoke = N, Description = "Read: Guid D (Utf8Parser)")]
    public void ReadGuidDUtf8Parser()
    {
        var buffer = guidDReadBuffer;
        for (var i = 0; i < N; i++)
        {
            _ = Utf8Parser.TryParse(buffer, out Guid _, out _, 'D');
        }
    }

    [Benchmark(OperationsPerInvoke = N, Description = "Write: Guid D (BCL char)")]
    public void WriteGuidDBcl()
    {
        var buffer = guidDWriteBuffer;
        var value = GuidSample;
        Span<char> chars = stackalloc char[40];
        for (var i = 0; i < N; i++)
        {
            value.TryFormat(chars, out var w, "D");
            Encoding.ASCII.GetBytes(chars[..w], buffer);
        }
    }

    [Benchmark(OperationsPerInvoke = N, Description = "Write: Guid D (Utf8Formatter)")]
    public void WriteGuidDUtf8Formatter()
    {
        var buffer = guidDWriteBuffer;
        var value = GuidSample;
        for (var i = 0; i < N; i++)
        {
            Utf8Formatter.TryFormat(value, buffer, out _, new StandardFormat('D'));
        }
    }
}
#pragma warning restore CA1822
