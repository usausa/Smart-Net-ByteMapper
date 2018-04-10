namespace Smart.IO.ByteMapper.Converters
{
    using Smart.IO.ByteMapper.Mock;

    using Xunit;

    public class BigEndianLongBinaryConverterTest
    {
        private const int Offset = 1;

        private const long Value = 1L;

        private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 });

        private readonly BigEndianLongBinaryConverter converter = new BigEndianLongBinaryConverter();

        [Fact]
        public void ReadToBigEndianLongBinary()
        {
            Assert.Equal(Value, (long)converter.Read(ValueBytes, Offset));
        }

        [Fact]
        public void WriteBigEndianLongBinaryToBuffer()
        {
            var buffer = new byte[8 + Offset];
            converter.Write(buffer, Offset, Value);

            Assert.Equal(ValueBytes, buffer);
        }
    }
}
