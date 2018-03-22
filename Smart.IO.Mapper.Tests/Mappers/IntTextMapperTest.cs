namespace Smart.IO.Mapper.Mappers
{
    using System.Globalization;
    using System.Reflection;
    using System.Text;

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
            enumMapper = CreateMapper(type.GetProperty(nameof(Target.EnumProperty)));
            nullableEnumMapper = CreateMapper(type.GetProperty(nameof(Target.NullableEnumProperty)));
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

        //--------------------------------------------------------------------------------
        // enum
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToEnumIsDefault()
        {
            var target = new Target { EnumProperty = MyEnum.One };
            enumMapper.Read(NullBytes, 0, target);

            Assert.Equal(MyEnum.Zero, target.EnumProperty);
        }

        [Fact]
        public void ReadValueToEnum()
        {
            var target = new Target();
            enumMapper.Read(ValueBytes, 0, target);

            Assert.Equal(MyEnum.One, target.EnumProperty);
        }

        [Fact]
        public void ReadUndefinedValueToEnum()
        {
            var target = new Target();
            enumMapper.Read(MinusBytes, 0, target);

            Assert.Equal((MyEnum)(-1), target.EnumProperty);
        }

        //--------------------------------------------------------------------------------
        // enum?
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToNullableEnumIsDefault()
        {
            var target = new Target { NullableEnumProperty = MyEnum.One };
            nullableEnumMapper.Read(NullBytes, 0, target);

            Assert.Null(target.NullableEnumProperty);
        }

        [Fact]
        public void ReadValueToNullableEnum()
        {
            var target = new Target();
            nullableEnumMapper.Read(ValueBytes, 0, target);

            Assert.Equal(MyEnum.One, target.NullableEnumProperty);
        }

        [Fact]
        public void ReadUndefinedValueToNullableEnum()
        {
            var target = new Target();
            nullableEnumMapper.Read(MinusBytes, 0, target);

            Assert.Equal((MyEnum)(-1), target.NullableEnumProperty);
        }

        // write 4+2?, out 2?

        //--------------------------------------------------------------------------------
        // helper
        //--------------------------------------------------------------------------------

        public enum MyEnum
        {
            Zero,
            One,
            Two
        }

        public class Target
        {
            public int IntProperty { get; set; }

            public int? NullableIntProperty { get; set; }

            public MyEnum EnumProperty { get; set; }

            public MyEnum? NullableEnumProperty { get; set; }
        }
    }
}
