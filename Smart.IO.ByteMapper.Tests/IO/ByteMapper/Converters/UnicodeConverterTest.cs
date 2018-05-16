namespace Smart.IO.ByteMapper.Converters
{
    using System.Text;

    using Smart.IO.ByteMapper.Mock;

    using Xunit;

    public class UnicodeConverterTest
    {
        private const int Offset = 1;

        private const int Length = 10;

        private const string Value = "1a*";

        private const string OverflowValue = "123456";

        private static readonly byte[] EmptyBytes;

        private static readonly byte[] ValueBytes;

        private static readonly byte[] OverflowBytes;

        private readonly UnicodeConverter converter;

        static UnicodeConverterTest()
        {
            EmptyBytes = TestBytes.Offset(Offset, Encoding.Unicode.GetBytes(string.Empty.PadRight(Length / 2, ' ')));
            ValueBytes = TestBytes.Offset(Offset, Encoding.Unicode.GetBytes(Value.PadRight(Length / 2, ' ')));
            OverflowBytes = TestBytes.Offset(Offset, Encoding.Unicode.GetBytes(OverflowValue.Substring(0, Length / 2)));
        }

        public UnicodeConverterTest()
        {
            converter = new UnicodeConverter(
                Length,
                true,
                Padding.Right,
                ' ');
        }

        //--------------------------------------------------------------------------------
        // string
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadToUnicode()
        {
            // Empty
            Assert.Equal(string.Empty, converter.Read(EmptyBytes, Offset));

            // Value
            Assert.Equal(Value, converter.Read(ValueBytes, Offset));
        }

        [Fact]
        public void WriteUnicodeToBuffer()
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
