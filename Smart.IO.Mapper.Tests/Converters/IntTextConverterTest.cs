namespace Smart.IO.Mapper.Converters
{
    using System;
    using System.Globalization;
    using System.Text;

    using Smart.IO.Mapper.Mock;

    using Xunit;

    public class IntTextConverterTest
    {
        private const int Length = 8;

        private static readonly byte[] NullBytes = Encoding.ASCII.GetBytes(string.Empty.PadLeft(Length, ' '));

        private static readonly byte[] ValueBytes = Encoding.ASCII.GetBytes("1".PadLeft(Length, ' '));

        private static readonly byte[] MinusBytes = Encoding.ASCII.GetBytes("-1".PadLeft(Length, ' '));

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
            Assert.Equal(0, intConverter.Read(NullBytes, 0));
        }

        [Fact]
        public void ReadValueToInt()
        {
            Assert.Equal(1, intConverter.Read(ValueBytes, 0));
        }

        [Fact]
        public void WriteValueIntToBuffer()
        {
            var buffer = new byte[Length];
            intConverter.Write(buffer, 0, 1);

            Assert.Equal(ValueBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // int?
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToNullableIntIsDefault()
        {
            Assert.Null(nullableIntConverter.Read(NullBytes, 0));
        }

        [Fact]
        public void ReadValueToNullableInt()
        {
            Assert.Equal(1, nullableIntConverter.Read(ValueBytes, 0));
        }

        [Fact]
        public void WriteNullIntToBuffer()
        {
            var buffer = new byte[Length];
            nullableIntConverter.Write(buffer, 0, null);

            Assert.Equal(NullBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // enum
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToEnumIsDefault()
        {
            Assert.Equal(IntEnum.Zero, enumConverter.Read(NullBytes, 0));
        }

        [Fact]
        public void ReadValueToEnum()
        {
            Assert.Equal(IntEnum.One, enumConverter.Read(ValueBytes, 0));
        }

        [Fact]
        public void ReadUndefinedValueToEnum()
        {
            Assert.Equal((IntEnum)(-1), enumConverter.Read(MinusBytes, 0));
        }

        [Fact]
        public void WriteValueEnumToBuffer()
        {
            var buffer = new byte[Length];
            enumConverter.Write(buffer, 0, IntEnum.One);

            Assert.Equal(ValueBytes, buffer);
        }

        [Fact]
        public void WriteUndefinedEnumToBuffer()
        {
            var buffer = new byte[Length];
            enumConverter.Write(buffer, 0, (IntEnum)(-1));

            Assert.Equal(MinusBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // enum?
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToNullableEnumIsDefault()
        {
            Assert.Null(nullableEnumConverter.Read(NullBytes, 0));
        }

        [Fact]
        public void ReadValueToNullableEnum()
        {
            Assert.Equal(IntEnum.One, nullableEnumConverter.Read(ValueBytes, 0));
        }

        [Fact]
        public void ReadUndefinedValueToNullableEnum()
        {
            Assert.Equal((IntEnum)(-1), nullableEnumConverter.Read(MinusBytes, 0));
        }

        [Fact]
        public void WriteNullEnumToBuffer()
        {
            var buffer = new byte[Length];
            nullableEnumConverter.Write(buffer, 0, null);

            Assert.Equal(NullBytes, buffer);
        }
    }
}
