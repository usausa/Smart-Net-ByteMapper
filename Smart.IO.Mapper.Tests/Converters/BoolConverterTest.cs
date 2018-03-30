namespace Smart.IO.Mapper.Converters
{
    using Smart.IO.Mapper.Mock;

    using Xunit;

    public class BoolConverterTest
    {
        private const int Offset = 1;

        private static readonly byte TrueByte = 0x31;

        private static readonly byte FalseByte = 0x30;

        private static readonly byte[] TrueBytes = TestBytes.Offset(Offset, new[] { TrueByte });

        private static readonly byte[] FalseBytes = TestBytes.Offset(Offset, new[] { FalseByte });

        private static readonly byte[] UnknownBytes = TestBytes.Offset(Offset, new byte[] { 0x00 });

        private readonly BoolConverter converter;

        public BoolConverterTest()
        {
            converter = new BoolConverter(TrueByte, FalseByte);
        }

        //--------------------------------------------------------------------------------
        // bool
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadTrueValueToBool()
        {
            Assert.True((bool)converter.Read(TrueBytes, Offset));
        }

        [Fact]
        public void ReadFalseValueToBool()
        {
            Assert.False((bool)converter.Read(FalseBytes, Offset));
        }

        [Fact]
        public void ReadUnknownValueToBoolIsFalse()
        {
            Assert.False((bool)converter.Read(UnknownBytes, Offset));
        }

        [Fact]
        public void WriteTrueBoolToBuffer()
        {
            var buffer = new byte[1 + Offset];
            converter.Write(buffer, Offset, true);

            Assert.Equal(TrueBytes, buffer);
        }

        [Fact]
        public void WriteFalseBoolToBuffer()
        {
            var buffer = new byte[1 + Offset];
            converter.Write(buffer, Offset, false);

            Assert.Equal(FalseBytes, buffer);
        }
    }
}
