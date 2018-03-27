namespace Smart.IO.Mapper.Converters
{
    using System;
    using System.Globalization;
    using System.Text;

    using Xunit;

    public class DateTimeTextConverterTest
    {
        private const string Format = "yyyyMMddHHmmss";

        private static readonly DateTime Value = new DateTime(2000, 12, 31, 12, 34, 56);

        private static readonly byte[] NullBytes = Encoding.ASCII.GetBytes(string.Empty.PadLeft(Format.Length, ' '));

        private static readonly byte[] ValueBytes = Encoding.ASCII.GetBytes("20001231123456".PadLeft(Format.Length, ' '));

        private readonly DateTimeTextConverter decimalConverter;

        private readonly DateTimeTextConverter nullableDateTimeConverter;

        public DateTimeTextConverterTest()
        {
            decimalConverter = CreateConverter(typeof(DateTime));
            nullableDateTimeConverter = CreateConverter(typeof(DateTime?));
        }

        private static DateTimeTextConverter CreateConverter(Type type)
        {
            return new DateTimeTextConverter(
                14,
                Encoding.ASCII,
                0x20,
                Format,
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
            Assert.Equal(default(DateTime), decimalConverter.Read(NullBytes, 0));
        }

        [Fact]
        public void ReadValueToDateTime()
        {
            Assert.Equal(Value, decimalConverter.Read(ValueBytes, 0));
        }

        [Fact]
        public void WriteValueDateTimeToBuffer()
        {
            var buffer = new byte[Format.Length];
            decimalConverter.Write(buffer, 0, Value);

            Assert.Equal(ValueBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // DateTime?
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToNullableDateTimeIsDefault()
        {
            Assert.Null(nullableDateTimeConverter.Read(NullBytes, 0));
        }

        [Fact]
        public void ReadValueToNullableDateTime()
        {
            Assert.Equal(Value, nullableDateTimeConverter.Read(ValueBytes, 0));
        }

        [Fact]
        public void WriteNullDateTimeToBuffer()
        {
            var buffer = new byte[Format.Length];
            nullableDateTimeConverter.Write(buffer, 0, null);

            Assert.Equal(NullBytes, buffer);
        }
    }
}
