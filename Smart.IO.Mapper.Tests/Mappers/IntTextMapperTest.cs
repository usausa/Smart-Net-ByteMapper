namespace Smart.IO.Mapper.Mappers
{
    using System;
    using System.Globalization;
    using System.Text;

    using Smart.IO.Mapper.Mock;

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
            intMapper = CreateMapper(typeof(int));
            nullableIntMapper = CreateMapper(typeof(int?));
            enumMapper = CreateMapper(typeof(IntEnum));
            nullableEnumMapper = CreateMapper(typeof(IntEnum?));
        }

        private static IntTextMapper CreateMapper(Type type)
        {
            return new IntTextMapper(
                Length,
                Encoding.ASCII,
                true,
                Padding.Left,
                0x20,
                NumberStyles.Integer,
                NumberFormatInfo.InvariantInfo,
                type);
        }

        //--------------------------------------------------------------------------------
        // int
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToIntIsDefault()
        {
            Assert.Equal(0, intMapper.Read(NullBytes, 0));
        }

        [Fact]
        public void ReadValueToInt()
        {
            Assert.Equal(1, intMapper.Read(ValueBytes, 0));
        }

        [Fact]
        public void WriteValueIntToBuffer()
        {
            var buffer = new byte[Length];
            intMapper.Write(buffer, 0, 1);

            Assert.Equal(ValueBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // int?
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToNullableIntIsDefault()
        {
            Assert.Null(nullableIntMapper.Read(NullBytes, 0));
        }

        [Fact]
        public void ReadValueToNullableInt()
        {
            Assert.Equal(1, nullableIntMapper.Read(ValueBytes, 0));
        }

        [Fact]
        public void WriteNullIntToBuffer()
        {
            var buffer = new byte[Length];
            nullableIntMapper.Write(buffer, 0, null);

            Assert.Equal(NullBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // enum
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToEnumIsDefault()
        {
            Assert.Equal(IntEnum.Zero, enumMapper.Read(NullBytes, 0));
        }

        [Fact]
        public void ReadValueToEnum()
        {
            Assert.Equal(IntEnum.One, enumMapper.Read(ValueBytes, 0));
        }

        [Fact]
        public void ReadUndefinedValueToEnum()
        {
            Assert.Equal((IntEnum)(-1), enumMapper.Read(MinusBytes, 0));
        }

        [Fact]
        public void WriteValueEnumToBuffer()
        {
            var buffer = new byte[Length];
            enumMapper.Write(buffer, 0, IntEnum.One);

            Assert.Equal(ValueBytes, buffer);
        }

        [Fact]
        public void WriteUndefinedEnumToBuffer()
        {
            var buffer = new byte[Length];
            enumMapper.Write(buffer, 0, (IntEnum)(-1));

            Assert.Equal(MinusBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // enum?
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToNullableEnumIsDefault()
        {
            Assert.Null(nullableEnumMapper.Read(NullBytes, 0));
        }

        [Fact]
        public void ReadValueToNullableEnum()
        {
            Assert.Equal(IntEnum.One, nullableEnumMapper.Read(ValueBytes, 0));
        }

        [Fact]
        public void ReadUndefinedValueToNullableEnum()
        {
            Assert.Equal((IntEnum)(-1), nullableEnumMapper.Read(MinusBytes, 0));
        }

        [Fact]
        public void WriteNullEnumToBuffer()
        {
            var buffer = new byte[Length];
            nullableEnumMapper.Write(buffer, 0, null);

            Assert.Equal(NullBytes, buffer);
        }
    }
}
