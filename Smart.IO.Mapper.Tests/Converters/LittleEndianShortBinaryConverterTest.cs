namespace Smart.IO.Mapper.Converters
{
    using Smart.IO.Mapper.Mock;

    using Xunit;

    public class LittleEndianShortBinaryConverterTest
    {
        private const int Offset = 1;

        private const short Value = 1;

        private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, new byte[] { 0x01, 0x00 });

        private readonly LittleEndianShortBinaryConverter converter = new LittleEndianShortBinaryConverter();

        [Fact]
        public void ReadValueToLittleEndianShortBinary()
        {
            Assert.Equal(Value, (short)converter.Read(ValueBytes, Offset));
        }

        [Fact]
        public void WriteLittleEndianShortBinaryToBuffer()
        {
            var buffer = new byte[2 + Offset];
            converter.Write(buffer, Offset, Value);

            Assert.Equal(ValueBytes, buffer);
        }
    }
}
