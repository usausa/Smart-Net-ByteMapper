namespace Smart.IO.Mapper.Converters
{
    using Smart.IO.Mapper.Mock;

    using Xunit;

    public class NullableBoolConverterTest
    {
        private const int Offset = 1;

        private static readonly byte TrueByte = 0x31;

        private static readonly byte FalseByte = 0x30;

        private static readonly byte NullByte = 0x00;

        private static readonly byte[] TrueBytes = TestBytes.Offset(Offset, new[] { TrueByte });

        private static readonly byte[] FalseBytes = TestBytes.Offset(Offset, new[] { FalseByte });

        private static readonly byte[] NullBytes = TestBytes.Offset(Offset, new[] { NullByte });

        private readonly NullableBoolConverter converter;

        public NullableBoolConverterTest()
        {
            converter = new NullableBoolConverter(TrueByte, FalseByte, NullByte);
        }

        //--------------------------------------------------------------------------------
        // bool
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadTrueValueToNullableBool()
        {
            Assert.True((bool?)converter.Read(TrueBytes, Offset));
        }

        [Fact]
        public void ReadFalseValueToNullableBool()
        {
            Assert.False((bool?)converter.Read(FalseBytes, Offset));
        }

        [Fact]
        public void ReadNullValueToNullableBoolIsNull()
        {
            Assert.Null(converter.Read(NullBytes, Offset));
        }

        [Fact]
        public void WriteTrueNullableBoolToBuffer()
        {
            var buffer = new byte[1 + Offset];
            converter.Write(buffer, Offset, true);

            Assert.Equal(TrueBytes, buffer);
        }

        [Fact]
        public void WriteFalseNullableBoolToBuffer()
        {
            var buffer = new byte[1 + Offset];
            converter.Write(buffer, Offset, false);

            Assert.Equal(FalseBytes, buffer);
        }

        [Fact]
        public void WritNullNullableBoolToBuffer()
        {
            var buffer = new byte[1 + Offset];
            converter.Write(buffer, Offset, null);

            Assert.Equal(NullBytes, buffer);
        }
    }
}
