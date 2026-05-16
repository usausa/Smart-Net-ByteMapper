namespace Smart.IO.ByteMapper.Benchmark;

using BenchmarkDotNet.Attributes;

using Smart.IO.ByteMapper.Helpers;

/// <summary>
/// Benchmark for methods targeted by intrinsics-opportunities.md:
///   §1 BytesHelper.TrimRange
///   §2 EncodingByteHelper.GetAsciiBytes / GetAsciiString
///   §3 EncodingByteHelper.FillUnicode
///   §5 NumberByteHelper.FormatInt32 / FormatInt64
/// </summary>
[Config(typeof(BenchmarkConfig))]
public class IntrinsicsBenchmark
{
    private const int N = 1000;

    // §1 – TrimRange data
    private static readonly byte[] TrimBufLeftHalf = CreateTrimBuf(Padding.Left, half: true);
    private static readonly byte[] TrimBufLeftAll = new byte[20];   // all fillers
    private static readonly byte[] TrimBufRightHalf = CreateTrimBuf(Padding.Right, half: true);

    // §2 – ASCII data
    private const string Ascii20 = "abcdefghijklmnopqrst";
    private static readonly byte[] AsciiBytes20 = System.Text.Encoding.ASCII.GetBytes(Ascii20);

    // §3 – Unicode fill
    private static readonly byte[] UnicodeBuf30 = new byte[30];

    // §5 – Integer format
    private static readonly byte[] Buf8 = new byte[8];
    private static readonly byte[] Buf18 = new byte[18];

    static IntrinsicsBenchmark()
    {
        TrimBufLeftAll.AsSpan().Fill((byte)' ');
    }

    private static byte[] CreateTrimBuf(Padding padding, bool half)
    {
        var buf = new byte[20];
        if (!half)
        {
            buf.AsSpan().Fill((byte)' ');
            return buf;
        }

        if (padding == Padding.Left)
        {
            for (var i = 0; i < 10; i++) buf[i] = (byte)' ';
            for (var i = 10; i < 20; i++) buf[i] = (byte)'A';
        }
        else
        {
            for (var i = 0; i < 10; i++) buf[i] = (byte)'A';
            for (var i = 10; i < 20; i++) buf[i] = (byte)' ';
        }

        return buf;
    }

    // ===================== §1 TrimRange =====================

    [Benchmark(OperationsPerInvoke = N)]
    public void TrimLeftHalf()
    {
        var buf = TrimBufLeftHalf;
        for (var i = 0; i < N; i++)
        {
            var start = 0;
            var size = 20;
            BytesHelper.TrimRange(buf, ref start, ref size, Padding.Left, (byte)' ');
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void TrimLeftAllFiller()
    {
        var buf = TrimBufLeftAll;
        for (var i = 0; i < N; i++)
        {
            var start = 0;
            var size = 20;
            BytesHelper.TrimRange(buf, ref start, ref size, Padding.Left, (byte)' ');
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void TrimRightHalf()
    {
        var buf = TrimBufRightHalf;
        for (var i = 0; i < N; i++)
        {
            var start = 0;
            var size = 20;
            BytesHelper.TrimRange(buf, ref start, ref size, Padding.Right, (byte)' ');
        }
    }

    // ===================== §2 ASCII =====================

    [Benchmark(OperationsPerInvoke = N)]
    public void GetAsciiBytes20()
    {
        for (var i = 0; i < N; i++)
        {
            _ = EncodingByteHelper.GetAsciiBytes(Ascii20);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void GetAsciiString20()
    {
        var buf = AsciiBytes20;
        for (var i = 0; i < N; i++)
        {
            _ = EncodingByteHelper.GetAsciiString(buf, 0, 20);
        }
    }

    // ===================== §3 FillUnicode =====================

    [Benchmark(OperationsPerInvoke = N)]
    public void FillUnicode30()
    {
        var buf = UnicodeBuf30;
        for (var i = 0; i < N; i++)
        {
            EncodingByteHelper.FillUnicode(buf, ' ');
        }
    }

    // ===================== §5 FormatInt32 / FormatInt64 =====================

    [Benchmark(OperationsPerInvoke = N)]
    public void FormatInt32Max8()
    {
        var buf = Buf8;
        for (var i = 0; i < N; i++)
        {
            NumberByteHelper.FormatInt32(buf, 0, 8, 99999999, Padding.Left, false, (byte)' ');
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void FormatInt32Zero8()
    {
        var buf = Buf8;
        for (var i = 0; i < N; i++)
        {
            NumberByteHelper.FormatInt32(buf, 0, 8, 0, Padding.Left, false, (byte)' ');
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void FormatInt64Max18()
    {
        var buf = Buf18;
        for (var i = 0; i < N; i++)
        {
            NumberByteHelper.FormatInt64(buf, 0, 18, 999999999999999999L, Padding.Left, false, (byte)' ');
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void FormatInt64Zero18()
    {
        var buf = Buf18;
        for (var i = 0; i < N; i++)
        {
            NumberByteHelper.FormatInt64(buf, 0, 18, 0L, Padding.Left, false, (byte)' ');
        }
    }
}
