namespace Smart.IO.Mapper.Converters
{
    using Smart.IO.Mapper.Mock;

    using Xunit;

    public class ByteConverterTest
    {
        private const int Offset = 1;

        private const byte Value = 0x01;

        private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, new[] { Value });

        private readonly ByteConverter converter = new ByteConverter();

        [Fact]
        public void ReadValueToByte()
        {
            Assert.Equal(Value, (byte)converter.Read(ValueBytes, Offset));
        }

        [Fact]
        public void WriteByteToBuffer()
        {
            var buffer = new byte[1 + Offset];
            converter.Write(buffer, Offset, Value);

            Assert.Equal(ValueBytes, buffer);
        }
    }
}
