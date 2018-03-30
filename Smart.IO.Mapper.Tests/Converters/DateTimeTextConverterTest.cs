namespace Smart.IO.Mapper.Converters
{
    using System;
    using System.Globalization;
    using System.Text;

    using Smart.IO.Mapper.Mock;

    using Xunit;

    public class DateTimeTextConverterTest
    {
        private const int Offset = 1;

        private const int Length = 14;

        private const string Format = "yyyyMMddHHmmss";

        private const string Format2 = "yyyyMMdd";

        private static readonly DateTime Value = new DateTime(2000, 12, 31, 12, 34, 56);

        private static readonly byte[] NullBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes(string.Empty.PadRight(Length, ' ')));

        private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes("20001231123456".PadRight(Length, ' ')));

        private static readonly byte[] Value2Bytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes("20001231".PadRight(Length, ' ')));

        private static readonly byte[] InvalidBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes("xxxxxxxxxxxxxx".PadRight(Length, ' ')));

        private readonly DateTimeTextConverter decimalConverter;

        private readonly DateTimeTextConverter nullableDateTimeConverter;

        private readonly DateTimeTextConverter decimalConverter2;

        public DateTimeTextConverterTest()
        {
            decimalConverter = CreateConverter(typeof(DateTime), Format);
            nullableDateTimeConverter = CreateConverter(typeof(DateTime?), Format);
            decimalConverter2 = CreateConverter(typeof(DateTime), Format2);
        }

        private static DateTimeTextConverter CreateConverter(Type type, string format)
        {
            return new DateTimeTextConverter(
                14,
                Encoding.ASCII,
                0x20,
                format,
                DateTimeStyles.None,
                DateTimeFormatInfo.InvariantInfo,
                type);
        }

        //--------------------------------------------------------------------------------
        // DateTime
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToDateTimeIsDefault()
        {
            Assert.Equal(default(DateTime), decimalConverter.Read(NullBytes, Offset));
        }

        [Fact]
        public void ReadInvalidToDateTimeIsDefault()
        {
            Assert.Equal(default(DateTime), decimalConverter.Read(InvalidBytes, Offset));
        }

        [Fact]
        public void ReadValueToDateTime()
        {
            Assert.Equal(Value, decimalConverter.Read(ValueBytes, Offset));
        }

        [Fact]
        public void WriteValueDateTimeToBuffer()
        {
            var buffer = new byte[Length + Offset];
            decimalConverter.Write(buffer, Offset, Value);

            Assert.Equal(ValueBytes, buffer);
        }

        [Fact]
        public void WriteValueDateTimeToBufferWithPadding()
        {
            var buffer = new byte[Length + Offset];
            decimalConverter2.Write(buffer, Offset, Value);

            Assert.Equal(Value2Bytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // DateTime?
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToNullableDateTimeIsDefault()
        {
            Assert.Null(nullableDateTimeConverter.Read(NullBytes, Offset));
        }

        [Fact]
        public void ReadInvalidToNullableDateTimeIsDefault()
        {
            Assert.Null(nullableDateTimeConverter.Read(InvalidBytes, Offset));
        }

        [Fact]
        public void ReadValueToNullableDateTime()
        {
            Assert.Equal(Value, nullableDateTimeConverter.Read(ValueBytes, Offset));
        }

        [Fact]
        public void WriteNullDateTimeToBuffer()
        {
            var buffer = new byte[Length + Offset];
            nullableDateTimeConverter.Write(buffer, Offset, null);

            Assert.Equal(NullBytes, buffer);
        }
    }
}
