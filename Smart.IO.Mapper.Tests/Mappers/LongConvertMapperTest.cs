namespace Smart.IO.Mapper.Mappers
{
    using System.Globalization;
    using System.Reflection;
    using System.Text;

    using Smart.IO.Mapper.Mock;
    using Smart.Reflection;

    using Xunit;

    public class LongConvertMapperTest
    {
        private const int Length = 10;

        private static readonly byte[] NullBytes = Encoding.ASCII.GetBytes(string.Empty.PadLeft(Length, ' '));

        private static readonly byte[] ValueBytes = Encoding.ASCII.GetBytes("1".PadLeft(Length, ' '));

        private static readonly byte[] MinusBytes = Encoding.ASCII.GetBytes("-1".PadLeft(Length, ' '));

        private readonly LongConvertMapper longMapper;

        private readonly LongConvertMapper nullableLongMapper;

        private readonly LongConvertMapper enumMapper;

        private readonly LongConvertMapper nullableEnumMapper;

        public LongConvertMapperTest()
        {
            var type = typeof(Target);

            longMapper = CreateMapper(type.GetProperty(nameof(Target.LongProperty)));
            nullableLongMapper = CreateMapper(type.GetProperty(nameof(Target.NullableLongProperty)));
            enumMapper = CreateMapper(type.GetProperty(nameof(Target.LongEnumProperty)));
            nullableEnumMapper = CreateMapper(type.GetProperty(nameof(Target.NullableLongEnumProperty)));
        }

        private static LongConvertMapper CreateMapper(PropertyInfo pi)
        {
            return new LongConvertMapper(
                0,
                Length,
                DelegateFactory.Default.CreateGetter(pi),
                DelegateFactory.Default.CreateSetter(pi),
                Encoding.ASCII,
                true,
                Padding.Left,
                0x20,
                NumberStyles.Integer,
                NumberFormatInfo.InvariantInfo,
                pi.PropertyType);
        }

        //--------------------------------------------------------------------------------
        // long
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToLongIsDefault()
        {
            var target = new Target { LongProperty = 1 };
            longMapper.Read(NullBytes, 0, target);

            Assert.Equal(0L, target.LongProperty);
        }

        [Fact]
        public void ReadValueToLong()
        {
            var target = new Target();
            longMapper.Read(ValueBytes, 0, target);

            Assert.Equal(1L, target.LongProperty);
        }

        [Fact]
        public void WriteValueLongToBuffer()
        {
            var buffer = new byte[Length];
            var target = new Target { LongProperty = 1 };
            longMapper.Write(buffer, 0, target);

            Assert.Equal(ValueBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // long?
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToNullableLongIsDefault()
        {
            var target = new Target { NullableLongProperty = 1 };
            nullableLongMapper.Read(NullBytes, 0, target);

            Assert.Null(target.NullableLongProperty);
        }

        [Fact]
        public void ReadValueToNullableLong()
        {
            var target = new Target();
            nullableLongMapper.Read(ValueBytes, 0, target);

            Assert.Equal(1L, target.NullableLongProperty);
        }

        [Fact]
        public void WriteNullLongToBuffer()
        {
            var buffer = new byte[Length];
            var target = new Target();
            nullableLongMapper.Write(buffer, 0, target);

            Assert.Equal(NullBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // enum
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToEnumIsDefault()
        {
            var target = new Target { LongEnumProperty = LongEnum.One };
            enumMapper.Read(NullBytes, 0, target);

            Assert.Equal(LongEnum.Zero, target.LongEnumProperty);
        }

        [Fact]
        public void ReadValueToEnum()
        {
            var target = new Target();
            enumMapper.Read(ValueBytes, 0, target);

            Assert.Equal(LongEnum.One, target.LongEnumProperty);
        }

        [Fact]
        public void ReadUndefinedValueToEnum()
        {
            var target = new Target();
            enumMapper.Read(MinusBytes, 0, target);

            Assert.Equal((LongEnum)(-1L), target.LongEnumProperty);
        }

        [Fact]
        public void WriteValueEnumToBuffer()
        {
            var buffer = new byte[Length];
            var target = new Target { LongEnumProperty = LongEnum.One };
            enumMapper.Write(buffer, 0, target);

            Assert.Equal(ValueBytes, buffer);
        }

        [Fact]
        public void WriteUndefinedEnumToBuffer()
        {
            var buffer = new byte[Length];
            var target = new Target { LongEnumProperty = (LongEnum)(-1) };
            enumMapper.Write(buffer, 0, target);

            Assert.Equal(MinusBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // enum?
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToNullableEnumIsDefault()
        {
            var target = new Target { NullableLongEnumProperty = LongEnum.One };
            nullableEnumMapper.Read(NullBytes, 0, target);

            Assert.Null(target.NullableLongEnumProperty);
        }

        [Fact]
        public void ReadValueToNullableEnum()
        {
            var target = new Target();
            nullableEnumMapper.Read(ValueBytes, 0, target);

            Assert.Equal(LongEnum.One, target.NullableLongEnumProperty);
        }

        [Fact]
        public void ReadUndefinedValueToNullableEnum()
        {
            var target = new Target();
            nullableEnumMapper.Read(MinusBytes, 0, target);

            Assert.Equal((LongEnum)(-1L), target.NullableLongEnumProperty);
        }

        [Fact]
        public void WriteNullEnumToBuffer()
        {
            var buffer = new byte[Length];
            var target = new Target();
            nullableEnumMapper.Write(buffer, 0, target);

            Assert.Equal(NullBytes, buffer);
        }
    }
}
