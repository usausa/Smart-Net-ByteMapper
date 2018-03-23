namespace Smart.IO.Mapper.Mappers
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Text;

    using Smart.IO.Mapper.Mock;
    using Smart.Reflection;

    using Xunit;

    public class DateTimeTextMapperTest
    {
        public const string Format = "yyyyMMddHHmmss";

        private static readonly byte[] NullBytes = Encoding.ASCII.GetBytes(string.Empty.PadLeft(Format.Length, ' '));

        private static readonly byte[] ValueBytes = Encoding.ASCII.GetBytes("20001231123456".PadLeft(Format.Length, ' '));

        private readonly DateTimeTextMapper decimalMapper;

        private readonly DateTimeTextMapper nullableDateTimeMapper;

        public DateTimeTextMapperTest()
        {
            var type = typeof(Target);

            decimalMapper = CreateMapper(type.GetProperty(nameof(Target.DateTimeProperty)));
            nullableDateTimeMapper = CreateMapper(type.GetProperty(nameof(Target.NullableDateTimeProperty)));
        }

        private static DateTimeTextMapper CreateMapper(PropertyInfo pi)
        {
            return new DateTimeTextMapper(
                0,
                DelegateFactory.Default.CreateGetter(pi),
                DelegateFactory.Default.CreateSetter(pi),
                Encoding.ASCII,
                0x20,
                Format,
                DateTimeStyles.None,
                DateTimeFormatInfo.InvariantInfo,
                pi.PropertyType);
        }

        //--------------------------------------------------------------------------------
        // DateTime
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToDateTimeIsDefault()
        {
            var target = new Target { DateTimeProperty = DateTime.Now };
            decimalMapper.Read(NullBytes, 0, target);

            Assert.Equal(default, target.DateTimeProperty);
        }

        [Fact]
        public void ReadValueToDateTime()
        {
            var target = new Target();
            decimalMapper.Read(ValueBytes, 0, target);

            Assert.Equal(new DateTime(2000, 12, 31, 12, 34, 56), target.DateTimeProperty);
        }

        [Fact]
        public void WriteValueDateTimeToBuffer()
        {
            var buffer = new byte[Format.Length];
            var target = new Target { DateTimeProperty = new DateTime(2000, 12, 31, 12, 34, 56) };
            decimalMapper.Write(buffer, 0, target);

            Assert.Equal(ValueBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // DateTime?
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToNullableDateTimeIsDefault()
        {
            var target = new Target { NullableDateTimeProperty = DateTime.Now };
            nullableDateTimeMapper.Read(NullBytes, 0, target);

            Assert.Null(target.NullableDateTimeProperty);
        }

        [Fact]
        public void ReadValueToNullableDateTime()
        {
            var target = new Target();
            nullableDateTimeMapper.Read(ValueBytes, 0, target);

            Assert.Equal(new DateTime(2000, 12, 31, 12, 34, 56), target.NullableDateTimeProperty);
        }

        [Fact]
        public void WriteNullDateTimeToBuffer()
        {
            var buffer = new byte[Format.Length];
            var target = new Target();
            nullableDateTimeMapper.Write(buffer, 0, target);

            Assert.Equal(NullBytes, buffer);
        }
    }
}
