namespace Smart.IO.Mapper.Mappers
{
    using System;
    using System.Globalization;
    using System.Text;

    using Smart.IO.Mapper.Mock;

    using Xunit;

    public class LongTextMapperTest
    {
        private const int Length = 10;

        private static readonly byte[] NullBytes = Encoding.ASCII.GetBytes(string.Empty.PadLeft(Length, ' '));

        private static readonly byte[] ValueBytes = Encoding.ASCII.GetBytes("1".PadLeft(Length, ' '));

        private static readonly byte[] MinusBytes = Encoding.ASCII.GetBytes("-1".PadLeft(Length, ' '));

        private readonly LongTextMapper longMapper;

        private readonly LongTextMapper nullableLongMapper;

        private readonly LongTextMapper enumMapper;

        private readonly LongTextMapper nullableEnumMapper;

        public LongTextMapperTest()
        {
            longMapper = CreateMapper(typeof(long));
            nullableLongMapper = CreateMapper(typeof(long?));
            enumMapper = CreateMapper(typeof(LongEnum));
            nullableEnumMapper = CreateMapper(typeof(LongEnum?));
        }

        private static LongTextMapper CreateMapper(Type type)
        {
            return new LongTextMapper(
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
        // long
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToLongIsDefault()
        {
            Assert.Equal(0L, longMapper.Read(NullBytes, 0));
        }

        [Fact]
        public void ReadValueToLong()
        {
            Assert.Equal(1L, longMapper.Read(ValueBytes, 0));
        }

        [Fact]
        public void WriteValueLongToBuffer()
        {
            var buffer = new byte[Length];
            longMapper.Write(buffer, 0, 1L);

            Assert.Equal(ValueBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // long?
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToNullableLongIsDefault()
        {
            Assert.Null(nullableLongMapper.Read(NullBytes, 0));
        }

        [Fact]
        public void ReadValueToNullableLong()
        {
            Assert.Equal(1L, nullableLongMapper.Read(ValueBytes, 0));
        }

        [Fact]
        public void WriteNullLongToBuffer()
        {
            var buffer = new byte[Length];
            nullableLongMapper.Write(buffer, 0, null);

            Assert.Equal(NullBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // enum
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToEnumIsDefault()
        {
            Assert.Equal(LongEnum.Zero, enumMapper.Read(NullBytes, 0));
        }

        [Fact]
        public void ReadValueToEnum()
        {
            Assert.Equal(LongEnum.One, enumMapper.Read(ValueBytes, 0));
        }

        [Fact]
        public void ReadUndefinedValueToEnum()
        {
            Assert.Equal((LongEnum)(-1L), enumMapper.Read(MinusBytes, 0));
        }

        [Fact]
        public void WriteValueEnumToBuffer()
        {
            var buffer = new byte[Length];
            enumMapper.Write(buffer, 0, LongEnum.One);

            Assert.Equal(ValueBytes, buffer);
        }

        [Fact]
        public void WriteUndefinedEnumToBuffer()
        {
            var buffer = new byte[Length];
            enumMapper.Write(buffer, 0, (LongEnum)(-1));

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
            Assert.Equal(LongEnum.One, nullableEnumMapper.Read(ValueBytes, 0));
        }

        [Fact]
        public void ReadUndefinedValueToNullableEnum()
        {
            Assert.Equal((LongEnum)(-1L), nullableEnumMapper.Read(MinusBytes, 0));
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
