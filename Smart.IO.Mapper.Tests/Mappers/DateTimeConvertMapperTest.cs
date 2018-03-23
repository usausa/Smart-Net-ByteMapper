namespace Smart.IO.Mapper.Mappers
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Text;

    using Smart.IO.Mapper.Mock;
    using Smart.Reflection;

    using Xunit;

    public class DateTimeConvertMapperTest
    {
        private const string Format = "yyyyMMddHHmmss";

        private static readonly DateTime Value = new DateTime(2000, 12, 31, 12, 34, 56);

        private static readonly byte[] NullBytes = Encoding.ASCII.GetBytes(string.Empty.PadLeft(Format.Length, ' '));

        private static readonly byte[] ValueBytes = Encoding.ASCII.GetBytes("20001231123456".PadLeft(Format.Length, ' '));

        private readonly DateTimeConvertMapper dateTimeMapper;

        private readonly DateTimeConvertMapper nullableDateTimeMapper;

        public DateTimeConvertMapperTest()
        {
            var type = typeof(Target);

            dateTimeMapper = CreateMapper(type.GetProperty(nameof(Target.DateTimeProperty)));
            nullableDateTimeMapper = CreateMapper(type.GetProperty(nameof(Target.NullableDateTimeProperty)));
        }

        private static DateTimeConvertMapper CreateMapper(PropertyInfo pi)
        {
            return new DateTimeConvertMapper(
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
            dateTimeMapper.Read(NullBytes, 0, target);

            Assert.Equal(default, target.DateTimeProperty);
        }

        [Fact]
        public void ReadValueToDateTime()
        {
            var target = new Target();
            dateTimeMapper.Read(ValueBytes, 0, target);

            Assert.Equal(Value, target.DateTimeProperty);
        }

        [Fact]
        public void WriteValueDateTimeToBuffer()
        {
            var buffer = new byte[Format.Length];
            var target = new Target { DateTimeProperty = Value };
            dateTimeMapper.Write(buffer, 0, target);

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

            Assert.Equal(Value, target.NullableDateTimeProperty);
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
