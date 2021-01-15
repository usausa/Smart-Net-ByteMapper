namespace Smart.IO.ByteMapper.Converters
{
    using Smart.IO.ByteMapper.Mock;

    using Xunit;

    public class BigEndianDecimalBinaryConverterTest
    {
        private const int Offset = 1;

        private const decimal Value = 1;

        private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, new byte[]
        {
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01
        });

        private readonly IMapConverter converter = BigEndianDecimalBinaryConverter.Default;

        [Fact]
        public void ReadToBigEndianDecimalBinary()
        {
            Assert.Equal(Value, (decimal)converter.Read(ValueBytes, Offset));
        }

        [Fact]
        public void WriteBigEndianDecimalBinaryToBuffer()
        {
            var buffer = new byte[16 + Offset];
            converter.Write(buffer, Offset, Value);

            Assert.Equal(ValueBytes, buffer);
        }
    }

    public class LittleEndianDecimalBinaryConverterTest
    {
        private const int Offset = 1;

        private const decimal Value = 1;

        private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, new byte[]
        {
            0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        });

        private readonly IMapConverter converter = LittleEndianDecimalBinaryConverter.Default;

        [Fact]
        public void ReadToLittleEndianDecimalBinary()
        {
            Assert.Equal(Value, (decimal)converter.Read(ValueBytes, Offset));
        }

        [Fact]
        public void WriteLittleEndianDecimalBinaryToBuffer()
        {
            var buffer = new byte[16 + Offset];
            converter.Write(buffer, Offset, Value);

            Assert.Equal(ValueBytes, buffer);
        }
    }
}
