namespace Smart.IO.Mapper.Mappers
{
    using System;
    using System.Globalization;
    using System.Text;

    using Xunit;

    public class DateTimeOffsetTextMapperTest
    {
        private const string Format = "yyyyMMddHHmmss";

        private static readonly DateTimeOffset Value = new DateTimeOffset(new DateTime(2000, 12, 31, 12, 34, 56));

        private static readonly byte[] NullBytes = Encoding.ASCII.GetBytes(string.Empty.PadLeft(Format.Length, ' '));

        private static readonly byte[] ValueBytes = Encoding.ASCII.GetBytes("20001231123456".PadLeft(Format.Length, ' '));

        private readonly DateTimeOffsetTextMapper decimalMapper;

        private readonly DateTimeOffsetTextMapper nullableDateTimeOffsetMapper;

        public DateTimeOffsetTextMapperTest()
        {
            decimalMapper = CreateMapper(typeof(DateTimeOffset));
            nullableDateTimeOffsetMapper = CreateMapper(typeof(DateTimeOffset?));
        }

        private static DateTimeOffsetTextMapper CreateMapper(Type type)
        {
            return new DateTimeOffsetTextMapper(
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
            Assert.Equal(default(DateTimeOffset), decimalMapper.Read(NullBytes, 0));
        }

        [Fact]
        public void ReadValueToDateTimeOffset()
        {
            Assert.Equal(Value, decimalMapper.Read(ValueBytes, 0));
        }

        [Fact]
        public void WriteValueDateTimeOffsetToBuffer()
        {
            var buffer = new byte[Format.Length];
            decimalMapper.Write(buffer, 0, Value);

            Assert.Equal(ValueBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // DateTimeOffset?
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToNullableDateTimeOffsetIsDefault()
        {
            Assert.Null(nullableDateTimeOffsetMapper.Read(NullBytes, 0));
        }

        [Fact]
        public void ReadValueToNullableDateTimeOffset()
        {
            Assert.Equal(Value, nullableDateTimeOffsetMapper.Read(ValueBytes, 0));
        }

        [Fact]
        public void WriteNullDateTimeOffsetToBuffer()
        {
            var buffer = new byte[Format.Length];
            nullableDateTimeOffsetMapper.Write(buffer, 0, null);

            Assert.Equal(NullBytes, buffer);
        }
    }
}
