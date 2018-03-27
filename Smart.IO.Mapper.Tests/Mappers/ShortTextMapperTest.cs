namespace Smart.IO.Mapper.Mappers
{
    using System;
    using System.Globalization;
    using System.Text;

    using Smart.IO.Mapper.Mock;

    using Xunit;

    public class ShortTextMapperTest
    {
        private const int Length = 4;

        private static readonly byte[] NullBytes = Encoding.ASCII.GetBytes(string.Empty.PadLeft(Length, ' '));

        private static readonly byte[] ValueBytes = Encoding.ASCII.GetBytes("1".PadLeft(Length, ' '));

        private static readonly byte[] MinusBytes = Encoding.ASCII.GetBytes("-1".PadLeft(Length, ' '));

        private readonly ShortTextMapper shortMapper;

        private readonly ShortTextMapper nullableShortMapper;

        private readonly ShortTextMapper enumMapper;

        private readonly ShortTextMapper nullableEnumMapper;

        public ShortTextMapperTest()
        {
            shortMapper = CreateMapper(typeof(short));
            nullableShortMapper = CreateMapper(typeof(short?));
            enumMapper = CreateMapper(typeof(ShortEnum));
            nullableEnumMapper = CreateMapper(typeof(ShortEnum?));
        }

        private static ShortTextMapper CreateMapper(Type type)
        {
            return new ShortTextMapper(
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
        // short
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToShortIsDefault()
        {
            Assert.Equal((short)0, shortMapper.Read(NullBytes, 0));
        }

        [Fact]
        public void ReadValueToShort()
        {
            Assert.Equal((short)1, shortMapper.Read(ValueBytes, 0));
        }

        [Fact]
        public void WriteValueShortToBuffer()
        {
            var buffer = new byte[Length];
            shortMapper.Write(buffer, 0, (short)1);

            Assert.Equal(ValueBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // short?
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToNullableShortIsDefault()
        {
            Assert.Null(nullableShortMapper.Read(NullBytes, 0));
        }

        [Fact]
        public void ReadValueToNullableShort()
        {
            Assert.Equal((short)1, nullableShortMapper.Read(ValueBytes, 0));
        }

        [Fact]
        public void WriteNullShortToBuffer()
        {
            var buffer = new byte[Length];
            nullableShortMapper.Write(buffer, 0, null);

            Assert.Equal(NullBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // enum
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToEnumIsDefault()
        {
            Assert.Equal(ShortEnum.Zero, enumMapper.Read(NullBytes, 0));
        }

        [Fact]
        public void ReadValueToEnum()
        {
            var ret = enumMapper.Read(ValueBytes, 0);

            Assert.Equal(ShortEnum.One, enumMapper.Read(ValueBytes, 0));
        }

        [Fact]
        public void ReadUndefinedValueToEnum()
        {
            Assert.Equal((ShortEnum)(-1), enumMapper.Read(MinusBytes, 0));
        }

        [Fact]
        public void WriteValueEnumToBuffer()
        {
            var buffer = new byte[Length];
            enumMapper.Write(buffer, 0, ShortEnum.One);

            Assert.Equal(ValueBytes, buffer);
        }

        [Fact]
        public void WriteUndefinedEnumToBuffer()
        {
            var buffer = new byte[Length];
            enumMapper.Write(buffer, 0, (ShortEnum)(-1));

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
            Assert.Equal(ShortEnum.One, nullableEnumMapper.Read(ValueBytes, 0));
        }

        [Fact]
        public void ReadUndefinedValueToNullableEnum()
        {
            Assert.Equal((ShortEnum)(-1), nullableEnumMapper.Read(MinusBytes, 0));
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
