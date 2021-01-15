namespace Smart.IO.ByteMapper.Converters
{
    using Smart.IO.ByteMapper.Mock;

    using Xunit;

    public class BigEndianDoubleBinaryConverterTest
    {
        private const int Offset = 1;

        private const double Value = 2;

        private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, new byte[]
        {
            0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        });

        private readonly IMapConverter converter = BigEndianDoubleBinaryConverter.Default;

        [Fact]
        public void ReadToBigEndianDoubleBinary()
        {
            Assert.Equal(Value, (double)converter.Read(ValueBytes, Offset));
        }

        [Fact]
        public void WriteBigEndianDoubleBinaryToBuffer()
        {
            var buffer = new byte[8 + Offset];
            converter.Write(buffer, Offset, Value);

            Assert.Equal(ValueBytes, buffer);
        }
    }

    public class LittleEndianDoubleBinaryConverterTest
    {
        private const int Offset = 1;

        private const double Value = 2;

        private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, new byte[]
        {
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x40
        });

        private readonly IMapConverter converter = LittleEndianDoubleBinaryConverter.Default;

        [Fact]
        public void ReadToLittleEndianDoubleBinary()
        {
            Assert.Equal(Value, (double)converter.Read(ValueBytes, Offset));
        }

        [Fact]
        public void WriteLittleEndianDoubleBinaryToBuffer()
        {
            var buffer = new byte[8 + Offset];
            converter.Write(buffer, Offset, Value);

            Assert.Equal(ValueBytes, buffer);
        }
    }
}
