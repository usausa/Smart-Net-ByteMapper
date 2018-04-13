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

        private const int NumberText8Max = 999999999;
        private const int NumberText8Zero = 0;

        private static readonly byte[] Bytes10 = new byte[10];

        private static readonly byte[] Bytes20 = new byte[20];

        private static readonly DateTime DateTimeText14 = new DateTime(2000, 12, 31, 23, 59, 59);

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

        private IMapConverter numberText8Converter;
        private byte[] numberText8MaxBuffer;
        private byte[] numberText8ZeroBuffer;

        private IMapConverter dateTimeText14Converter;
        private byte[] dateTimeText14Buffer;

        private static IBuilderContext CreateBuilderContext()
        {
            var config = new MapperFactoryConfig();
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
            text20Single20Buffer = SjisEncoding.GetFixedBytes(Text20Single10, 20);
            text20Wide10Buffer = SjisEncoding.GetFixedBytes(Text20Wide5, 20);
            text20EmptyBuffer = SjisEncoding.GetFixedBytes(Text20Empty, 20);

            // Number
            var numberText8Builder = new NumberTextConverterBuilder { Length = 8 };
            numberText8Converter = numberText8Builder.CreateConverter(context, typeof(int));
            numberText8MaxBuffer = SjisEncoding.GetFixedBytes(NumberText8Max.ToString(CultureInfo.InvariantCulture), 8, FixedAlignment.Right);
            numberText8ZeroBuffer = SjisEncoding.GetFixedBytes(NumberText8Zero.ToString(CultureInfo.InvariantCulture), 8, FixedAlignment.Right);

            // DateTime
            var dateTimeText14Builder = new DateTimeTextConverterBuilder { Length = 14 };
            dateTimeText14Converter = dateTimeText14Builder.CreateConverter(context, typeof(DateTime));
            dateTimeText14Buffer = SjisEncoding.GetFixedBytes(DateTimeText14.ToString("yyyyMMddHHmmss"), 14);
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
        public void ReadNumberText8Max()
        {
            numberText8Converter.Read(numberText8MaxBuffer, 0);
        }

        [Benchmark]
        public void ReadNumberText8Zero()
        {
            numberText8Converter.Read(numberText8ZeroBuffer, 0);
        }

        // DateTime

        [Benchmark]
        public void ReadDateTimeText14()
        {
            dateTimeText14Converter.Read(dateTimeText14Buffer, 0);
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
        public void WriteNumberText8Max()
        {
            numberText8Converter.Write(numberText8MaxBuffer, 0, NumberText8Max);
        }

        [Benchmark]
        public void WriteNumberText8Zero()
        {
            numberText8Converter.Write(numberText8ZeroBuffer, 0, NumberText8Zero);
        }

        // DateTime

        [Benchmark]
        public void WriteDateTimeText14()
        {
            dateTimeText14Converter.Write(dateTimeText14Buffer, 0, DateTimeText14);
        }
    }
}
