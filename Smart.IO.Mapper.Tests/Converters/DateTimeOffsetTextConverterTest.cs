namespace Smart.IO.Mapper.Converters
{
    using System;
    using System.Globalization;
    using System.Text;

    using Xunit;

    public class DateTimeOffsetTextConverterTest
    {
        private const string Format = "yyyyMMddHHmmss";

        private static readonly DateTimeOffset Value = new DateTimeOffset(new DateTime(2000, 12, 31, 12, 34, 56));

        private static readonly byte[] NullBytes = Encoding.ASCII.GetBytes(string.Empty.PadLeft(Format.Length, ' '));

        private static readonly byte[] ValueBytes = Encoding.ASCII.GetBytes("20001231123456".PadLeft(Format.Length, ' '));

        private readonly DateTimeOffsetTextConverter decimalConverter;

        private readonly DateTimeOffsetTextConverter nullableDateTimeOffsetConverter;

        public DateTimeOffsetTextConverterTest()
        {
            decimalConverter = CreateConverter(typeof(DateTimeOffset));
            nullableDateTimeOffsetConverter = CreateConverter(typeof(DateTimeOffset?));
        }

        private static DateTimeOffsetTextConverter CreateConverter(Type type)
        {
            return new DateTimeOffsetTextConverter(
                14,
                Encoding.ASCII,
                0x20,
                Format,
                DateTimeStyles.None,
                DateTimeFormatInfo.InvariantInfo,
                type);
        }

        //--------------------------------------------------------------------------------
        // DateTimeOffset
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToDateTimeOffsetIsDefault()
        {
            Assert.Equal(default(DateTimeOffset), decimalConverter.Read(NullBytes, 0));
        }

        [Fact]
        public void ReadValueToDateTimeOffset()
        {
            Assert.Equal(Value, decimalConverter.Read(ValueBytes, 0));
        }

        [Fact]
        public void WriteValueDateTimeOffsetToBuffer()
        {
            var buffer = new byte[Format.Length];
            decimalConverter.Write(buffer, 0, Value);

            Assert.Equal(ValueBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // DateTimeOffset?
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToNullableDateTimeOffsetIsDefault()
        {
            Assert.Null(nullableDateTimeOffsetConverter.Read(NullBytes, 0));
        }

        [Fact]
        public void ReadValueToNullableDateTimeOffset()
        {
            Assert.Equal(Value, nullableDateTimeOffsetConverter.Read(ValueBytes, 0));
        }

        [Fact]
        public void WriteNullDateTimeOffsetToBuffer()
        {
            var buffer = new byte[Format.Length];
            nullableDateTimeOffsetConverter.Write(buffer, 0, null);

            Assert.Equal(NullBytes, buffer);
        }
    }
}
