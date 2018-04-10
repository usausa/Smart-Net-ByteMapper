namespace Smart.IO.ByteMapper.Converters
{
    using Smart.IO.ByteMapper.Mock;

    using Xunit;

    public class BigEndianShortBinaryConverterTest
    {
        private const int Offset = 1;

        private const short Value = 1;

        private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, new byte[] { 0x00, 0x01 });

        private readonly IMapConverter converter = BigEndianShortBinaryConverter.Default;

        [Fact]
        public void ReadToBigEndianShortBinary()
        {
            Assert.Equal(Value, (short)converter.Read(ValueBytes, Offset));
        }

        [Fact]
        public void WriteBigEndianShortBinaryToBuffer()
        {
            var buffer = new byte[2 + Offset];
            converter.Write(buffer, Offset, Value);

            Assert.Equal(ValueBytes, buffer);
        }
    }
}
