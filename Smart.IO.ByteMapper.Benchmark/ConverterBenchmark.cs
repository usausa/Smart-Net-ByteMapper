namespace Smart.IO.ByteMapper.Benchmark;

using System.Globalization;
using System.Text;

using BenchmarkDotNet.Attributes;

using Smart.IO.ByteMapper;
using Smart.IO.ByteMapper.Builders;
using Smart.IO.ByteMapper.Converters;
using Smart.Text.Japanese;

#pragma warning disable CA1305
// ReSharper disable StringLiteralTypo
[Config(typeof(BenchmarkConfig))]
public sealed class ConverterBenchmark
{
    private const int N = 1000;

    private const string Text20Single10 = "abcdefghijklmnoqrstu";
    private const string Text20Wide5 = "あいうえお";
    private const string Text20Empty = "";

    private const string Ascii13 = "123456789012X";

    private const string Unicode30Wide15 = "○○○○○○○○○○○○○○○";

    private const short ZeroShort = 0;
    private const int ZeroInteger = 0;
    private const long ZeroLong = 0L;
    private const decimal ZeroDecimal = 0m;

    private const short Length4Integer = 9999;
    private const int Length8Integer = 99999999;
    private const long Length18Integer = 999999999999999999;

    private const decimal Length8Decimal = 999999.99m;
    private const decimal Length18Decimal = 999999999999999.999m;
    private const decimal Length28Decimal = 999999999999999999999999.9999m;

    private static readonly byte[] Bytes10 = new byte[10];

    private static readonly byte[] Bytes20 = new byte[20];

    private static readonly DateTime DateTime8 = new(2000, 12, 31);
    private static readonly DateTime DateTime14 = new(2000, 12, 31, 23, 59, 59);
    private static readonly DateTime DateTime17 = new(2000, 12, 31, 23, 59, 59, 999);

    // Default

    private IMapConverter intBinaryConverter;
    private byte[] intBinaryBuffer;

    private IMapConverter booleanConverter;
    private byte[] booleanBuffer;

    private IMapConverter bytes10Converter;
    private byte[] bytes10Buffer;
    private IMapConverter bytes20Converter;
    private byte[] bytes20Buffer;

    private IMapConverter text20Converter;
    private byte[] text20Single20Buffer;
    private byte[] text20Wide10Buffer;
    private byte[] text20EmptyBuffer;

    private IMapConverter numberTextShort4Converter;
    private byte[] numberTextShort4ZeroBuffer;
    private byte[] numberTextShort4MaxBuffer;

    private IMapConverter numberTextInt8Converter;
    private byte[] numberTextInt8ZeroBuffer;
    private byte[] numberTextInt8MaxBuffer;

    private IMapConverter numberTextLong18Converter;
    private byte[] numberTextLong18ZeroBuffer;
    private byte[] numberTextLong18MaxBuffer;

    private IMapConverter numberTextDecimal8Converter;
    private byte[] numberTextDecimal8ZeroBuffer;
    private byte[] numberTextDecimal8MaxBuffer;

    private IMapConverter numberTextDecimal18Converter;
    private byte[] numberTextDecimal18ZeroBuffer;
    private byte[] numberTextDecimal18MaxBuffer;

    private IMapConverter numberTextDecimal28Converter;
    private byte[] numberTextDecimal28ZeroBuffer;
    private byte[] numberTextDecimal28MaxBuffer;

    private IMapConverter dateTimeText8Converter;
    private byte[] dateTimeText8Buffer;

    private IMapConverter dateTimeText14Converter;
    private byte[] dateTimeText14Buffer;

    private IMapConverter dateTimeText17Converter;
    private byte[] dateTimeText17Buffer;

    // Options

    private IMapConverter text13Converter;
    private IMapConverter ascii13Converter;
    private byte[] ascii13Buffer;

    private IMapConverter text30Converter;
    private IMapConverter unicode30Converter;
    private byte[] unicode30Buffer;

    private IMapConverter short4Converter;
    private byte[] short4ZeroBuffer;
    private byte[] short4MaxBuffer;

    private IMapConverter int8Converter;
    private byte[] int8ZeroBuffer;
    private byte[] int8MaxBuffer;

    private IMapConverter long18Converter;
    private byte[] long18ZeroBuffer;
    private byte[] long18MaxBuffer;

    private IMapConverter decimal8Converter;
    private byte[] decimal8ZeroBuffer;
    private byte[] decimal8MaxBuffer;

    private IMapConverter decimal18Converter;
    private byte[] decimal18ZeroBuffer;
    private byte[] decimal18MaxBuffer;

    private IMapConverter decimal28Converter;
    private byte[] decimal28ZeroBuffer;
    private byte[] decimal28MaxBuffer;

    private IMapConverter dateTime8Converter;
    private byte[] dateTime8Buffer;

    private IMapConverter dateTime14Converter;
    private byte[] dateTime14Buffer;

    private IMapConverter dateTime17Converter;
    private byte[] dateTime17Buffer;

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

        // Default

        // Binary
        var intBinaryBuilder = new BinaryConverterBuilder();
        intBinaryConverter = intBinaryBuilder.CreateConverter(context, typeof(int));
        intBinaryBuffer = new byte[intBinaryBuilder.CalcSize(typeof(int))];

        // Boolean
        var booleanBuilder = new BooleanConverterBuilder();
        booleanConverter = booleanBuilder.CreateConverter(context, typeof(bool));
        booleanBuffer = [0x30];

        // bytes
        var bytes10Builder = new BytesConverterBuilder { Length = 10 };
        bytes10Converter = bytes10Builder.CreateConverter(context, typeof(byte[]));
        bytes10Buffer = new byte[bytes10Builder.CalcSize(typeof(byte[]))];

        var bytes20Builder = new BytesConverterBuilder { Length = 20 };
        bytes20Converter = bytes20Builder.CreateConverter(context, typeof(byte[]));
        bytes20Buffer = new byte[bytes20Builder.CalcSize(typeof(byte[]))];

        // Text
        var text20Builder = new TextConverterBuilder { Length = 20 };
        text20Converter = text20Builder.CreateConverter(context, typeof(string));
        text20Single20Buffer = SjisEncoding.GetFixedBytes(Text20Single10, 20, FixedAlignment.Left);
        text20Wide10Buffer = SjisEncoding.GetFixedBytes(Text20Wide5, 20, FixedAlignment.Left);
        text20EmptyBuffer = SjisEncoding.GetFixedBytes(Text20Empty, 20, FixedAlignment.Left);

        // Number
        var numberTextShort4Builder = new NumberTextConverterBuilder { Length = 4 };
        numberTextShort4Converter = numberTextShort4Builder.CreateConverter(context, typeof(short));
        numberTextShort4MaxBuffer = SjisEncoding.GetFixedBytes(Length4Integer.ToString(CultureInfo.InvariantCulture), 4, FixedAlignment.Right);
        numberTextShort4ZeroBuffer = SjisEncoding.GetFixedBytes(ZeroInteger.ToString(CultureInfo.InvariantCulture), 4, FixedAlignment.Right);

        var numberTextInt8Builder = new NumberTextConverterBuilder { Length = 8 };
        numberTextInt8Converter = numberTextInt8Builder.CreateConverter(context, typeof(int));
        numberTextInt8MaxBuffer = SjisEncoding.GetFixedBytes(Length8Integer.ToString(CultureInfo.InvariantCulture), 8, FixedAlignment.Right);
        numberTextInt8ZeroBuffer = SjisEncoding.GetFixedBytes(ZeroInteger.ToString(CultureInfo.InvariantCulture), 8, FixedAlignment.Right);

        var numberTextLong18Builder = new NumberTextConverterBuilder { Length = 18 };
        numberTextLong18Converter = numberTextLong18Builder.CreateConverter(context, typeof(long));
        numberTextLong18MaxBuffer = SjisEncoding.GetFixedBytes(Length18Integer.ToString(CultureInfo.InvariantCulture), 18, FixedAlignment.Right);
        numberTextLong18ZeroBuffer = SjisEncoding.GetFixedBytes(ZeroInteger.ToString(CultureInfo.InvariantCulture), 18, FixedAlignment.Right);

        var numberTextDecimal8Builder = new NumberTextConverterBuilder { Length = 8 };
        numberTextDecimal8Converter = numberTextDecimal8Builder.CreateConverter(context, typeof(decimal));
        numberTextDecimal8MaxBuffer = SjisEncoding.GetFixedBytes(Length8Decimal.ToString(CultureInfo.InvariantCulture), 8, FixedAlignment.Right);
        numberTextDecimal8ZeroBuffer = SjisEncoding.GetFixedBytes("0.00", 10, FixedAlignment.Right);

        var numberTextDecimal18Builder = new NumberTextConverterBuilder { Length = 18 };
        numberTextDecimal18Converter = numberTextDecimal18Builder.CreateConverter(context, typeof(decimal));
        numberTextDecimal18MaxBuffer = SjisEncoding.GetFixedBytes(Length18Decimal.ToString(CultureInfo.InvariantCulture), 19, FixedAlignment.Right);
        numberTextDecimal18ZeroBuffer = SjisEncoding.GetFixedBytes("0.000", 19, FixedAlignment.Right);

        var numberTextDecimal28Builder = new NumberTextConverterBuilder { Length = 28 };
        numberTextDecimal28Converter = numberTextDecimal28Builder.CreateConverter(context, typeof(decimal));
        numberTextDecimal28MaxBuffer = SjisEncoding.GetFixedBytes(Length28Decimal.ToString(CultureInfo.InvariantCulture), 29, FixedAlignment.Right);
        numberTextDecimal28ZeroBuffer = SjisEncoding.GetFixedBytes("0.0000", 29, FixedAlignment.Right);

        // DateTime
        var dateTimeText8Builder = new DateTimeTextConverterBuilder { Length = 8, Format = "yyyyMMdd" };
        dateTimeText8Converter = dateTimeText8Builder.CreateConverter(context, typeof(DateTime));
        dateTimeText8Buffer = SjisEncoding.GetFixedBytes(DateTime8.ToString("yyyyMMdd"), 8, FixedAlignment.Left);

        var dateTimeText14Builder = new DateTimeTextConverterBuilder { Length = 14, Format = "yyyyMMddHHmmss" };
        dateTimeText14Converter = dateTimeText14Builder.CreateConverter(context, typeof(DateTime));
        dateTimeText14Buffer = SjisEncoding.GetFixedBytes(DateTime14.ToString("yyyyMMddHHmmss"), 14, FixedAlignment.Left);

        var dateTimeText17Builder = new DateTimeTextConverterBuilder { Length = 17, Format = "yyyyMMddHHmmssfff" };
        dateTimeText17Converter = dateTimeText17Builder.CreateConverter(context, typeof(DateTime));
        dateTimeText17Buffer = SjisEncoding.GetFixedBytes(DateTime17.ToString("yyyyMMddHHmmssfff"), 17, FixedAlignment.Left);

        // Options

        // ASCII
        var text13Builder = new TextConverterBuilder { Length = 13, Encoding = Encoding.ASCII };
        text13Converter = text13Builder.CreateConverter(context, typeof(string));
        var ascii13Builder = new AsciiConverterBuilder { Length = 13 };
        ascii13Converter = ascii13Builder.CreateConverter(context, typeof(string));
        ascii13Buffer = new byte[ascii13Builder.CalcSize(typeof(string))];

        // Unicode
        var text30Builder = new TextConverterBuilder { Length = 30, Encoding = Encoding.Unicode };
        text30Converter = text30Builder.CreateConverter(context, typeof(string));
        var ascii30Builder = new UnicodeConverterBuilder { Length = 30 };
        unicode30Converter = ascii30Builder.CreateConverter(context, typeof(string));
        unicode30Buffer = new byte[ascii30Builder.CalcSize(typeof(string))];

        // Integer
        var short4Builder = new IntegerConverterBuilder { Length = 4 };
        short4Converter = short4Builder.CreateConverter(context, typeof(short));
        short4MaxBuffer = SjisEncoding.GetFixedBytes(Length4Integer.ToString(CultureInfo.InvariantCulture), 4, FixedAlignment.Right);
        short4ZeroBuffer = SjisEncoding.GetFixedBytes(ZeroShort.ToString(CultureInfo.InvariantCulture), 4, FixedAlignment.Right);

        var int8Builder = new IntegerConverterBuilder { Length = 8 };
        int8Converter = int8Builder.CreateConverter(context, typeof(int));
        int8MaxBuffer = SjisEncoding.GetFixedBytes(Length8Integer.ToString(CultureInfo.InvariantCulture), 8, FixedAlignment.Right);
        int8ZeroBuffer = SjisEncoding.GetFixedBytes(ZeroInteger.ToString(CultureInfo.InvariantCulture), 8, FixedAlignment.Right);

        var long18Builder = new IntegerConverterBuilder { Length = 18 };
        long18Converter = long18Builder.CreateConverter(context, typeof(long));
        long18MaxBuffer = SjisEncoding.GetFixedBytes(Length18Integer.ToString(CultureInfo.InvariantCulture), 18, FixedAlignment.Right);
        long18ZeroBuffer = SjisEncoding.GetFixedBytes(ZeroLong.ToString(CultureInfo.InvariantCulture), 18, FixedAlignment.Right);

        // Decimal
        var decimal8Builder = new DecimalConverterBuilder { Length = 10, Scale = 2 };
        decimal8Converter = decimal8Builder.CreateConverter(context, typeof(decimal));
        decimal8MaxBuffer = SjisEncoding.GetFixedBytes(Length8Decimal.ToString(CultureInfo.InvariantCulture), 10, FixedAlignment.Right);
        decimal8ZeroBuffer = SjisEncoding.GetFixedBytes("0.00", 10, FixedAlignment.Right);

        var decimal18Builder = new DecimalConverterBuilder { Length = 19, Scale = 3 };
        decimal18Converter = decimal18Builder.CreateConverter(context, typeof(decimal));
        decimal18MaxBuffer = SjisEncoding.GetFixedBytes(Length18Decimal.ToString(CultureInfo.InvariantCulture), 19, FixedAlignment.Right);
        decimal18ZeroBuffer = SjisEncoding.GetFixedBytes("0.000", 19, FixedAlignment.Right);

        var decimal28Builder = new DecimalConverterBuilder { Length = 29, Scale = 4 };
        decimal28Converter = decimal28Builder.CreateConverter(context, typeof(decimal));
        decimal28MaxBuffer = SjisEncoding.GetFixedBytes(Length28Decimal.ToString(CultureInfo.InvariantCulture), 29, FixedAlignment.Right);
        decimal28ZeroBuffer = SjisEncoding.GetFixedBytes("0.0000", 29, FixedAlignment.Right);

        // DateTime
        var dateTime8Builder = new DateTimeConverterBuilder { Format = "yyyyMMdd" };
        dateTime8Converter = dateTime8Builder.CreateConverter(context, typeof(DateTime));
        dateTime8Buffer = SjisEncoding.GetFixedBytes(DateTime8.ToString("yyyyMMdd"), 8, FixedAlignment.Left);

        var dateTime14Builder = new DateTimeConverterBuilder { Format = "yyyyMMddHHmmss" };
        dateTime14Converter = dateTime14Builder.CreateConverter(context, typeof(DateTime));
        dateTime14Buffer = SjisEncoding.GetFixedBytes(DateTime14.ToString("yyyyMMddHHmmss"), 14, FixedAlignment.Left);

        var dateTime17Builder = new DateTimeConverterBuilder { Format = "yyyyMMddHHmmssfff" };
        dateTime17Converter = dateTime17Builder.CreateConverter(context, typeof(DateTime));
        dateTime17Buffer = SjisEncoding.GetFixedBytes(DateTime17.ToString("yyyyMMddHHmmssfff"), 17, FixedAlignment.Left);
    }

    //--------------------------------------------------------------------------------
    // Read
    //--------------------------------------------------------------------------------

    // Binary

    [Benchmark(OperationsPerInvoke = N)]

    public void ReadIntBinary()
    {
        var c = intBinaryConverter;
        var buffer = intBinaryBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Read(buffer, 0);
        }
    }

    // Boolean

    [Benchmark(OperationsPerInvoke = N)]

    public void ReadBoolean()
    {
        var c = booleanConverter;
        var buffer = booleanBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Read(buffer, 0);
        }
    }

    // Bytes

    [Benchmark(OperationsPerInvoke = N)]
    public void ReadBytes10()
    {
        var c = bytes10Converter;
        var buffer = bytes10Buffer;
        for (var i = 0; i < N; i++)
        {
            c.Read(buffer, 0);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void ReadBytes20()
    {
        var c = bytes20Converter;
        var buffer = bytes20Buffer;
        for (var i = 0; i < N; i++)
        {
            c.Read(buffer, 0);
        }
    }

    // Text

    [Benchmark(OperationsPerInvoke = N)]
    public void ReadSjisText20Single20()
    {
        var c = text20Converter;
        var buffer = text20Single20Buffer;
        for (var i = 0; i < N; i++)
        {
            c.Read(buffer, 0);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void ReadSjisText20Wide5()
    {
        var c = text20Converter;
        var buffer = text20Wide10Buffer;
        for (var i = 0; i < N; i++)
        {
            c.Read(buffer, 0);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void ReadSjisText20Empty()
    {
        var c = text20Converter;
        var buffer = text20EmptyBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Read(buffer, 0);
        }
    }

    // Number

    [Benchmark(OperationsPerInvoke = N)]
    public void ReadNumberTextShort4Zero()
    {
        var c = numberTextShort4Converter;
        var buffer = numberTextShort4ZeroBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Read(buffer, 0);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void ReadNumberTextShort4Max()
    {
        var c = numberTextShort4Converter;
        var buffer = numberTextShort4MaxBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Read(buffer, 0);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void ReadNumberTextInt8Zero()
    {
        var c = numberTextInt8Converter;
        var buffer = numberTextInt8ZeroBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Read(buffer, 0);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void ReadNumberTextInt8Max()
    {
        var c = numberTextInt8Converter;
        var buffer = numberTextInt8MaxBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Read(buffer, 0);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void ReadNumberTextLong18Zero()
    {
        var c = numberTextLong18Converter;
        var buffer = numberTextLong18ZeroBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Read(buffer, 0);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void ReadNumberTextLong18Max()
    {
        var c = numberTextLong18Converter;
        var buffer = numberTextLong18MaxBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Read(buffer, 0);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void ReadNumberTextDecimal8Zero()
    {
        var c = numberTextDecimal8Converter;
        var buffer = numberTextDecimal8ZeroBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Read(buffer, 0);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void ReadNumberTextDecimal8Max()
    {
        var c = numberTextDecimal8Converter;
        var buffer = numberTextDecimal8MaxBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Read(buffer, 0);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void ReadNumberTextDecimal18Zero()
    {
        var c = numberTextDecimal18Converter;
        var buffer = numberTextDecimal18ZeroBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Read(buffer, 0);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void ReadNumberTextDecimal18Max()
    {
        var c = numberTextDecimal18Converter;
        var buffer = numberTextDecimal18MaxBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Read(buffer, 0);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void ReadNumberTextDecimal28Zero()
    {
        var c = numberTextDecimal28Converter;
        var buffer = numberTextDecimal28ZeroBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Read(buffer, 0);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void ReadNumberTextDecimal28Max()
    {
        var c = numberTextDecimal28Converter;
        var buffer = numberTextDecimal28MaxBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Read(buffer, 0);
        }
    }

    // DateTime

    [Benchmark(OperationsPerInvoke = N)]
    public void ReadDateTimeText8()
    {
        var c = dateTimeText8Converter;
        var buffer = dateTimeText8Buffer;
        for (var i = 0; i < N; i++)
        {
            c.Read(buffer, 0);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void ReadDateTimeText14()
    {
        var c = dateTimeText14Converter;
        var buffer = dateTimeText14Buffer;
        for (var i = 0; i < N; i++)
        {
            c.Read(buffer, 0);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void ReadDateTimeText17()
    {
        var c = dateTimeText17Converter;
        var buffer = dateTimeText17Buffer;
        for (var i = 0; i < N; i++)
        {
            c.Read(buffer, 0);
        }
    }

    //--------------------------------------------------------------------------------
    // Read.Options
    //--------------------------------------------------------------------------------

    // ASCII

    [Benchmark(OperationsPerInvoke = N)]
    public void ReadText13Code()
    {
        var c = text13Converter;
        var buffer = ascii13Buffer;
        for (var i = 0; i < N; i++)
        {
            c.Read(buffer, 0);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void ReadAscii13Code()
    {
        var c = ascii13Converter;
        var buffer = ascii13Buffer;
        for (var i = 0; i < N; i++)
        {
            c.Read(buffer, 0);
        }
    }

    // Unicode

    [Benchmark(OperationsPerInvoke = N)]
    public void ReadText30Wide15()
    {
        var c = text30Converter;
        var buffer = unicode30Buffer;
        for (var i = 0; i < N; i++)
        {
            c.Read(buffer, 0);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void ReadUnicode30Wide15()
    {
        var c = unicode30Converter;
        var buffer = unicode30Buffer;
        for (var i = 0; i < N; i++)
        {
            c.Read(buffer, 0);
        }
    }

    // Integer

    [Benchmark(OperationsPerInvoke = N)]
    public void ReadIntegerShort4Zero()
    {
        var c = short4Converter;
        var buffer = short4ZeroBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Read(buffer, 0);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void ReadIntegerShort4Max()
    {
        var c = short4Converter;
        var buffer = short4MaxBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Read(buffer, 0);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void ReadInteger8Zero()
    {
        var c = int8Converter;
        var buffer = int8ZeroBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Read(buffer, 0);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void ReadInteger8Max()
    {
        var c = int8Converter;
        var buffer = int8MaxBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Read(buffer, 0);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void ReadLong18Zero()
    {
        var c = long18Converter;
        var buffer = long18ZeroBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Read(buffer, 0);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void ReadLong18Max()
    {
        var c = long18Converter;
        var buffer = long18MaxBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Read(buffer, 0);
        }
    }

    // Decimal

    [Benchmark(OperationsPerInvoke = N)]
    public void ReadDecimal8Zero()
    {
        var c = decimal8Converter;
        var buffer = decimal8ZeroBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Read(buffer, 0);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void ReadDecimal8Max()
    {
        var c = decimal8Converter;
        var buffer = decimal8MaxBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Read(buffer, 0);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void ReadDecimal18Zero()
    {
        var c = decimal18Converter;
        var buffer = decimal18ZeroBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Read(buffer, 0);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void ReadDecimal18Max()
    {
        var c = decimal18Converter;
        var buffer = decimal18MaxBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Read(buffer, 0);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void ReadDecimal28Zero()
    {
        var c = decimal28Converter;
        var buffer = decimal28ZeroBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Read(buffer, 0);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void ReadDecimal28Max()
    {
        var c = decimal28Converter;
        var buffer = decimal28MaxBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Read(buffer, 0);
        }
    }

    // DateTime

    [Benchmark(OperationsPerInvoke = N)]
    public void ReadDateTime8()
    {
        var c = dateTime8Converter;
        var buffer = dateTime8Buffer;
        for (var i = 0; i < N; i++)
        {
            c.Read(buffer, 0);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void ReadDateTime14()
    {
        var c = dateTime14Converter;
        var buffer = dateTime14Buffer;
        for (var i = 0; i < N; i++)
        {
            c.Read(buffer, 0);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void ReadDateTime17()
    {
        var c = dateTime17Converter;
        var buffer = dateTime17Buffer;
        for (var i = 0; i < N; i++)
        {
            c.Read(buffer, 0);
        }
    }

    //--------------------------------------------------------------------------------
    // Write
    //--------------------------------------------------------------------------------

    // Binary

    [Benchmark(OperationsPerInvoke = N)]

    public void WriteIntBinary()
    {
        var c = intBinaryConverter;
        var buffer = dateTime17Buffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer, 0, 0);
        }
    }

    // Boolean

    [Benchmark(OperationsPerInvoke = N)]

    public void WriteBoolean()
    {
        var c = booleanConverter;
        var buffer = booleanBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer, 0, false);
        }
    }

    // Bytes

    [Benchmark(OperationsPerInvoke = N)]
    public void WriteBytes10()
    {
        var c = bytes10Converter;
        var buffer = bytes10Buffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer, 0, Bytes10);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void WriteBytes20()
    {
        var c = bytes20Converter;
        var buffer = bytes20Buffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer, 0, Bytes20);
        }
    }

    // Text

    [Benchmark(OperationsPerInvoke = N)]
    public void WriteSjisText20Single20()
    {
        var c = text20Converter;
        var buffer = text20Single20Buffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer, 0, Text20Single10);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void WriteSjisText20Wide5()
    {
        var c = text20Converter;
        var buffer = text20Wide10Buffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer, 0, Text20Wide5);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void WriteSjisText20Empty()
    {
        var c = text20Converter;
        var buffer = text20EmptyBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer, 0, Text20Empty);
        }
    }

    // Number

    [Benchmark(OperationsPerInvoke = N)]
    public void WriteNumberTextShort4Zero()
    {
        var c = numberTextShort4Converter;
        var buffer = numberTextShort4ZeroBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer, 0, ZeroShort);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void WriteNumberTextShort4Max()
    {
        var c = numberTextShort4Converter;
        var buffer = numberTextShort4MaxBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer, 0, Length4Integer);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void WriteNumberTextInt8Zero()
    {
        var c = numberTextInt8Converter;
        var buffer = numberTextInt8ZeroBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer, 0, ZeroInteger);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void WriteNumberTextInt8Max()
    {
        var c = numberTextInt8Converter;
        var buffer = numberTextInt8MaxBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer, 0, Length8Integer);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void WriteNumberTextLong18Zero()
    {
        var c = numberTextLong18Converter;
        var buffer = numberTextLong18ZeroBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer, 0, ZeroLong);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void WriteNumberTextLong18Max()
    {
        var c = numberTextLong18Converter;
        var buffer = numberTextLong18MaxBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer, 0, Length18Integer);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void WriteNumberTextDecimal8Zero()
    {
        var c = numberTextDecimal8Converter;
        var buffer = numberTextDecimal8ZeroBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer, 0, ZeroDecimal);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void WriteNumberTextDecimal8Max()
    {
        var c = numberTextDecimal8Converter;
        var buffer = numberTextDecimal8MaxBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer, 0, Length8Decimal);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void WriteNumberTextDecimal18Zero()
    {
        var c = numberTextDecimal18Converter;
        var buffer = numberTextDecimal18ZeroBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer, 0, ZeroDecimal);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void WriteNumberTextDecimal18Max()
    {
        var c = numberTextDecimal18Converter;
        var buffer = numberTextDecimal18MaxBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer, 0, Length18Decimal);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void WriteNumberTextDecimal28Zero()
    {
        var c = numberTextDecimal28Converter;
        var buffer = numberTextDecimal28ZeroBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer, 0, ZeroDecimal);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void WriteNumberTextDecimal28Max()
    {
        var c = numberTextDecimal28Converter;
        var buffer = numberTextDecimal28MaxBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer, 0, Length28Decimal);
        }
    }

    // DateTime

    [Benchmark(OperationsPerInvoke = N)]
    public void WriteDateTimeText8()
    {
        var c = dateTimeText8Converter;
        var buffer = dateTimeText8Buffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer, 0, DateTime8);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void WriteDateTimeText14()
    {
        var c = dateTimeText14Converter;
        var buffer = dateTimeText14Buffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer, 0, DateTime14);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void WriteDateTimeText17()
    {
        var c = dateTimeText17Converter;
        var buffer = dateTimeText17Buffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer, 0, DateTime17);
        }
    }

    //--------------------------------------------------------------------------------
    // Write.Options
    //--------------------------------------------------------------------------------

    // ASCII

    [Benchmark(OperationsPerInvoke = N)]
    public void WriteText13Code()
    {
        var c = text13Converter;
        var buffer = ascii13Buffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer, 0, Ascii13);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void WriteAscii13Code()
    {
        var c = ascii13Converter;
        var buffer = ascii13Buffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer, 0, Ascii13);
        }
    }

    // Unicode

    [Benchmark(OperationsPerInvoke = N)]
    public void WriteText30Wide15()
    {
        var c = text30Converter;
        var buffer = unicode30Buffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer, 0, Unicode30Wide15);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void WriteUnicode30Wide15()
    {
        var c = unicode30Converter;
        var buffer = unicode30Buffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer, 0, Unicode30Wide15);
        }
    }

    // Integer

    [Benchmark(OperationsPerInvoke = N)]
    public void WriteIntegerShort4Zero()
    {
        var c = short4Converter;
        var buffer = short4ZeroBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer, 0, ZeroShort);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void WriteIntegerShort4Max()
    {
        var c = short4Converter;
        var buffer = short4MaxBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer, 0, Length4Integer);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void WriteInteger8Zero()
    {
        var c = int8Converter;
        var buffer = int8ZeroBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer, 0, ZeroInteger);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void WriteInteger8Max()
    {
        var c = int8Converter;
        var buffer = int8MaxBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer, 0, Length8Integer);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void WriteLong18Zero()
    {
        var c = long18Converter;
        var buffer = long18ZeroBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer, 0, ZeroLong);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void WriteLong18Max()
    {
        var c = long18Converter;
        var buffer = long18MaxBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer, 0, Length18Integer);
        }
    }

    // Decimal

    [Benchmark(OperationsPerInvoke = N)]
    public void WriteDecimal8Zero()
    {
        var c = decimal8Converter;
        var buffer = decimal8ZeroBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer, 0, ZeroDecimal);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void WriteDecimal8Max()
    {
        var c = decimal8Converter;
        var buffer = decimal8MaxBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer, 0, Length8Decimal);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void WriteDecimal18Zero()
    {
        var c = decimal18Converter;
        var buffer = decimal18ZeroBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer, 0, ZeroDecimal);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void WriteDecimal18Max()
    {
        var c = decimal18Converter;
        var buffer = decimal18MaxBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer, 0, Length18Decimal);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void WriteDecimal28Zero()
    {
        var c = decimal28Converter;
        var buffer = decimal28ZeroBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer, 0, ZeroDecimal);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void WriteDecimal28Max()
    {
        var c = decimal28Converter;
        var buffer = decimal28MaxBuffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer, 0, Length28Decimal);
        }
    }

    // DateTime

    [Benchmark(OperationsPerInvoke = N)]
    public void WriteDateTime8()
    {
        var c = dateTime8Converter;
        var buffer = dateTime8Buffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer, 0, DateTime8);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void WriteDateTime14()
    {
        var c = dateTime14Converter;
        var buffer = dateTime14Buffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer, 0, DateTime14);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void WriteDateTime17()
    {
        var c = dateTime17Converter;
        var buffer = dateTime17Buffer;
        for (var i = 0; i < N; i++)
        {
            c.Write(buffer, 0, DateTime17);
        }
    }
}
