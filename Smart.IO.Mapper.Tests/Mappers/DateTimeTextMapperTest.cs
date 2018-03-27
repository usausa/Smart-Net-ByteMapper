namespace Smart.IO.Mapper.Mappers
{
    using System;
    using System.Globalization;
    using System.Text;

    using Xunit;

    public class DateTimeTextMapperTest
    {
        private const string Format = "yyyyMMddHHmmss";

        private static readonly DateTime Value = new DateTime(2000, 12, 31, 12, 34, 56);

        private static readonly byte[] NullBytes = Encoding.ASCII.GetBytes(string.Empty.PadLeft(Format.Length, ' '));

        private static readonly byte[] ValueBytes = Encoding.ASCII.GetBytes("20001231123456".PadLeft(Format.Length, ' '));

        private readonly DateTimeTextMapper decimalMapper;

        private readonly DateTimeTextMapper nullableDateTimeMapper;

        public DateTimeTextMapperTest()
        {
            decimalMapper = CreateMapper(typeof(DateTime));
            nullableDateTimeMapper = CreateMapper(typeof(DateTime?));
        }

        private static DateTimeTextMapper CreateMapper(Type type)
        {
            return new DateTimeTextMapper(
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
            Assert.Equal(default(DateTime), decimalMapper.Read(NullBytes, 0));
        }

        [Fact]
        public void ReadValueToDateTime()
        {
            Assert.Equal(Value, decimalMapper.Read(ValueBytes, 0));
        }

        [Fact]
        public void WriteValueDateTimeToBuffer()
        {
            var buffer = new byte[Format.Length];
            decimalMapper.Write(buffer, 0, Value);

            Assert.Equal(ValueBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // DateTime?
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToNullableDateTimeIsDefault()
        {
            Assert.Null(nullableDateTimeMapper.Read(NullBytes, 0));
        }

        [Fact]
        public void ReadValueToNullableDateTime()
        {
            Assert.Equal(Value, nullableDateTimeMapper.Read(ValueBytes, 0));
        }

        [Fact]
        public void WriteNullDateTimeToBuffer()
        {
            var buffer = new byte[Format.Length];
            nullableDateTimeMapper.Write(buffer, 0, null);

            Assert.Equal(NullBytes, buffer);
        }
    }
}
