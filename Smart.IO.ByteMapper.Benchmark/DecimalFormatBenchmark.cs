namespace Smart.IO.ByteMapper.Benchmark;

using System.Buffers.Text;
using System.Globalization;
using System.Text;

using BenchmarkDotNet.Attributes;

using Smart.IO.ByteMapper.Helpers;

// BCL (char-buffer) vs Utf8Parser/Utf8Formatter (direct bytes) vs NumberByteHelper (Options) for decimal.
// Current DecimalTextConverter path: bytes → stackalloc char → TryParse / TryFormat → stackalloc char → bytes.
// Candidates: Utf8Parser (read) / Utf8Formatter (write) eliminate the char round-trip;
//             NumberByteHelper operates natively on bytes with fixed scale.
[Config(typeof(BenchmarkConfig))]
#pragma warning disable CA1822
public class DecimalFormatBenchmark
{
    private const int N = 1000;

    private const decimal DecimalSample = 123456.78m;
    private const int Length = 9;
    private const byte Scale = 2;

    // Plain decimal text; compatible with all three parsers.
    private readonly byte[] readBuffer = "123456.78"u8.ToArray();
    private readonly byte[] writeBuffer = new byte[Length];

    // ----- Read (Parse) -----

    [Benchmark(OperationsPerInvoke = N, Description = "Read: decimal (BCL char)")]
    public void ReadDecimalBcl()
    {
        var buffer = readBuffer;
        Span<char> chars = stackalloc char[Length];
        for (var i = 0; i < N; i++)
        {
            var n = Encoding.ASCII.GetChars(buffer, chars);
            _ = decimal.TryParse(chars[..n], NumberStyles.Number, CultureInfo.InvariantCulture, out _);
        }
    }

    [Benchmark(OperationsPerInvoke = N, Description = "Read: decimal (Utf8Parser)")]
    public void ReadDecimalUtf8Parser()
    {
        var buffer = readBuffer;
        for (var i = 0; i < N; i++)
        {
            _ = Utf8Parser.TryParse(buffer, out decimal _, out _);
        }
    }

    [Benchmark(OperationsPerInvoke = N, Description = "Read: decimal (NumberByteHelper)")]
    public void ReadDecimalNumberByteHelper()
    {
        var buffer = readBuffer;
        for (var i = 0; i < N; i++)
        {
            NumberByteHelper.TryParseDecimal(buffer, 0, Length, 0x20, out _);
        }
    }

    // ----- Write (Format) -----

    [Benchmark(OperationsPerInvoke = N, Description = "Write: decimal (BCL char)")]
    public void WriteDecimalBcl()
    {
        var buffer = writeBuffer;
        var value = DecimalSample;
        Span<char> chars = stackalloc char[Length + 32];
        for (var i = 0; i < N; i++)
        {
            value.TryFormat(chars, out var w, "G", CultureInfo.InvariantCulture);
            Encoding.ASCII.GetBytes(chars[..w], buffer);
        }
    }

    [Benchmark(OperationsPerInvoke = N, Description = "Write: decimal (Utf8Formatter)")]
    public void WriteDecimalUtf8Formatter()
    {
        var buffer = writeBuffer;
        var value = DecimalSample;
        for (var i = 0; i < N; i++)
        {
            Utf8Formatter.TryFormat(value, buffer, out _);
        }
    }

    [Benchmark(OperationsPerInvoke = N, Description = "Write: decimal (NumberByteHelper)")]
    public void WriteDecimalNumberByteHelper()
    {
        var buffer = writeBuffer;
        var value = DecimalSample;
        for (var i = 0; i < N; i++)
        {
            NumberByteHelper.FormatDecimal(buffer, 0, Length, value, Scale, -1, Padding.Left, false, 0x20);
        }
    }
}
#pragma warning restore CA1822
