namespace Smart.IO.ByteMapper.Benchmark;

using System.Runtime.CompilerServices;
using System.Text;

using BenchmarkDotNet.Attributes;

// ASCII 読み取り実装の比較:
//   Legacy  : Encoding.ASCII.GetString (改善前)
//   Current : System.Text.Ascii.ToUtf16 + unsafe fixed (現行実装)
[Config(typeof(BenchmarkConfig))]
public class AsciiReadBenchmark
{
    private const int FieldLen = 20;
    private const int N = 1000;

    private byte[] buffer = [];

    [GlobalSetup]
    public void Setup()
    {
        buffer = new byte[FieldLen];
        // "Hello World         " (右パディング)
        var src = System.Text.Encoding.ASCII.GetBytes("Hello World");
        src.CopyTo(buffer, 0);
        buffer.AsSpan(src.Length).Fill(0x20);
    }

    // ---- Legacy: Encoding.ASCII.GetString ----
    [Benchmark(Baseline = true, Description = "Encoding.ASCII.GetString")]
    public string LegacyEncodingAscii()
    {
        var result = string.Empty;
        for (var i = 0; i < N; i++)
        {
            result = GetAsciiLegacy(buffer, 0, FieldLen);
        }
        return result;
    }

    // ---- Current: Ascii.ToUtf16 + unsafe fixed ----
    [Benchmark(Description = "Ascii.ToUtf16 (unsafe fixed)")]
    public string CurrentAsciiToUtf16()
    {
        var result = string.Empty;
        for (var i = 0; i < N; i++)
        {
            result = GetAsciiToUtf16(buffer, 0, FieldLen);
        }
        return result;
    }

    // ---- Legacy implementation (改善前) ----
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static string GetAsciiLegacy(ReadOnlySpan<byte> bytes, int index, int length) =>
        Encoding.ASCII.GetString(bytes.Slice(index, length));

    // ---- Current implementation (現行) ----
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static unsafe string GetAsciiToUtf16(ReadOnlySpan<byte> bytes, int index, int length)
    {
        var slice = bytes.Slice(index, length);
        var result = new string('\0', length);
        fixed (char* dest = result)
        {
            System.Text.Ascii.ToUtf16(slice, new Span<char>(dest, length), out _);
        }
        return result;
    }
}
