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

        private static readonly byte[] EmptyBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes(string.Empty.PadLeft(Length, ' ')));

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
        public void ReadToDecimal()
        {
            // Default
            Assert.Equal(0m, decimalConverter.Read(EmptyBytes, Offset));

            // Value
            Assert.Equal(Value, decimalConverter.Read(ValueBytes, Offset));
        }

        [Fact]
        public void WriteDecimalToBuffer()
        {
            var buffer = new byte[Length + Offset];

            // Value
            decimalConverter.Write(buffer, Offset, Value);
            Assert.Equal(ValueBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // decimal?
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadToNullableDecimal()
        {
            // Null
            Assert.Null(nullableDecimalConverter.Read(EmptyBytes, Offset));

            // Value
            Assert.Equal(Value, nullableDecimalConverter.Read(ValueBytes, Offset));
        }

        [Fact]
        public void WriteNullableDecimalToBuffer()
        {
            var buffer = new byte[Length + Offset];

            // Null
            nullableDecimalConverter.Write(buffer, Offset, null);
            Assert.Equal(EmptyBytes, buffer);
        }
    }
}
