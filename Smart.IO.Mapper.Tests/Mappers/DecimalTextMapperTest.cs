namespace Smart.IO.Mapper.Mappers
{
    using System.Globalization;
    using System.Reflection;
    using System.Text;

    using Smart.IO.Mapper.Mock;
    using Smart.Reflection;

    using Xunit;

    public class DecimalTextMapperTest
    {
        private const int Length = 21;

        private static readonly byte[] NullBytes = Encoding.ASCII.GetBytes(string.Empty.PadLeft(Length, ' '));

        private static readonly byte[] ValueBytes = Encoding.ASCII.GetBytes("1234567890.98".PadLeft(Length, ' '));

        private readonly DecimalTextMapper decimalMapper;

        private readonly DecimalTextMapper nullableDecimalMapper;

        public DecimalTextMapperTest()
        {
            var type = typeof(Target);

            decimalMapper = CreateMapper(type.GetProperty(nameof(Target.DecimalProperty)));
            nullableDecimalMapper = CreateMapper(type.GetProperty(nameof(Target.NullableDecimalProperty)));
        }

        private static DecimalTextMapper CreateMapper(PropertyInfo pi)
        {
            return new DecimalTextMapper(
                0,
                Length,
                DelegateFactory.Default.CreateGetter(pi),
                DelegateFactory.Default.CreateSetter(pi),
                Encoding.ASCII,
                true,
                Padding.Left,
                0x20,
                NumberStyles.Number,
                NumberFormatInfo.InvariantInfo,
                pi.PropertyType);
        }

        //--------------------------------------------------------------------------------
        // decimal
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToDecimalIsDefault()
        {
            var target = new Target { DecimalProperty = 1 };
            decimalMapper.Read(NullBytes, 0, target);

            Assert.Equal(0m, target.DecimalProperty);
        }

        [Fact]
        public void ReadValueToDecimal()
        {
            var target = new Target();
            decimalMapper.Read(ValueBytes, 0, target);

            Assert.Equal(1234567890.98m, target.DecimalProperty);
        }

        [Fact]
        public void WriteValueDecimalToBuffer()
        {
            var buffer = new byte[Length];
            var target = new Target { DecimalProperty = 1234567890.98m };
            decimalMapper.Write(buffer, 0, target);

            Assert.Equal(ValueBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // decimal?
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToNullableDecimalIsDefault()
        {
            var target = new Target { NullableDecimalProperty = 1 };
            nullableDecimalMapper.Read(NullBytes, 0, target);

            Assert.Null(target.NullableDecimalProperty);
        }

        [Fact]
        public void ReadValueToNullableDecimal()
        {
            var target = new Target();
            nullableDecimalMapper.Read(ValueBytes, 0, target);

            Assert.Equal(1234567890.98m, target.NullableDecimalProperty);
        }

        [Fact]
        public void WriteNullDecimalToBuffer()
        {
            var buffer = new byte[Length];
            var target = new Target();
            nullableDecimalMapper.Write(buffer, 0, target);

            Assert.Equal(NullBytes, buffer);
        }
    }
}
