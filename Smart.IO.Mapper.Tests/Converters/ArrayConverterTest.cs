namespace Smart.IO.Mapper.Converters
{
    using Smart.IO.Mapper.Mock;

    using Xunit;

    public class ArrayConverterTest
    {
        private const int Offset = 1;

        private static readonly int[] Value = { 1, 1, 1 };

        private static readonly byte[] ValueBytes = TestBytes.Offset(
            Offset,
            new byte[] { 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01 });

        private static readonly byte[] NullBytes = TestBytes.Offset(
            Offset,
            new byte[12]);

        private readonly IByteConverter converter = new ArrayConverter(
            x => new int[x],
            3,
            0x00,
            4,
            new BigEndianIntBinaryConverter());

        [Fact]
        public void ReadToIntArray()
        {
            Assert.Equal(Value, (int[])converter.Read(ValueBytes, Offset));
        }

        [Fact]
        public void WriteIntArrayToBuffer()
        {
            var buffer = new byte[12 + Offset];

            // Value
            converter.Write(buffer, Offset, Value);
            Assert.Equal(ValueBytes, buffer);

            // Null
            converter.Write(buffer, Offset, null);
            Assert.Equal(NullBytes, buffer);
        }
    }
}
