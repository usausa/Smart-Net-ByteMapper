namespace Smart.IO.ByteMapper.Converters
{
    using Smart.IO.ByteMapper.Mock;

    using Xunit;

    public class BooleanConverterTest
    {
        private const int Offset = 1;

        private const byte TrueByte = 0x31;

        private const byte FalseByte = 0x30;

        private static readonly byte[] TrueBytes = TestBytes.Offset(Offset, new[] { TrueByte });

        private static readonly byte[] FalseBytes = TestBytes.Offset(Offset, new[] { FalseByte });

        private static readonly byte[] UnknownBytes = TestBytes.Offset(Offset, new byte[] { 0x00 });

        private readonly BooleanConverter converter;

        public BooleanConverterTest()
        {
            converter = new BooleanConverter(TrueByte, FalseByte);
        }

        //--------------------------------------------------------------------------------
        // bool
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadToBoolean()
        {
            // True
            Assert.True((bool)converter.Read(TrueBytes, Offset));

            // False
            Assert.False((bool)converter.Read(FalseBytes, Offset));

            // Unknown
            Assert.False((bool)converter.Read(UnknownBytes, Offset));
        }

        [Fact]
        public void WriteBooleanToBuffer()
        {
            var buffer = new byte[1 + Offset];

            // True
            converter.Write(buffer, Offset, true);
            Assert.Equal(TrueBytes, buffer);

            // False
            converter.Write(buffer, Offset, false);
            Assert.Equal(FalseBytes, buffer);
        }
    }
}
