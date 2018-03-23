namespace Smart.IO.Mapper.Mappers
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Text;

    using Smart.IO.Mapper.Mock;
    using Smart.Reflection;

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
            var type = typeof(Target);

            decimalMapper = CreateMapper(type.GetProperty(nameof(Target.DateTimeOffsetProperty)));
            nullableDateTimeOffsetMapper = CreateMapper(type.GetProperty(nameof(Target.NullableDateTimeOffsetProperty)));
        }

        private static DateTimeOffsetTextMapper CreateMapper(PropertyInfo pi)
        {
            return new DateTimeOffsetTextMapper(
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
        // DateTimeOffset
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToDateTimeOffsetIsDefault()
        {
            var target = new Target { DateTimeOffsetProperty = DateTimeOffset.Now };
            decimalMapper.Read(NullBytes, 0, target);

            Assert.Equal(default, target.DateTimeOffsetProperty);
        }

        [Fact]
        public void ReadValueToDateTimeOffset()
        {
            var target = new Target();
            decimalMapper.Read(ValueBytes, 0, target);

            Assert.Equal(Value, target.DateTimeOffsetProperty);
        }

        [Fact]
        public void WriteValueDateTimeOffsetToBuffer()
        {
            var buffer = new byte[Format.Length];
            var target = new Target { DateTimeOffsetProperty = Value };
            decimalMapper.Write(buffer, 0, target);

            Assert.Equal(ValueBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // DateTimeOffset?
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToNullableDateTimeOffsetIsDefault()
        {
            var target = new Target { NullableDateTimeOffsetProperty = DateTimeOffset.Now };
            nullableDateTimeOffsetMapper.Read(NullBytes, 0, target);

            Assert.Null(target.NullableDateTimeOffsetProperty);
        }

        [Fact]
        public void ReadValueToNullableDateTimeOffset()
        {
            var target = new Target();
            nullableDateTimeOffsetMapper.Read(ValueBytes, 0, target);

            Assert.Equal(Value, target.NullableDateTimeOffsetProperty);
        }

        [Fact]
        public void WriteNullDateTimeOffsetToBuffer()
        {
            var buffer = new byte[Format.Length];
            var target = new Target();
            nullableDateTimeOffsetMapper.Write(buffer, 0, target);

            Assert.Equal(NullBytes, buffer);
        }
    }
}
