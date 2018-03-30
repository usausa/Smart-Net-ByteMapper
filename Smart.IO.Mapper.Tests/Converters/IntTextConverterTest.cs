namespace Smart.IO.Mapper.Converters
{
    using System;
    using System.Globalization;
    using System.Text;

    using Smart.IO.Mapper.Mock;

    using Xunit;

    public class IntTextConverterTest
    {
        private const int Offset = 1;

        private const int Length = 8;

        private static readonly byte[] NullBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes(string.Empty.PadLeft(Length, ' ')));

        private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes("1".PadLeft(Length, ' ')));

        private static readonly byte[] MinusBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes("-1".PadLeft(Length, ' ')));

        private readonly IntTextConverter intConverter;

        private readonly IntTextConverter nullableIntConverter;

        private readonly IntTextConverter enumConverter;

        private readonly IntTextConverter nullableEnumConverter;

        public IntTextConverterTest()
        {
            intConverter = CreateConverter(typeof(int));
            nullableIntConverter = CreateConverter(typeof(int?));
            enumConverter = CreateConverter(typeof(IntEnum));
            nullableEnumConverter = CreateConverter(typeof(IntEnum?));
        }

        private static IntTextConverter CreateConverter(Type type)
        {
            return new IntTextConverter(
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
            Assert.Equal(0, intConverter.Read(NullBytes, Offset));
        }

        [Fact]
        public void ReadValueToInt()
        {
            Assert.Equal(1, intConverter.Read(ValueBytes, Offset));
        }

        [Fact]
        public void WriteValueIntToBuffer()
        {
            var buffer = new byte[Length + Offset];
            intConverter.Write(buffer, Offset, 1);

            Assert.Equal(ValueBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // int?
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToNullableIntIsDefault()
        {
            Assert.Null(nullableIntConverter.Read(NullBytes, Offset));
        }

        [Fact]
        public void ReadValueToNullableInt()
        {
            Assert.Equal(1, nullableIntConverter.Read(ValueBytes, Offset));
        }

        [Fact]
        public void WriteNullIntToBuffer()
        {
            var buffer = new byte[Length + Offset];
            nullableIntConverter.Write(buffer, Offset, null);

            Assert.Equal(NullBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // enum
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToEnumIsDefault()
        {
            Assert.Equal(IntEnum.Zero, enumConverter.Read(NullBytes, Offset));
        }

        [Fact]
        public void ReadValueToEnum()
        {
            Assert.Equal(IntEnum.One, enumConverter.Read(ValueBytes, Offset));
        }

        [Fact]
        public void ReadUndefinedValueToEnum()
        {
            Assert.Equal((IntEnum)(-1), enumConverter.Read(MinusBytes, Offset));
        }

        [Fact]
        public void WriteValueEnumToBuffer()
        {
            var buffer = new byte[Length + Offset];
            enumConverter.Write(buffer, Offset, IntEnum.One);

            Assert.Equal(ValueBytes, buffer);
        }

        [Fact]
        public void WriteUndefinedEnumToBuffer()
        {
            var buffer = new byte[Length + Offset];
            enumConverter.Write(buffer, Offset, (IntEnum)(-1));

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
            Assert.Equal(IntEnum.One, nullableEnumConverter.Read(ValueBytes, Offset));
        }

        [Fact]
        public void ReadUndefinedValueToNullableEnum()
        {
            Assert.Equal((IntEnum)(-1), nullableEnumConverter.Read(MinusBytes, Offset));
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
