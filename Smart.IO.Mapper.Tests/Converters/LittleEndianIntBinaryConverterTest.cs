namespace Smart.IO.Mapper.Converters
{
    using Smart.IO.Mapper.Mock;

    using Xunit;

    public class LittleEndianIntBinaryConverterTest
    {
        private const int Offset = 1;

        private const int Value = 1;

        private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, new byte[] { 0x01, 0x00, 0x00, 0x00 });

        private readonly LittleEndianIntBinaryConverter converter = new LittleEndianIntBinaryConverter();

        [Fact]
        public void ReadValueToLittleEndianIntBinary()
        {
            Assert.Equal(Value, (int)converter.Read(ValueBytes, Offset));
        }

        [Fact]
        public void WriteLittleEndianIntBinaryToBuffer()
        {
            var buffer = new byte[4 + Offset];
            converter.Write(buffer, Offset, Value);

            Assert.Equal(ValueBytes, buffer);
        }
    }
}
