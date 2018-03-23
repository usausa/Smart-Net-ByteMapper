namespace Smart.IO.Mapper.Mappers
{
    using System.Globalization;
    using System.Reflection;
    using System.Text;

    using Smart.IO.Mapper.Mock;
    using Smart.Reflection;

    using Xunit;

    public class ShortConvertMapperTest
    {
        private const int Length = 4;

        private static readonly byte[] NullBytes = Encoding.ASCII.GetBytes(string.Empty.PadLeft(Length, ' '));

        private static readonly byte[] ValueBytes = Encoding.ASCII.GetBytes("1".PadLeft(Length, ' '));

        private static readonly byte[] MinusBytes = Encoding.ASCII.GetBytes("-1".PadLeft(Length, ' '));

        private readonly ShortConvertMapper shortMapper;

        private readonly ShortConvertMapper nullableShortMapper;

        private readonly ShortConvertMapper enumMapper;

        private readonly ShortConvertMapper nullableEnumMapper;

        public ShortConvertMapperTest()
        {
            var type = typeof(Target);

            shortMapper = CreateMapper(type.GetProperty(nameof(Target.ShortProperty)));
            nullableShortMapper = CreateMapper(type.GetProperty(nameof(Target.NullableShortProperty)));
            enumMapper = CreateMapper(type.GetProperty(nameof(Target.ShortEnumProperty)));
            nullableEnumMapper = CreateMapper(type.GetProperty(nameof(Target.NullableShortEnumProperty)));
        }

        private static ShortConvertMapper CreateMapper(PropertyInfo pi)
        {
            return new ShortConvertMapper(
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
        // short
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToShortIsDefault()
        {
            var target = new Target { ShortProperty = 1 };
            shortMapper.Read(NullBytes, 0, target);

            Assert.Equal((short)0, target.ShortProperty);
        }

        [Fact]
        public void ReadValueToShort()
        {
            var target = new Target();
            shortMapper.Read(ValueBytes, 0, target);

            Assert.Equal((short)1, target.ShortProperty);
        }

        [Fact]
        public void WriteValueShortToBuffer()
        {
            var buffer = new byte[Length];
            var target = new Target { ShortProperty = 1 };
            shortMapper.Write(buffer, 0, target);

            Assert.Equal(ValueBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // short?
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToNullableShortIsDefault()
        {
            var target = new Target { NullableShortProperty = 1 };
            nullableShortMapper.Read(NullBytes, 0, target);

            Assert.Null(target.NullableShortProperty);
        }

        [Fact]
        public void ReadValueToNullableShort()
        {
            var target = new Target();
            nullableShortMapper.Read(ValueBytes, 0, target);

            Assert.Equal((short)1, target.NullableShortProperty);
        }

        [Fact]
        public void WriteNullShortToBuffer()
        {
            var buffer = new byte[Length];
            var target = new Target();
            nullableShortMapper.Write(buffer, 0, target);

            Assert.Equal(NullBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // enum
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToEnumIsDefault()
        {
            var target = new Target { ShortEnumProperty = ShortEnum.One };
            enumMapper.Read(NullBytes, 0, target);

            Assert.Equal(ShortEnum.Zero, target.ShortEnumProperty);
        }

        [Fact]
        public void ReadValueToEnum()
        {
            var target = new Target();
            enumMapper.Read(ValueBytes, 0, target);

            Assert.Equal(ShortEnum.One, target.ShortEnumProperty);
        }

        [Fact]
        public void ReadUndefinedValueToEnum()
        {
            var target = new Target();
            enumMapper.Read(MinusBytes, 0, target);

            Assert.Equal((ShortEnum)(-1), target.ShortEnumProperty);
        }

        [Fact]
        public void WriteValueEnumToBuffer()
        {
            var buffer = new byte[Length];
            var target = new Target { ShortEnumProperty = ShortEnum.One };
            enumMapper.Write(buffer, 0, target);

            Assert.Equal(ValueBytes, buffer);
        }

        [Fact]
        public void WriteUndefinedEnumToBuffer()
        {
            var buffer = new byte[Length];
            var target = new Target { ShortEnumProperty = (ShortEnum)(-1) };
            enumMapper.Write(buffer, 0, target);

            Assert.Equal(MinusBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // enum?
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToNullableEnumIsDefault()
        {
            var target = new Target { NullableShortEnumProperty = ShortEnum.One };
            nullableEnumMapper.Read(NullBytes, 0, target);

            Assert.Null(target.NullableShortEnumProperty);
        }

        [Fact]
        public void ReadValueToNullableEnum()
        {
            var target = new Target();
            nullableEnumMapper.Read(ValueBytes, 0, target);

            Assert.Equal(ShortEnum.One, target.NullableShortEnumProperty);
        }

        [Fact]
        public void ReadUndefinedValueToNullableEnum()
        {
            var target = new Target();
            nullableEnumMapper.Read(MinusBytes, 0, target);

            Assert.Equal((ShortEnum)(-1), target.NullableShortEnumProperty);
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
