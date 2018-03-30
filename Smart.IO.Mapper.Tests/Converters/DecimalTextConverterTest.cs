namespace Smart.IO.Mapper.Converters
{
    using System;
    using System.Globalization;
    using System.Text;

    using Smart.IO.Mapper.Mock;

    using Xunit;

    public class DecimalTextConverterTest
    {
        private const int Offset = 1;

        private const int Length = 21;

        private const decimal Value = 1234567890.98m;

        private static readonly byte[] NullBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes(string.Empty.PadLeft(Length, ' ')));

        private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes("1234567890.98".PadLeft(Length, ' ')));

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
            Assert.Equal(0m, decimalConverter.Read(NullBytes, Offset));
        }

        [Fact]
        public void ReadValueToDecimal()
        {
            Assert.Equal(Value, decimalConverter.Read(ValueBytes, Offset));
        }

        [Fact]
        public void WriteValueDecimalToBuffer()
        {
            var buffer = new byte[Length + Offset];
            decimalConverter.Write(buffer, Offset, Value);

            Assert.Equal(ValueBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // decimal?
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToNullableDecimalIsDefault()
        {
            Assert.Null(nullableDecimalConverter.Read(NullBytes, Offset));
        }

        [Fact]
        public void ReadValueToNullableDecimal()
        {
            Assert.Equal(Value, nullableDecimalConverter.Read(ValueBytes, Offset));
        }

        [Fact]
        public void WriteNullDecimalToBuffer()
        {
            var buffer = new byte[Length + Offset];
            nullableDecimalConverter.Write(buffer, Offset, null);

            Assert.Equal(NullBytes, buffer);
        }
    }
}
