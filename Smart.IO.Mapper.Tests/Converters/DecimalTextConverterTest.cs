namespace Smart.IO.Mapper.Converters
{
    using System;
    using System.Globalization;
    using System.Text;

    using Xunit;

    public class DecimalTextConverterTest
    {
        private const int Length = 21;

        private const decimal Value = 1234567890.98m;

        private static readonly byte[] NullBytes = Encoding.ASCII.GetBytes(string.Empty.PadLeft(Length, ' '));

        private static readonly byte[] ValueBytes = Encoding.ASCII.GetBytes("1234567890.98".PadLeft(Length, ' '));

        private readonly DecimalTextConverter decimalConverter;

        private readonly DecimalTextConverter nullableDecimalConverter;

        public DecimalTextConverterTest()
        {
            decimalConverter = CreateConverter(typeof(decimal));
            nullableDecimalConverter = CreateConverter(typeof(decimal?));
        }

        private static DecimalTextConverter CreateConverter(Type type)
        {
            return new DecimalTextConverter(
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
            Assert.Equal(0m, decimalConverter.Read(NullBytes, 0));
        }

        [Fact]
        public void ReadValueToDecimal()
        {
            Assert.Equal(Value, decimalConverter.Read(ValueBytes, 0));
        }

        [Fact]
        public void WriteValueDecimalToBuffer()
        {
            var buffer = new byte[Length];
            decimalConverter.Write(buffer, 0, Value);

            Assert.Equal(ValueBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // decimal?
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToNullableDecimalIsDefault()
        {
            Assert.Null(nullableDecimalConverter.Read(NullBytes, 0));
        }

        [Fact]
        public void ReadValueToNullableDecimal()
        {
            Assert.Equal(Value, nullableDecimalConverter.Read(ValueBytes, 0));
        }

        [Fact]
        public void WriteNullDecimalToBuffer()
        {
            var buffer = new byte[Length];
            nullableDecimalConverter.Write(buffer, 0, null);

            Assert.Equal(NullBytes, buffer);
        }
    }
}
