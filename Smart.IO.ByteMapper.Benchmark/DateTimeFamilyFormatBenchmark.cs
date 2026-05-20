namespace Smart.IO.ByteMapper.Benchmark;

using System.Globalization;
using System.Text;

using BenchmarkDotNet.Attributes;

using Smart.IO.ByteMapper.Builders;
using Smart.IO.ByteMapper.Converters;
using Smart.Text.Japanese;

// Fast-path vs Raw BCL comparison for DateOnly, TimeOnly, and DateTimeOffset.
[Config(typeof(BenchmarkConfig))]
#pragma warning disable CA1822
public class DateTimeFamilyFormatBenchmark
{
    private const int N = 1000;

    private static readonly DateOnly DateOnlySample = new(2026, 5, 17);

    private static readonly TimeOnly TimeOnlySample = new(12, 34, 56);

    private static readonly DateTimeOffset DateTimeOffsetSample = new(new DateTime(2026, 5, 17, 12, 34, 56));

    private byte[] dateOnlyReadBuffer = default!;
    private byte[] dateOnlyWriteBuffer = default!;
    private byte[] timeOnlyReadBuffer = default!;
    private byte[] timeOnlyWriteBuffer = default!;
    private byte[] dateTimeOffsetReadBuffer = default!;
    private byte[] dateTimeOffsetWriteBuffer = default!;

    private IMapConverter dateOnlyConverter = default!;
    private IMapConverter timeOnlyConverter = default!;
    private IMapConverter dateTimeOffsetConverter = default!;

    private static BuilderContext CreateBuilderContext()
    {
        var config = new MapperFactoryConfig()
            .UseOptionsDefault();
        config.DefaultEncoding(SjisEncoding.Instance);

        return new BuilderContext(
            ((IMapperFactoryConfig)config).ResolveComponents(),
            ((IMapperFactoryConfig)config).ResolveParameters(),
            new Dictionary<string, object>());
    }

    [GlobalSetup]
    public void Setup()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        var context = CreateBuilderContext();

        dateOnlyConverter = new DateTimeTextConverterBuilder
        {
            Length = 8,
            Format = "yyyyMMdd"
        }.CreateConverter(context, typeof(DateOnly));

        timeOnlyConverter = new DateTimeTextConverterBuilder
        {
            Length = 6,
            Format = "HHmmss"
        }.CreateConverter(context, typeof(TimeOnly));

        dateTimeOffsetConverter = new DateTimeTextConverterBuilder
        {
            Length = 14,
            Format = "yyyyMMddHHmmss"
        }.CreateConverter(context, typeof(DateTimeOffset));

        dateOnlyReadBuffer = Encoding.ASCII.GetBytes(DateOnlySample.ToString("yyyyMMdd", CultureInfo.InvariantCulture));
        dateOnlyWriteBuffer = new byte[8];

        timeOnlyReadBuffer = Encoding.ASCII.GetBytes(TimeOnlySample.ToString("HHmmss", CultureInfo.InvariantCulture));
        timeOnlyWriteBuffer = new byte[6];

        dateTimeOffsetReadBuffer = Encoding.ASCII.GetBytes(DateTimeOffsetSample.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture));
        dateTimeOffsetWriteBuffer = new byte[14];
    }

    // ----- DateOnly -----

    [Benchmark(OperationsPerInvoke = N, Description = "Read: DateOnly (fast-path)")]
    public void ReadDateOnlyFast()
    {
        var c = dateOnlyConverter;
        var buffer = dateOnlyReadBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Read(buffer.AsSpan());
        }
    }

    [Benchmark(OperationsPerInvoke = N, Description = "Read: DateOnly TryParseExact (BCL)")]
    public void ReadDateOnlyRawBcl()
    {
        var buffer = dateOnlyReadBuffer;
        Span<char> chars = stackalloc char[8];
        for (var i = 0; i < N; i++)
        {
            var n = Encoding.ASCII.GetChars(buffer, chars);
            DateOnly.TryParseExact(chars[..n], "yyyyMMdd", null, DateTimeStyles.None, out _);
        }
    }

    [Benchmark(OperationsPerInvoke = N, Description = "Write: DateOnly (fast-path)")]
    public void WriteDateOnlyFast()
    {
        var c = dateOnlyConverter;
        var buffer = dateOnlyWriteBuffer;
        var value = DateOnlySample;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer.AsSpan(), value);
        }
    }

    [Benchmark(OperationsPerInvoke = N, Description = "Write: DateOnly TryFormat (BCL)")]
    public void WriteDateOnlyRawBcl()
    {
        var buffer = dateOnlyWriteBuffer;
        var value = DateOnlySample;
        Span<char> chars = stackalloc char[8];
        for (var i = 0; i < N; i++)
        {
            value.TryFormat(chars, out var w, "yyyyMMdd", CultureInfo.InvariantCulture);
            Encoding.ASCII.GetBytes(chars[..w], buffer);
        }
    }

    // ----- TimeOnly -----

    [Benchmark(OperationsPerInvoke = N, Description = "Read: TimeOnly (fast-path)")]
    public void ReadTimeOnlyFast()
    {
        var c = timeOnlyConverter;
        var buffer = timeOnlyReadBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Read(buffer.AsSpan());
        }
    }

    [Benchmark(OperationsPerInvoke = N, Description = "Read: TimeOnly TryParseExact (BCL)")]
    public void ReadTimeOnlyRawBcl()
    {
        var buffer = timeOnlyReadBuffer;
        Span<char> chars = stackalloc char[6];
        for (var i = 0; i < N; i++)
        {
            var n = Encoding.ASCII.GetChars(buffer, chars);
            TimeOnly.TryParseExact(chars[..n], "HHmmss", null, DateTimeStyles.None, out _);
        }
    }

    [Benchmark(OperationsPerInvoke = N, Description = "Write: TimeOnly (fast-path)")]
    public void WriteTimeOnlyFast()
    {
        var c = timeOnlyConverter;
        var buffer = timeOnlyWriteBuffer;
        var value = TimeOnlySample;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer.AsSpan(), value);
        }
    }

    [Benchmark(OperationsPerInvoke = N, Description = "Write: TimeOnly TryFormat (BCL)")]
    public void WriteTimeOnlyRawBcl()
    {
        var buffer = timeOnlyWriteBuffer;
        var value = TimeOnlySample;
        Span<char> chars = stackalloc char[6];
        for (var i = 0; i < N; i++)
        {
            value.TryFormat(chars, out var w, "HHmmss", CultureInfo.InvariantCulture);
            Encoding.ASCII.GetBytes(chars[..w], buffer);
        }
    }

    // ----- DateTimeOffset -----

    [Benchmark(OperationsPerInvoke = N, Description = "Read: DateTimeOffset (fast-path)")]
    public void ReadDateTimeOffsetFast()
    {
        var c = dateTimeOffsetConverter;
        var buffer = dateTimeOffsetReadBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Read(buffer.AsSpan());
        }
    }

    [Benchmark(OperationsPerInvoke = N, Description = "Read: DateTimeOffset TryParseExact (BCL)")]
    public void ReadDateTimeOffsetRawBcl()
    {
        var buffer = dateTimeOffsetReadBuffer;
        Span<char> chars = stackalloc char[14];
        for (var i = 0; i < N; i++)
        {
            var n = Encoding.ASCII.GetChars(buffer, chars);
            DateTimeOffset.TryParseExact(chars[..n], "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
        }
    }

    [Benchmark(OperationsPerInvoke = N, Description = "Write: DateTimeOffset (fast-path)")]
    public void WriteDateTimeOffsetFast()
    {
        var c = dateTimeOffsetConverter;
        var buffer = dateTimeOffsetWriteBuffer;
        var value = DateTimeOffsetSample;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer.AsSpan(), value);
        }
    }

    [Benchmark(OperationsPerInvoke = N, Description = "Write: DateTimeOffset TryFormat (BCL)")]
    public void WriteDateTimeOffsetRawBcl()
    {
        var buffer = dateTimeOffsetWriteBuffer;
        var value = DateTimeOffsetSample;
        Span<char> chars = stackalloc char[14];
        for (var i = 0; i < N; i++)
        {
            value.TryFormat(chars, out var w, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            Encoding.ASCII.GetBytes(chars[..w], buffer);
        }
    }
}
#pragma warning restore CA1822
