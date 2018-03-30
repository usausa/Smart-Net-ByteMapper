namespace Smart.IO.Mapper.Converters
{
    using System;
    using System.Globalization;
    using System.Text;

    using Smart.IO.Mapper.Mock;

    using Xunit;

    public class DateTimeOffsetTextConverterTest
    {
        private const int Offset = 1;

        private const int Length = 14;

        private const string Format = "yyyyMMddHHmmss";

        private const string Format2 = "yyyyMMdd";

        private static readonly DateTimeOffset Value = new DateTimeOffset(new DateTime(2000, 12, 31, 12, 34, 56));

        private static readonly byte[] NullBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes(string.Empty.PadRight(Length, ' ')));

        private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes("20001231123456".PadRight(Length, ' ')));

        private static readonly byte[] Value2Bytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes("20001231".PadRight(Length, ' ')));

        private static readonly byte[] InvalidBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes("xxxxxxxxxxxxxx".PadRight(Length, ' ')));

        private readonly DateTimeOffsetTextConverter decimalConverter;

        private readonly DateTimeOffsetTextConverter nullableDateTimeOffsetConverter;

        private readonly DateTimeOffsetTextConverter decimalConverter2;

        public DateTimeOffsetTextConverterTest()
        {
            decimalConverter = CreateConverter(typeof(DateTimeOffset), Format);
            nullableDateTimeOffsetConverter = CreateConverter(typeof(DateTimeOffset?), Format);
            decimalConverter2 = CreateConverter(typeof(DateTimeOffset), Format2);
        }

        private static DateTimeOffsetTextConverter CreateConverter(Type type, string format)
        {
            return new DateTimeOffsetTextConverter(
                14,
                Encoding.ASCII,
                0x20,
                format,
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
            Assert.Equal(default(DateTimeOffset), decimalConverter.Read(NullBytes, Offset));
        }

        [Fact]
        public void ReadInvalidToDateTimeOffsetIsDefault()
        {
            Assert.Equal(default(DateTimeOffset), decimalConverter.Read(InvalidBytes, Offset));
        }

        [Fact]
        public void ReadValueToDateTimeOffset()
        {
            Assert.Equal(Value, decimalConverter.Read(ValueBytes, Offset));
        }

        [Fact]
        public void WriteValueDateTimeOffsetToBuffer()
        {
            var buffer = new byte[Length + Offset];
            decimalConverter.Write(buffer, Offset, Value);

            Assert.Equal(ValueBytes, buffer);
        }

        [Fact]
        public void WriteValueDateTimeOffsetToBufferWithPadding()
        {
            var buffer = new byte[Length + Offset];
            decimalConverter2.Write(buffer, Offset, Value);

            Assert.Equal(Value2Bytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // DateTimeOffset?
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToNullableDateTimeOffsetIsDefault()
        {
            Assert.Null(nullableDateTimeOffsetConverter.Read(NullBytes, Offset));
        }

        [Fact]
        public void ReadInvalidToNullableDateTimeOffsetIsDefault()
        {
            Assert.Null(nullableDateTimeOffsetConverter.Read(InvalidBytes, Offset));
        }

        [Fact]
        public void ReadValueToNullableDateTimeOffset()
        {
            Assert.Equal(Value, nullableDateTimeOffsetConverter.Read(ValueBytes, Offset));
        }

        [Fact]
        public void WriteNullDateTimeOffsetToBuffer()
        {
            var buffer = new byte[Length + Offset];
            nullableDateTimeOffsetConverter.Write(buffer, Offset, null);

            Assert.Equal(NullBytes, buffer);
        }
    }
}
