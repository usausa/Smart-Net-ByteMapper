namespace Smart.IO.ByteMapper.Converters
{
    using System;
    using System.Text;

    using Smart.IO.ByteMapper.Mock;

    using Xunit;

    public class DateTimeOffsetConverterTest
    {
        private const int Offset = 1;

        private const int Length = 17;

        private const string Format = "yyyyMMddHHmmssfff";

        private static readonly DateTimeOffset Value = new DateTimeOffset(new DateTime(2000, 12, 31, 12, 34, 56, 789, DateTimeKind.Utc));

        private static readonly byte[] EmptyBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes(string.Empty.PadRight(Length, ' ')));

        private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes("20001231123456789"));

        private static readonly byte[] InvalidBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes("xxxxxxxxxxxxxxxxx"));

        private readonly DateTimeOffsetConverter decimalConverter;

        private readonly DateTimeOffsetConverter nullableDateTimeOffsetConverter;

        public DateTimeOffsetConverterTest()
        {
            decimalConverter = CreateConverter(typeof(DateTimeOffset), Format);
            nullableDateTimeOffsetConverter = CreateConverter(typeof(DateTimeOffset?), Format);
        }

        private static DateTimeOffsetConverter CreateConverter(Type type, string format)
        {
            return new DateTimeOffsetConverter(
                format,
                0x20,
                type);
        }

        //--------------------------------------------------------------------------------
        // DateTimeOffset
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadToDateTimeOffset()
        {
            // Default
            Assert.Equal(default(DateTimeOffset), decimalConverter.Read(EmptyBytes, Offset));

            // Invalid
            Assert.Equal(default(DateTimeOffset), decimalConverter.Read(InvalidBytes, Offset));

            // Value
            Assert.Equal(Value, decimalConverter.Read(ValueBytes, Offset));
        }

        [Fact]
        public void WriteDateTimeOffsetToBuffer()
        {
            var buffer = new byte[Length + Offset];

            // Value
            decimalConverter.Write(buffer, Offset, Value);
            Assert.Equal(ValueBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // DateTimeOffset?
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadToNullableDateTimeOffset()
        {
            // Null
            Assert.Null(nullableDateTimeOffsetConverter.Read(EmptyBytes, Offset));

            // Invalid
            Assert.Null(nullableDateTimeOffsetConverter.Read(InvalidBytes, Offset));

            // Value
            Assert.Equal(Value, nullableDateTimeOffsetConverter.Read(ValueBytes, Offset));
        }

        [Fact]
        public void WriteNullDateTimeOffsetToBuffer()
        {
            var buffer = new byte[Length + Offset];

            // Null
            nullableDateTimeOffsetConverter.Write(buffer, Offset, null);
            Assert.Equal(EmptyBytes, buffer);
        }
    }
}
