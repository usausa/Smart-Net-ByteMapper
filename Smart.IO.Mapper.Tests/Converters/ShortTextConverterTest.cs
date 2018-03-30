namespace Smart.IO.Mapper.Converters
{
    using System;
    using System.Globalization;
    using System.Text;

    using Smart.IO.Mapper.Mock;

    using Xunit;

    public class ShortTextConverterTest
    {
        private const int Offset = 1;

        private const int Length = 4;

        private static readonly byte[] NullBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes(string.Empty.PadLeft(Length, ' ')));

        private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes("1".PadLeft(Length, ' ')));

        private static readonly byte[] MinusBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes("-1".PadLeft(Length, ' ')));

        private readonly ShortTextConverter shortConverter;

        private readonly ShortTextConverter nullableShortConverter;

        private readonly ShortTextConverter enumConverter;

        private readonly ShortTextConverter nullableEnumConverter;

        public ShortTextConverterTest()
        {
            shortConverter = CreateConverter(typeof(short));
            nullableShortConverter = CreateConverter(typeof(short?));
            enumConverter = CreateConverter(typeof(ShortEnum));
            nullableEnumConverter = CreateConverter(typeof(ShortEnum?));
        }

        private static ShortTextConverter CreateConverter(Type type)
        {
            return new ShortTextConverter(
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
            Assert.Equal((short)0, shortConverter.Read(NullBytes, Offset));
        }

        [Fact]
        public void ReadValueToShort()
        {
            Assert.Equal((short)1, shortConverter.Read(ValueBytes, Offset));
        }

        [Fact]
        public void WriteValueShortToBuffer()
        {
            var buffer = new byte[Length + Offset];
            shortConverter.Write(buffer, Offset, (short)1);

            Assert.Equal(ValueBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // short?
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToNullableShortIsDefault()
        {
            Assert.Null(nullableShortConverter.Read(NullBytes, Offset));
        }

        [Fact]
        public void ReadValueToNullableShort()
        {
            Assert.Equal((short)1, nullableShortConverter.Read(ValueBytes, Offset));
        }

        [Fact]
        public void WriteNullShortToBuffer()
        {
            var buffer = new byte[Length + Offset];
            nullableShortConverter.Write(buffer, Offset, null);

            Assert.Equal(NullBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // enum
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToEnumIsDefault()
        {
            Assert.Equal(ShortEnum.Zero, enumConverter.Read(NullBytes, Offset));
        }

        [Fact]
        public void ReadValueToEnum()
        {
            Assert.Equal(ShortEnum.One, enumConverter.Read(ValueBytes, Offset));
        }

        [Fact]
        public void ReadUndefinedValueToEnum()
        {
            Assert.Equal((ShortEnum)(-1), enumConverter.Read(MinusBytes, Offset));
        }

        [Fact]
        public void WriteValueEnumToBuffer()
        {
            var buffer = new byte[Length + Offset];
            enumConverter.Write(buffer, Offset, ShortEnum.One);

            Assert.Equal(ValueBytes, buffer);
        }

        [Fact]
        public void WriteUndefinedEnumToBuffer()
        {
            var buffer = new byte[Length + Offset];
            enumConverter.Write(buffer, Offset, (ShortEnum)(-1));

            Assert.Equal(MinusBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // enum?
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToNullableEnumIsDefault()
        {
            Assert.Null(nullableEnumConverter.Read(NullBytes, Offset));
        }

        [Fact]
        public void ReadValueToNullableEnum()
        {
            Assert.Equal(ShortEnum.One, nullableEnumConverter.Read(ValueBytes, Offset));
        }

        [Fact]
        public void ReadUndefinedValueToNullableEnum()
        {
            Assert.Equal((ShortEnum)(-1), nullableEnumConverter.Read(MinusBytes, Offset));
        }

        [Fact]
        public void WriteNullEnumToBuffer()
        {
            var buffer = new byte[Length + Offset];
            nullableEnumConverter.Write(buffer, Offset, null);

            Assert.Equal(NullBytes, buffer);
        }
    }
}
