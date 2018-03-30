namespace Smart.IO.Mapper.Converters
{
    using System.Text;

    using Smart.IO.Mapper.Mock;

    using Xunit;

    public class StringConverterTest
    {
        private const int Offset = 1;

        private const int Length = 10;

        private const string Value = "1aあ";

        private const string OverflowValue = "12345678901";

        private static readonly byte[] EmptyBytes;

        private static readonly byte[] ValueBytes;

        private static readonly byte[] OverflowBytes;

        private readonly StringConverter converter;

        static StringConverterTest()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var enc = Encoding.GetEncoding(932);

            EmptyBytes = TestBytes.Offset(Offset, enc.GetBytes(string.Empty.PadRight(Length, ' ')));
            ValueBytes = TestBytes.Offset(Offset, enc.GetBytes(Value.PadRight(Length - (enc.GetByteCount(Value) - Value.Length), ' ')));
            OverflowBytes = TestBytes.Offset(Offset, enc.GetBytes(OverflowValue.Substring(0, Length)));
        }

        public StringConverterTest()
        {
            converter = new StringConverter(
                Length,
                Encoding.GetEncoding(932),
                true,
                Padding.Right,
                0x20);
        }

        //--------------------------------------------------------------------------------
        // string
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToStringIsEmpty()
        {
            Assert.Equal(string.Empty, converter.Read(EmptyBytes, Offset));
        }

        [Fact]
        public void ReadValueToString()
        {
            Assert.Equal(Value, converter.Read(ValueBytes, Offset));
        }

        [Fact]
        public void WriteValueStringToBuffer()
        {
            var buffer = new byte[Length + Offset];
            converter.Write(buffer, Offset, Value);

            Assert.Equal(ValueBytes, buffer);
        }

        [Fact]
        public void WriteNullStringToBuffer()
        {
            var buffer = new byte[Length + Offset];
            converter.Write(buffer, Offset, null);

            Assert.Equal(EmptyBytes, buffer);
        }

        [Fact]
        public void WriteOverflowValueStringToBuffer()
        {
            var buffer = new byte[Length + Offset];
            converter.Write(buffer, Offset, OverflowValue);

            Assert.Equal(OverflowBytes, buffer);
        }
    }
}
