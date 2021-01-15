namespace Smart.IO.ByteMapper.Converters
{
    using System.Text;

    using Smart.IO.ByteMapper.Mock;

    using Xunit;

    public class AsciiConverterTest
    {
        private const int Offset = 1;

        private const int Length = 10;

        private const string Value = "1a*";

        private const string OverflowValue = "12345678901";

        private static readonly byte[] EmptyBytes;

        private static readonly byte[] ValueBytes;

        private static readonly byte[] OverflowBytes;

        private readonly AsciiConverter converter;

        static AsciiConverterTest()
        {
            EmptyBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes(string.Empty.PadRight(Length, ' ')));
            ValueBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes(Value.PadRight(Length, ' ')));
            OverflowBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes(OverflowValue.Substring(0, Length)));
        }

        public AsciiConverterTest()
        {
            converter = new AsciiConverter(
                Length,
                true,
                Padding.Right,
                0x20);
        }

        //--------------------------------------------------------------------------------
        // string
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadToAscii()
        {
            // Empty
            Assert.Equal(string.Empty, converter.Read(EmptyBytes, Offset));

            // Value
            Assert.Equal(Value, converter.Read(ValueBytes, Offset));
        }

        [Fact]
        public void WriteAsciiToBuffer()
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