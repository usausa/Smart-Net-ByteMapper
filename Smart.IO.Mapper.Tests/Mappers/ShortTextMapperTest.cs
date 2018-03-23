namespace Smart.IO.Mapper.Mappers
{
    using System.Globalization;
    using System.Reflection;
    using System.Text;

    using Smart.IO.Mapper.Mock;
    using Smart.Reflection;

    using Xunit;

    public class IntTextMapperTest
    {
        private const int Length = 8;

        private static readonly byte[] NullBytes = Encoding.ASCII.GetBytes(string.Empty.PadLeft(Length, ' '));

        private static readonly byte[] ValueBytes = Encoding.ASCII.GetBytes("1".PadLeft(Length, ' '));

        private static readonly byte[] MinusBytes = Encoding.ASCII.GetBytes("-1".PadLeft(Length, ' '));

        private readonly IntTextMapper intMapper;

        private readonly IntTextMapper nullableIntMapper;

        private readonly IntTextMapper enumMapper;

        private readonly IntTextMapper nullableEnumMapper;

        public IntTextMapperTest()
        {
            var type = typeof(Target);

            intMapper = CreateMapper(type.GetProperty(nameof(Target.IntProperty)));
            nullableIntMapper = CreateMapper(type.GetProperty(nameof(Target.NullableIntProperty)));
            enumMapper = CreateMapper(type.GetProperty(nameof(Target.IntEnumProperty)));
            nullableEnumMapper = CreateMapper(type.GetProperty(nameof(Target.NullableIntEnumProperty)));
        }

        private static IntTextMapper CreateMapper(PropertyInfo pi)
        {
            return new IntTextMapper(
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
        // int
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToIntIsDefault()
        {
            var target = new Target { IntProperty = 1 };
            intMapper.Read(NullBytes, 0, target);

            Assert.Equal(0, target.IntProperty);
        }

        [Fact]
        public void ReadValueToInt()
        {
            var target = new Target();
            intMapper.Read(ValueBytes, 0, target);

            Assert.Equal(1, target.IntProperty);
        }

        [Fact]
        public void WriteValueIntToBuffer()
        {
            var buffer = new byte[Length];
            var target = new Target { IntProperty = 1 };
            intMapper.Write(buffer, 0, target);

            Assert.Equal(ValueBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // int?
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToNullableIntIsDefault()
        {
            var target = new Target { NullableIntProperty = 1 };
            nullableIntMapper.Read(NullBytes, 0, target);

            Assert.Null(target.NullableIntProperty);
        }

        [Fact]
        public void ReadValueToNullableInt()
        {
            var target = new Target();
            nullableIntMapper.Read(ValueBytes, 0, target);

            Assert.Equal(1, target.NullableIntProperty);
        }

        [Fact]
        public void WriteNullIntToBuffer()
        {
            var buffer = new byte[Length];
            var target = new Target();
            nullableIntMapper.Write(buffer, 0, target);

            Assert.Equal(NullBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // enum
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToEnumIsDefault()
        {
            var target = new Target { IntEnumProperty = IntEnum.One };
            enumMapper.Read(NullBytes, 0, target);

            Assert.Equal(IntEnum.Zero, target.IntEnumProperty);
        }

        [Fact]
        public void ReadValueToEnum()
        {
            var target = new Target();
            enumMapper.Read(ValueBytes, 0, target);

            Assert.Equal(IntEnum.One, target.IntEnumProperty);
        }

        [Fact]
        public void ReadUndefinedValueToEnum()
        {
            var target = new Target();
            enumMapper.Read(MinusBytes, 0, target);

            Assert.Equal((IntEnum)(-1), target.IntEnumProperty);
        }

        [Fact]
        public void WriteValueEnumToBuffer()
        {
            var buffer = new byte[Length];
            var target = new Target { IntEnumProperty = IntEnum.One };
            enumMapper.Write(buffer, 0, target);

            Assert.Equal(ValueBytes, buffer);
        }

        [Fact]
        public void WriteUndefinedEnumToBuffer()
        {
            var buffer = new byte[Length];
            var target = new Target { IntEnumProperty = (IntEnum)(-1) };
            enumMapper.Write(buffer, 0, target);

            Assert.Equal(MinusBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // enum?
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToNullableEnumIsDefault()
        {
            var target = new Target { NullableIntEnumProperty = IntEnum.One };
            nullableEnumMapper.Read(NullBytes, 0, target);

            Assert.Null(target.NullableIntEnumProperty);
        }

        [Fact]
        public void ReadValueToNullableEnum()
        {
            var target = new Target();
            nullableEnumMapper.Read(ValueBytes, 0, target);

            Assert.Equal(IntEnum.One, target.NullableIntEnumProperty);
        }

        [Fact]
        public void ReadUndefinedValueToNullableEnum()
        {
            var target = new Target();
            nullableEnumMapper.Read(MinusBytes, 0, target);

            Assert.Equal((IntEnum)(-1), target.NullableIntEnumProperty);
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
