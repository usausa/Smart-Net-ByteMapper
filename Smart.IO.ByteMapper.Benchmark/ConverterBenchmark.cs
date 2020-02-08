namespace Smart.IO.ByteMapper.Benchmark
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    using BenchmarkDotNet.Attributes;

    using Smart.IO.ByteMapper;
    using Smart.IO.ByteMapper.Builders;
    using Smart.IO.ByteMapper.Converters;
    using Smart.Text.Japanese;

    [Config(typeof(BenchmarkConfig))]
    public class ConverterBenchmark
    {
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

        private static readonly DateTime DateTime8 = new DateTime(2000, 12, 31);
        private static readonly DateTime DateTime14 = new DateTime(2000, 12, 31, 23, 59, 59);
        private static readonly DateTime DateTime17 = new DateTime(2000, 12, 31, 23, 59, 59, 999);

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

        private static IBuilderContext CreateBuilderContext()
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
            booleanBuffer = new byte[] { 0x30 };

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
            text20Single20Buffer = SjisEncoding.Instance.GetFixedBytes(Text20Single10, 0, 20);
            text20Wide10Buffer = SjisEncoding.Instance.GetFixedBytes(Text20Wide5, 0, 20);
            text20EmptyBuffer = SjisEncoding.Instance.GetFixedBytes(Text20Empty, 0, 20);

            // Number
            var numberTextShort4Builder = new NumberTextConverterBuilder { Length = 4 };
            numberTextShort4Converter = numberTextShort4Builder.CreateConverter(context, typeof(short));
            numberTextShort4MaxBuffer = SjisEncoding.Instance.GetFixedBytes(Length4Integer.ToString(CultureInfo.InvariantCulture), 4, FixedAlignment.Right);
            numberTextShort4ZeroBuffer = SjisEncoding.Instance.GetFixedBytes(ZeroInteger.ToString(CultureInfo.InvariantCulture), 4, FixedAlignment.Right);

            var numberTextInt8Builder = new NumberTextConverterBuilder { Length = 8 };
            numberTextInt8Converter = numberTextInt8Builder.CreateConverter(context, typeof(int));
            numberTextInt8MaxBuffer = SjisEncoding.Instance.GetFixedBytes(Length8Integer.ToString(CultureInfo.InvariantCulture), 8, FixedAlignment.Right);
            numberTextInt8ZeroBuffer = SjisEncoding.Instance.GetFixedBytes(ZeroInteger.ToString(CultureInfo.InvariantCulture), 8, FixedAlignment.Right);

            var numberTextLong18Builder = new NumberTextConverterBuilder { Length = 18 };
            numberTextLong18Converter = numberTextLong18Builder.CreateConverter(context, typeof(long));
            numberTextLong18MaxBuffer = SjisEncoding.Instance.GetFixedBytes(Length18Integer.ToString(CultureInfo.InvariantCulture), 18, FixedAlignment.Right);
            numberTextLong18ZeroBuffer = SjisEncoding.Instance.GetFixedBytes(ZeroInteger.ToString(CultureInfo.InvariantCulture), 18, FixedAlignment.Right);

            var numberTextDecimal8Builder = new NumberTextConverterBuilder { Length = 8 };
            numberTextDecimal8Converter = numberTextDecimal8Builder.CreateConverter(context, typeof(decimal));
            numberTextDecimal8MaxBuffer = SjisEncoding.Instance.GetFixedBytes(Length8Decimal.ToString(CultureInfo.InvariantCulture), 8, FixedAlignment.Right);
            numberTextDecimal8ZeroBuffer = SjisEncoding.Instance.GetFixedBytes("0.00", 10, FixedAlignment.Right);

            var numberTextDecimal18Builder = new NumberTextConverterBuilder { Length = 18 };
            numberTextDecimal18Converter = numberTextDecimal18Builder.CreateConverter(context, typeof(decimal));
            numberTextDecimal18MaxBuffer = SjisEncoding.Instance.GetFixedBytes(Length18Decimal.ToString(CultureInfo.InvariantCulture), 19, FixedAlignment.Right);
            numberTextDecimal18ZeroBuffer = SjisEncoding.Instance.GetFixedBytes("0.000", 19, FixedAlignment.Right);

            var numberTextDecimal28Builder = new NumberTextConverterBuilder { Length = 28 };
            numberTextDecimal28Converter = numberTextDecimal28Builder.CreateConverter(context, typeof(decimal));
            numberTextDecimal28MaxBuffer = SjisEncoding.Instance.GetFixedBytes(Length28Decimal.ToString(CultureInfo.InvariantCulture), 29, FixedAlignment.Right);
            numberTextDecimal28ZeroBuffer = SjisEncoding.Instance.GetFixedBytes("0.0000", 29, FixedAlignment.Right);

            // DateTime
            var dateTimeText8Builder = new DateTimeTextConverterBuilder { Length = 8, Format = "yyyyMMdd" };
            dateTimeText8Converter = dateTimeText8Builder.CreateConverter(context, typeof(DateTime));
            dateTimeText8Buffer = SjisEncoding.Instance.GetFixedBytes(DateTime8.ToString("yyyyMMdd"), 0, 8);

            var dateTimeText14Builder = new DateTimeTextConverterBuilder { Length = 14, Format = "yyyyMMddHHmmss" };
            dateTimeText14Converter = dateTimeText14Builder.CreateConverter(context, typeof(DateTime));
            dateTimeText14Buffer = SjisEncoding.Instance.GetFixedBytes(DateTime14.ToString("yyyyMMddHHmmss"), 0, 14);

            var dateTimeText17Builder = new DateTimeTextConverterBuilder { Length = 17, Format = "yyyyMMddHHmmssfff" };
            dateTimeText17Converter = dateTimeText17Builder.CreateConverter(context, typeof(DateTime));
            dateTimeText17Buffer = SjisEncoding.Instance.GetFixedBytes(DateTime17.ToString("yyyyMMddHHmmssfff"), 0, 17);

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
            short4MaxBuffer = SjisEncoding.Instance.GetFixedBytes(Length4Integer.ToString(CultureInfo.InvariantCulture), 4, FixedAlignment.Right);
            short4ZeroBuffer = SjisEncoding.Instance.GetFixedBytes(ZeroShort.ToString(CultureInfo.InvariantCulture), 4, FixedAlignment.Right);

            var int8Builder = new IntegerConverterBuilder { Length = 8 };
            int8Converter = int8Builder.CreateConverter(context, typeof(int));
            int8MaxBuffer = SjisEncoding.Instance.GetFixedBytes(Length8Integer.ToString(CultureInfo.InvariantCulture), 8, FixedAlignment.Right);
            int8ZeroBuffer = SjisEncoding.Instance.GetFixedBytes(ZeroInteger.ToString(CultureInfo.InvariantCulture), 8, FixedAlignment.Right);

            var long18Builder = new IntegerConverterBuilder { Length = 18 };
            long18Converter = long18Builder.CreateConverter(context, typeof(long));
            long18MaxBuffer = SjisEncoding.Instance.GetFixedBytes(Length18Integer.ToString(CultureInfo.InvariantCulture), 18, FixedAlignment.Right);
            long18ZeroBuffer = SjisEncoding.Instance.GetFixedBytes(ZeroLong.ToString(CultureInfo.InvariantCulture), 18, FixedAlignment.Right);

            // Decimal
            var decimal8Builder = new DecimalConverterBuilder { Length = 10, Scale = 2 };
            decimal8Converter = decimal8Builder.CreateConverter(context, typeof(decimal));
            decimal8MaxBuffer = SjisEncoding.Instance.GetFixedBytes(Length8Decimal.ToString(CultureInfo.InvariantCulture), 10, FixedAlignment.Right);
            decimal8ZeroBuffer = SjisEncoding.Instance.GetFixedBytes("0.00", 10, FixedAlignment.Right);

            var decimal18Builder = new DecimalConverterBuilder { Length = 19, Scale = 3 };
            decimal18Converter = decimal18Builder.CreateConverter(context, typeof(decimal));
            decimal18MaxBuffer = SjisEncoding.Instance.GetFixedBytes(Length18Decimal.ToString(CultureInfo.InvariantCulture), 19, FixedAlignment.Right);
            decimal18ZeroBuffer = SjisEncoding.Instance.GetFixedBytes("0.000", 19, FixedAlignment.Right);

            var decimal28Builder = new DecimalConverterBuilder { Length = 29, Scale = 4 };
            decimal28Converter = decimal28Builder.CreateConverter(context, typeof(decimal));
            decimal28MaxBuffer = SjisEncoding.Instance.GetFixedBytes(Length28Decimal.ToString(CultureInfo.InvariantCulture), 29, FixedAlignment.Right);
            decimal28ZeroBuffer = SjisEncoding.Instance.GetFixedBytes("0.0000", 29, FixedAlignment.Right);

            // DateTime
            var dateTime8Builder = new DateTimeConverterBuilder { Format = "yyyyMMdd" };
            dateTime8Converter = dateTime8Builder.CreateConverter(context, typeof(DateTime));
            dateTime8Buffer = SjisEncoding.Instance.GetFixedBytes(DateTime8.ToString("yyyyMMdd"), 0, 8);

            var dateTime14Builder = new DateTimeConverterBuilder { Format = "yyyyMMddHHmmss" };
            dateTime14Converter = dateTime14Builder.CreateConverter(context, typeof(DateTime));
            dateTime14Buffer = SjisEncoding.Instance.GetFixedBytes(DateTime14.ToString("yyyyMMddHHmmss"), 0, 14);

            var dateTime17Builder = new DateTimeConverterBuilder { Format = "yyyyMMddHHmmssfff" };
            dateTime17Converter = dateTime17Builder.CreateConverter(context, typeof(DateTime));
            dateTime17Buffer = SjisEncoding.Instance.GetFixedBytes(DateTime17.ToString("yyyyMMddHHmmssfff"), 0, 17);
        }

        //--------------------------------------------------------------------------------
        // Read
        //--------------------------------------------------------------------------------

        // Binary

        [Benchmark]

        public void ReadIntBinary()
        {
            intBinaryConverter.Read(intBinaryBuffer, 0);
        }

        // Boolean

        [Benchmark]

        public void ReadBoolean()
        {
            booleanConverter.Read(booleanBuffer, 0);
        }

        // Bytes

        [Benchmark]
        public void ReadBytes10()
        {
            bytes10Converter.Read(bytes10Buffer, 0);
        }

        [Benchmark]
        public void ReadBytes20()
        {
            bytes20Converter.Read(bytes20Buffer, 0);
        }

        // Text

        [Benchmark]
        public void ReadSjisText20Single20()
        {
            text20Converter.Read(text20Single20Buffer, 0);
        }

        [Benchmark]
        public void ReadSjisText20Wide5()
        {
            text20Converter.Read(text20Wide10Buffer, 0);
        }

        [Benchmark]
        public void ReadSjisText20Empty()
        {
            text20Converter.Read(text20EmptyBuffer, 0);
        }

        // Number

        [Benchmark]
        public void ReadNumberTextShort4Zero()
        {
            numberTextShort4Converter.Read(numberTextShort4ZeroBuffer, 0);
        }

        [Benchmark]
        public void ReadNumberTextShort4Max()
        {
            numberTextShort4Converter.Read(numberTextShort4MaxBuffer, 0);
        }

        [Benchmark]
        public void ReadNumberTextInt8Zero()
        {
            numberTextInt8Converter.Read(numberTextInt8ZeroBuffer, 0);
        }

        [Benchmark]
        public void ReadNumberTextInt8Max()
        {
            numberTextInt8Converter.Read(numberTextInt8MaxBuffer, 0);
        }

        [Benchmark]
        public void ReadNumberTextLong18Zero()
        {
            numberTextLong18Converter.Read(numberTextLong18ZeroBuffer, 0);
        }

        [Benchmark]
        public void ReadNumberTextLong18Max()
        {
            numberTextLong18Converter.Read(numberTextLong18MaxBuffer, 0);
        }

        [Benchmark]
        public void ReadNumberTextDecimal8Zero()
        {
            numberTextDecimal8Converter.Read(numberTextDecimal8ZeroBuffer, 0);
        }

        [Benchmark]
        public void ReadNumberTextDecimal8Max()
        {
            numberTextDecimal8Converter.Read(numberTextDecimal8MaxBuffer, 0);
        }

        [Benchmark]
        public void ReadNumberTextDecimal18Zero()
        {
            numberTextDecimal18Converter.Read(numberTextDecimal18ZeroBuffer, 0);
        }

        [Benchmark]
        public void ReadNumberTextDecimal18Max()
        {
            numberTextDecimal18Converter.Read(numberTextDecimal18MaxBuffer, 0);
        }

        [Benchmark]
        public void ReadNumberTextDecimal28Zero()
        {
            numberTextDecimal28Converter.Read(numberTextDecimal28ZeroBuffer, 0);
        }

        [Benchmark]
        public void ReadNumberTextDecimal28Max()
        {
            numberTextDecimal28Converter.Read(numberTextDecimal28MaxBuffer, 0);
        }

        // DateTime

        [Benchmark]
        public void ReadDateTimeText8()
        {
            dateTimeText8Converter.Read(dateTimeText8Buffer, 0);
        }

        [Benchmark]
        public void ReadDateTimeText14()
        {
            dateTimeText14Converter.Read(dateTimeText14Buffer, 0);
        }

        [Benchmark]
        public void ReadDateTimeText17()
        {
            dateTimeText17Converter.Read(dateTimeText17Buffer, 0);
        }

        //--------------------------------------------------------------------------------
        // Read.Options
        //--------------------------------------------------------------------------------

        // ASCII

        [Benchmark]
        public void ReadText13Code()
        {
            text13Converter.Read(ascii13Buffer, 0);
        }

        [Benchmark]
        public void ReadAscii13Code()
        {
            ascii13Converter.Read(ascii13Buffer, 0);
        }

        // Unicode

        [Benchmark]
        public void ReadText30Wide15()
        {
            text30Converter.Read(unicode30Buffer, 0);
        }

        [Benchmark]
        public void ReadUnicode30Wide15()
        {
            unicode30Converter.Read(unicode30Buffer, 0);
        }

        // Integer

        [Benchmark]
        public void ReadIntegerShort4Zero()
        {
            short4Converter.Read(short4ZeroBuffer, 0);
        }

        [Benchmark]
        public void ReadIntegerShort4Max()
        {
            short4Converter.Read(short4MaxBuffer, 0);
        }

        [Benchmark]
        public void ReadInteger8Zero()
        {
            int8Converter.Read(int8ZeroBuffer, 0);
        }

        [Benchmark]
        public void ReadInteger8Max()
        {
            int8Converter.Read(int8MaxBuffer, 0);
        }

        [Benchmark]
        public void ReadLong18Zero()
        {
            long18Converter.Read(long18ZeroBuffer, 0);
        }

        [Benchmark]
        public void ReadLong18Max()
        {
            long18Converter.Read(long18MaxBuffer, 0);
        }

        // Decimal

        [Benchmark]
        public void ReadDecimal8Zero()
        {
            decimal8Converter.Read(decimal8ZeroBuffer, 0);
        }

        [Benchmark]
        public void ReadDecimal8Max()
        {
            decimal8Converter.Read(decimal8MaxBuffer, 0);
        }

        [Benchmark]
        public void ReadDecimal18Zero()
        {
            decimal18Converter.Read(decimal18ZeroBuffer, 0);
        }

        [Benchmark]
        public void ReadDecimal18Max()
        {
            decimal18Converter.Read(decimal18MaxBuffer, 0);
        }

        [Benchmark]
        public void ReadDecimal28Zero()
        {
            decimal28Converter.Read(decimal28ZeroBuffer, 0);
        }

        [Benchmark]
        public void ReadDecimal28Max()
        {
            decimal28Converter.Read(decimal28MaxBuffer, 0);
        }

        // DateTime

        [Benchmark]
        public void ReadDateTime8()
        {
            dateTime8Converter.Read(dateTime8Buffer, 0);
        }

        [Benchmark]
        public void ReadDateTime14()
        {
            dateTime14Converter.Read(dateTime14Buffer, 0);
        }

        [Benchmark]
        public void ReadDateTime17()
        {
            dateTime17Converter.Read(dateTime17Buffer, 0);
        }

        //--------------------------------------------------------------------------------
        // Write
        //--------------------------------------------------------------------------------

        // Binary

        [Benchmark]

        public void WriteIntBinary()
        {
            intBinaryConverter.Write(intBinaryBuffer, 0, 0);
        }

        // Boolean

        [Benchmark]

        public void WriteBoolean()
        {
            booleanConverter.Write(booleanBuffer, 0, false);
        }

        // Bytes

        [Benchmark]
        public void WriteBytes10()
        {
            bytes10Converter.Write(bytes10Buffer, 0, Bytes10);
        }

        [Benchmark]
        public void WriteBytes20()
        {
            bytes20Converter.Write(bytes20Buffer, 0, Bytes20);
        }

        // Text

        [Benchmark]
        public void WriteSjisText20Single20()
        {
            text20Converter.Write(text20Single20Buffer, 0, Text20Single10);
        }

        [Benchmark]
        public void WriteSjisText20Wide5()
        {
            text20Converter.Write(text20Wide10Buffer, 0, Text20Wide5);
        }

        [Benchmark]
        public void WriteSjisText20Empty()
        {
            text20Converter.Write(text20EmptyBuffer, 0, Text20Empty);
        }

        // Number

        [Benchmark]
        public void WriteNumberTextShort4Zero()
        {
            numberTextShort4Converter.Write(numberTextShort4ZeroBuffer, 0, ZeroShort);
        }

        [Benchmark]
        public void WriteNumberTextShort4Max()
        {
            numberTextShort4Converter.Write(numberTextShort4MaxBuffer, 0, Length4Integer);
        }

        [Benchmark]
        public void WriteNumberTextInt8Zero()
        {
            numberTextInt8Converter.Write(numberTextInt8ZeroBuffer, 0, ZeroInteger);
        }

        [Benchmark]
        public void WriteNumberTextInt8Max()
        {
            numberTextInt8Converter.Write(numberTextInt8MaxBuffer, 0, Length8Integer);
        }

        [Benchmark]
        public void WriteNumberTextLong18Zero()
        {
            numberTextLong18Converter.Write(numberTextLong18ZeroBuffer, 0, ZeroLong);
        }

        [Benchmark]
        public void WriteNumberTextLong18Max()
        {
            numberTextLong18Converter.Write(numberTextLong18MaxBuffer, 0, Length18Integer);
        }

        [Benchmark]
        public void WriteNumberTextDecimal8Zero()
        {
            numberTextDecimal8Converter.Write(numberTextDecimal8ZeroBuffer, 0, ZeroDecimal);
        }

        [Benchmark]
        public void WriteNumberTextDecimal8Max()
        {
            numberTextDecimal8Converter.Write(numberTextDecimal8MaxBuffer, 0, Length8Decimal);
        }

        [Benchmark]
        public void WriteNumberTextDecimal18Zero()
        {
            numberTextDecimal18Converter.Write(numberTextDecimal18ZeroBuffer, 0, ZeroDecimal);
        }

        [Benchmark]
        public void WriteNumberTextDecimal18Max()
        {
            numberTextDecimal18Converter.Write(numberTextDecimal18MaxBuffer, 0, Length18Decimal);
        }

        [Benchmark]
        public void WriteNumberTextDecimal28Zero()
        {
            numberTextDecimal28Converter.Write(numberTextDecimal28ZeroBuffer, 0, ZeroDecimal);
        }

        [Benchmark]
        public void WriteNumberTextDecimal28Max()
        {
            numberTextDecimal28Converter.Write(numberTextDecimal28MaxBuffer, 0, Length28Decimal);
        }

        // DateTime

        [Benchmark]
        public void WriteDateTimeText8()
        {
            dateTimeText8Converter.Write(dateTimeText8Buffer, 0, DateTime14);
        }

        [Benchmark]
        public void WriteDateTimeText14()
        {
            dateTimeText14Converter.Write(dateTimeText14Buffer, 0, DateTime14);
        }

        [Benchmark]
        public void WriteDateTimeText17()
        {
            dateTimeText17Converter.Write(dateTimeText17Buffer, 0, DateTime14);
        }

        //--------------------------------------------------------------------------------
        // Write.Options
        //--------------------------------------------------------------------------------

        // ASCII

        [Benchmark]
        public void WriteText13Code()
        {
            text13Converter.Write(ascii13Buffer, 0, Ascii13);
        }

        [Benchmark]
        public void WriteAscii13Code()
        {
            ascii13Converter.Write(ascii13Buffer, 0, Ascii13);
        }

        // Unicode

        [Benchmark]
        public void WriteText30Wide15()
        {
            text30Converter.Write(unicode30Buffer, 0, Unicode30Wide15);
        }

        [Benchmark]
        public void WriteUnicode30Wide15()
        {
            unicode30Converter.Write(unicode30Buffer, 0, Unicode30Wide15);
        }

        // Integer

        [Benchmark]
        public void WriteIntegerShort4Zero()
        {
            short4Converter.Write(short4ZeroBuffer, 0, ZeroShort);
        }

        [Benchmark]
        public void WriteIntegerShort4Max()
        {
            short4Converter.Write(short4MaxBuffer, 0, Length4Integer);
        }

        [Benchmark]
        public void WriteInteger8Zero()
        {
            int8Converter.Write(int8ZeroBuffer, 0, ZeroInteger);
        }

        [Benchmark]
        public void WriteInteger8Max()
        {
            int8Converter.Write(int8MaxBuffer, 0, Length8Integer);
        }

        [Benchmark]
        public void WriteLong18Zero()
        {
            long18Converter.Write(long18ZeroBuffer, 0, ZeroLong);
        }

        [Benchmark]
        public void WriteLong18Max()
        {
            long18Converter.Write(long18MaxBuffer, 0, Length18Integer);
        }

        // Decimal

        [Benchmark]
        public void WriteDecimal8Zero()
        {
            decimal8Converter.Write(decimal8ZeroBuffer, 0, ZeroDecimal);
        }

        [Benchmark]
        public void WriteDecimal8Max()
        {
            decimal8Converter.Write(decimal8MaxBuffer, 0, Length8Decimal);
        }

        [Benchmark]
        public void WriteDecimal18Zero()
        {
            decimal18Converter.Write(decimal18ZeroBuffer, 0, ZeroDecimal);
        }

        [Benchmark]
        public void WriteDecimal18Max()
        {
            decimal18Converter.Write(decimal18MaxBuffer, 0, Length18Decimal);
        }

        [Benchmark]
        public void WriteDecimal28Zero()
        {
            decimal28Converter.Write(decimal28ZeroBuffer, 0, ZeroDecimal);
        }

        [Benchmark]
        public void WriteDecimal28Max()
        {
            decimal28Converter.Write(decimal28MaxBuffer, 0, Length28Decimal);
        }

        // DateTime

        [Benchmark]
        public void WriteDateTime8()
        {
            dateTime8Converter.Write(dateTime8Buffer, 0, DateTime8);
        }

        [Benchmark]
        public void WriteDateTime14()
        {
            dateTime14Converter.Write(dateTime14Buffer, 0, DateTime14);
        }

        [Benchmark]
        public void WriteDateTime17()
        {
            dateTime17Converter.Write(dateTime17Buffer, 0, DateTime17);
        }
    }
}
