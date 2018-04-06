namespace Smart.IO.Mapper.Converters
{
    using System.Text;

    using Smart.IO.Mapper.Mock;

    using Xunit;

    public class TextConverterTest
    {
        private const int Offset = 1;

        private const int Length = 10;

        private const string Value = "1aあ";

        private const string OverflowValue = "12345678901";

        private static readonly byte[] EmptyBytes;

        private static readonly byte[] ValueBytes;

        private static readonly byte[] OverflowBytes;

        private static readonly Encoding Encoding;

        private readonly TextConverter converter;

        static TextConverterTest()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding = Encoding.GetEncoding(932);

            EmptyBytes = TestBytes.Offset(Offset, Encoding.GetBytes(string.Empty.PadRight(Length, ' ')));
            ValueBytes = TestBytes.Offset(Offset, Encoding.GetBytes(Value.PadRight(Length - (Encoding.GetByteCount(Value) - Value.Length), ' ')));
            OverflowBytes = TestBytes.Offset(Offset, Encoding.GetBytes(OverflowValue.Substring(0, Length)));
        }

        public TextConverterTest()
        {
            converter = new TextConverter(
                Length,
                Encoding,
                true,
                Padding.Right,
                0x20);
        }

        //--------------------------------------------------------------------------------
        // string
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadToText()
        {
            // Empty
            Assert.Equal(string.Empty, converter.Read(EmptyBytes, Offset));

            // Value
            Assert.Equal(Value, converter.Read(ValueBytes, Offset));
        }

        [Fact]
        public void WriteTextToBuffer()
        {
            var buffer = new byte[Length + Offset];

            // Value
            converter.Write(buffer, Offset, Value);
            Assert.Equal(ValueBytes, buffer);

            // Null
            converter.Write(buffer, Offset, null);
            Assert.Equal(EmptyBytes, buffer);

            // Overflow
            converter.Write(buffer, Offset, OverflowValue);
            Assert.Equal(OverflowBytes, buffer);
        }
    }
}
