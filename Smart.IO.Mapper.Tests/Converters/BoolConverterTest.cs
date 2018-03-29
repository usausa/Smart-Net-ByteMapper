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

        private readonly BoolConverter boolConverter;

        public BoolConverterTest()
        {
            boolConverter = new BoolConverter(TrueByte, FalseByte);
        }

        [Fact]
        public void ReadTrueValueToBool()
        {
            Assert.True((bool)boolConverter.Read(TrueBytes, Offset));
        }

        [Fact]
        public void ReadFalseValueToBool()
        {
            Assert.False((bool)boolConverter.Read(FalseBytes, Offset));
        }

        [Fact]
        public void ReadUnknownValueToBoolIsFalse()
        {
            Assert.False((bool)boolConverter.Read(UnknownBytes, Offset));
        }

        // TODO nullable
    }
}
