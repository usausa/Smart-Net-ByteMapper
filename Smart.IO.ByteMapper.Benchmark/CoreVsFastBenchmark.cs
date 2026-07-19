namespace Smart.IO.ByteMapper.Benchmark;

using System.Globalization;

using BenchmarkDotNet.Attributes;

using Smart.IO.ByteMapper.Converters;

// Smart.IO.ByteMapper (Core) と Smart.IO.ByteMapper.Fast の重複機能を直接比較します。
//
// 対象ペア:
//   ASCII 文字列   : Core=TextConverter(ASCII)      vs Fast=AsciiConverter
//   整数テキスト   : Core=NumberTextConverter<int>  vs Fast=IntegerConverter<int>
//   decimal テキスト: Core=NumberTextConverter<decimal> vs Fast=FastDecimalConverter
//   DateTime テキスト: Core=DateTimeTextConverter<DateTime> vs Fast=FastDateTimeConverter
[Config(typeof(BenchmarkConfig))]
public class CoreVsFastBenchmark
{
    private const int N = 1000;
    private const int FieldLen = 20;

    // ---- ASCII 文字列 ----
    private readonly TextConverter coreAscii = new(FieldLen);          // codePage=20127(ASCII)
    private readonly AsciiConverter fastAscii = new(FieldLen);

    private readonly byte[] asciiReadBuffer;
    private readonly byte[] asciiWriteBuffer = new byte[FieldLen];
    private const string AsciiValue = "Hello World";

    // ---- 整数テキスト ----
    private readonly NumberTextConverter<int> coreInt = new(10);
    private readonly FastIntegerConverter<int> fastInt = new(10);

    private readonly byte[] intReadBuffer = new byte[10];
    private readonly byte[] intWriteBuffer = new byte[10];
    private const int IntValue = 123456;

    // ---- decimal テキスト ----
    private readonly NumberTextConverter<decimal> coreDec = new(20, style: NumberStyles.Number);
    private readonly FastDecimalConverter fastDec = new(20);

    private readonly byte[] decReadBuffer = new byte[20];
    private readonly byte[] decWriteBuffer = new byte[20];
    private const decimal DecValue = 12345.67m;

    // ---- DateTime テキスト ----
    private const string DateFormat = "yyyyMMddHHmmss";
    private readonly DateTimeTextConverter<DateTime> coreDt = new(DateFormat.Length, DateFormat);
    private readonly FastDateTimeConverter fastDt = new(DateFormat);

    private readonly byte[] dtReadBuffer = new byte[14];
    private readonly byte[] dtWriteBuffer = new byte[14];
    private static readonly DateTime DtValue = new(2024, 6, 1, 12, 30, 45);

    public CoreVsFastBenchmark()
    {
        // ASCII 読み取りバッファ準備
        asciiReadBuffer = new byte[FieldLen];
        System.Text.Encoding.ASCII.GetBytes("Hello World         ".AsSpan(0, FieldLen), asciiReadBuffer);

        // 整数読み取りバッファ準備（右詰め、スペース埋め）
        var intStr = IntValue.ToString(CultureInfo.InvariantCulture).PadLeft(10);
        System.Text.Encoding.ASCII.GetBytes(intStr, intReadBuffer);

        // decimal 読み取りバッファ準備
        var decStr = DecValue.ToString("0.00", CultureInfo.InvariantCulture).PadLeft(20);
        System.Text.Encoding.ASCII.GetBytes(decStr, decReadBuffer);

        // DateTime 読み取りバッファ準備
        System.Text.Encoding.ASCII.GetBytes(DtValue.ToString(DateFormat, CultureInfo.InvariantCulture), dtReadBuffer);
    }

    // ===== ASCII Read =====

    [Benchmark(OperationsPerInvoke = N)]
    public string? CoreAsciiRead()
    {
        var c = coreAscii;
        var buf = asciiReadBuffer;
        string? r = null;
        for (var i = 0; i < N; i++)
        {
            r = c.Read(buf);
        }
        return r;
    }

    [Benchmark(OperationsPerInvoke = N)]
    public string? FastAsciiRead()
    {
        var c = fastAscii;
        var buf = asciiReadBuffer;
        string? r = null;
        for (var i = 0; i < N; i++)
        {
            r = c.Read(buf);
        }
        return r;
    }

    // ===== ASCII Write =====

    [Benchmark(OperationsPerInvoke = N)]
    public void CoreAsciiWrite()
    {
        var c = coreAscii;
        var buf = asciiWriteBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buf, AsciiValue);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void FastAsciiWrite()
    {
        var c = fastAscii;
        var buf = asciiWriteBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buf, AsciiValue);
        }
    }

    // ===== Integer Read =====

    [Benchmark(OperationsPerInvoke = N)]
    public int? CoreIntRead()
    {
        var c = coreInt;
        var buf = intReadBuffer;
        int? r = null;
        for (var i = 0; i < N; i++)
        {
            r = c.Read(buf);
        }
        return r;
    }

    [Benchmark(OperationsPerInvoke = N)]
    public int? FastIntRead()
    {
        var c = fastInt;
        var buf = intReadBuffer;
        int? r = null;
        for (var i = 0; i < N; i++)
        {
            r = c.Read(buf);
        }
        return r;
    }

    // ===== Integer Write =====

    [Benchmark(OperationsPerInvoke = N)]
    public void CoreIntWrite()
    {
        var c = coreInt;
        var buf = intWriteBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buf, IntValue);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void FastIntWrite()
    {
        var c = fastInt;
        var buf = intWriteBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buf, IntValue);
        }
    }

    // ===== Decimal Read =====

    [Benchmark(OperationsPerInvoke = N)]
    public decimal? CoreDecimalRead()
    {
        var c = coreDec;
        var buf = decReadBuffer;
        decimal? r = null;
        for (var i = 0; i < N; i++)
        {
            r = c.Read(buf);
        }
        return r;
    }

    [Benchmark(OperationsPerInvoke = N)]
    public decimal? FastDecimalRead()
    {
        var c = fastDec;
        var buf = decReadBuffer;
        decimal? r = null;
        for (var i = 0; i < N; i++)
        {
            r = c.Read(buf);
        }
        return r;
    }

    // ===== Decimal Write =====

    [Benchmark(OperationsPerInvoke = N)]
    public void CoreDecimalWrite()
    {
        var c = coreDec;
        var buf = decWriteBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buf, DecValue);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void FastDecimalWrite()
    {
        var c = fastDec;
        var buf = decWriteBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buf, DecValue);
        }
    }

    // ===== DateTime Read =====

    [Benchmark(OperationsPerInvoke = N)]
    public DateTime? CoreDateTimeRead()
    {
        var c = coreDt;
        var buf = dtReadBuffer;
        DateTime? r = null;
        for (var i = 0; i < N; i++)
        {
            r = c.Read(buf);
        }
        return r;
    }

    [Benchmark(OperationsPerInvoke = N)]
    public DateTime? FastDateTimeRead()
    {
        var c = fastDt;
        var buf = dtReadBuffer;
        DateTime? r = null;
        for (var i = 0; i < N; i++)
        {
            r = c.Read(buf);
        }
        return r;
    }

    // ===== DateTime Write =====

    [Benchmark(OperationsPerInvoke = N)]
    public void CoreDateTimeWrite()
    {
        var c = coreDt;
        var buf = dtWriteBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buf, DtValue);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void FastDateTimeWrite()
    {
        var c = fastDt;
        var buf = dtWriteBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buf, DtValue);
        }
    }
}
