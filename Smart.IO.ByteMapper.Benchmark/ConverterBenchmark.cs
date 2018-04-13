namespace Smart.IO.ByteMapper.Benchmark
{
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

        private IMapConverter intBinaryConverter;
        private byte[] intBinaryBuffer;

        private IMapConverter text20Converter;
        private byte[] text20Single20Buffer;
        private byte[] text20Wide10Buffer;
        private byte[] text20EmptyBuffer;

        private IMapConverter numberText8Converter;
        private IMapConverter numberText8AsciiConverter;
        private byte[] numberText8MaxBuffer;
        private byte[] numberText8ZeroBuffer;

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

            // Text
            var text20Builder = new TextConverterBuilder { Length = 20 };
            text20Converter = text20Builder.CreateConverter(context, typeof(string));

            text20Single20Buffer = SjisEncoding.GetFixedBytes(Text20Single10, 20);
            text20Wide10Buffer = SjisEncoding.GetFixedBytes(Text20Wide5, 20);
            text20EmptyBuffer = SjisEncoding.GetFixedBytes(Text20Empty, 20);

            // Number
            var numberText8Builder = new NumberTextConverterBuilder { Length = 8 };
            numberText8Converter = numberText8Builder.CreateConverter(context, typeof(int));
            var numberText8AsciiBuilder = new NumberTextConverterBuilder { Length = 8, Encoding = Encoding.ASCII };
            numberText8AsciiConverter = numberText8AsciiBuilder.CreateConverter(context, typeof(int));

            numberText8MaxBuffer = SjisEncoding.GetFixedBytes(NumberText8Max.ToString(CultureInfo.InvariantCulture), 8, FixedAlignment.Right);
            numberText8ZeroBuffer = SjisEncoding.GetFixedBytes(NumberText8Zero.ToString(CultureInfo.InvariantCulture), 8, FixedAlignment.Right);

            // TODO DateTime
            // TODO Bool
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

        // Text

        [Benchmark]
        public void ReadText20Single20()
        {
            text20Converter.Read(text20Single20Buffer, 0);
        }

        [Benchmark]
        public void ReadText20Wide5()
        {
            text20Converter.Read(text20Wide10Buffer, 0);
        }

        [Benchmark]
        public void ReadText20Empty()
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

        [Benchmark]
        public void ReadNumberText8AsciiMax()
        {
            numberText8AsciiConverter.Read(numberText8MaxBuffer, 0);
        }

        [Benchmark]
        public void ReadNumberText8AsciiZero()
        {
            numberText8AsciiConverter.Read(numberText8ZeroBuffer, 0);
        }

        //[Benchmark]
        //public void ReadIntText8()
        //{
        //    numberText8Converter.Read(numberText8Buffer, 0);
        //}

        // TODO DateTime
        // TODO Bool

        //--------------------------------------------------------------------------------
        // Write
        //--------------------------------------------------------------------------------

        // Binary

        [Benchmark]

        public void WriteIntBinary()
        {
            intBinaryConverter.Write(intBinaryBuffer, 0, 0);
        }

        // Text

        [Benchmark]
        public void WriteText20Single20()
        {
            text20Converter.Write(text20Single20Buffer, 0, Text20Single10);
        }

        [Benchmark]
        public void WriteText20Wide5()
        {
            text20Converter.Write(text20Wide10Buffer, 0, Text20Wide5);
        }

        [Benchmark]
        public void WriteText20Empty()
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
            numberText8Converter.Write(numberText8MaxBuffer, 0, NumberText8Zero);
        }

        [Benchmark]
        public void WriteNumberText8AsciiMax()
        {
            numberText8AsciiConverter.Write(numberText8MaxBuffer, 0, NumberText8Max);
        }

        [Benchmark]
        public void WriteNumberText8AsciiZero()
        {
            numberText8AsciiConverter.Write(numberText8MaxBuffer, 0, NumberText8Zero);
        }

        // TODO DateTime
        // TODO Bool
    }
}
