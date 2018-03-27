namespace Smart.IO.Mapper.Mappers
{
    using System;
    using System.Globalization;
    using System.Text;

    using Xunit;

    public class DecimalTextMapperTest
    {
        private const int Length = 21;

        private const decimal Value = 1234567890.98m;

        private static readonly byte[] NullBytes = Encoding.ASCII.GetBytes(string.Empty.PadLeft(Length, ' '));

        private static readonly byte[] ValueBytes = Encoding.ASCII.GetBytes("1234567890.98".PadLeft(Length, ' '));

        private readonly DecimalTextMapper decimalMapper;

        private readonly DecimalTextMapper nullableDecimalMapper;

        public DecimalTextMapperTest()
        {
            decimalMapper = CreateMapper(typeof(decimal));
            nullableDecimalMapper = CreateMapper(typeof(decimal?));
        }

        private static DecimalTextMapper CreateMapper(Type type)
        {
            return new DecimalTextMapper(
                Length,
                Encoding.ASCII,
                true,
                Padding.Left,
                0x20,
                NumberStyles.Number,
                NumberFormatInfo.InvariantInfo,
                type);
        }

        //--------------------------------------------------------------------------------
        // decimal
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToDecimalIsDefault()
        {
            Assert.Equal(0m, decimalMapper.Read(NullBytes, 0));
        }

        [Fact]
        public void ReadValueToDecimal()
        {
            Assert.Equal(Value, decimalMapper.Read(ValueBytes, 0));
        }

        [Fact]
        public void WriteValueDecimalToBuffer()
        {
            var buffer = new byte[Length];
            decimalMapper.Write(buffer, 0, Value);

            Assert.Equal(ValueBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // decimal?
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToNullableDecimalIsDefault()
        {
            Assert.Null(nullableDecimalMapper.Read(NullBytes, 0));
        }

        [Fact]
        public void ReadValueToNullableDecimal()
        {
            Assert.Equal(Value, nullableDecimalMapper.Read(ValueBytes, 0));
        }

        [Fact]
        public void WriteNullDecimalToBuffer()
        {
            var buffer = new byte[Length];
            nullableDecimalMapper.Write(buffer, 0, null);

            Assert.Equal(NullBytes, buffer);
        }
    }
}
