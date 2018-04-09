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

        private static readonly byte[] EmptyBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes(string.Empty.PadLeft(Length, ' ')));

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
                null,
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
        public void ReadToInt()
        {
            // Default
            Assert.Equal(0, intConverter.Read(EmptyBytes, Offset));

            // Value
            Assert.Equal(1, intConverter.Read(ValueBytes, Offset));
        }

        [Fact]
        public void WriteIntToBuffer()
        {
            var buffer = new byte[Length + Offset];

            // Value
            intConverter.Write(buffer, Offset, 1);
            Assert.Equal(ValueBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // int?
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadToNullableInt()
        {
            // Null
            Assert.Null(nullableIntConverter.Read(EmptyBytes, Offset));

            // Value
            Assert.Equal(1, nullableIntConverter.Read(ValueBytes, Offset));
        }

        [Fact]
        public void WriteNullIntToBuffer()
        {
            var buffer = new byte[Length + Offset];

            // Null
            nullableIntConverter.Write(buffer, Offset, null);
            Assert.Equal(EmptyBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // enum
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadToIntEnum()
        {
            // Default
            Assert.Equal(IntEnum.Zero, enumConverter.Read(EmptyBytes, Offset));

            // Value
            Assert.Equal(IntEnum.One, enumConverter.Read(ValueBytes, Offset));

            // Undefined
            Assert.Equal((IntEnum)(-1), enumConverter.Read(MinusBytes, Offset));
        }

        [Fact]
        public void WriteIntEnumToBuffer()
        {
            var buffer = new byte[Length + Offset];

            // Value
            enumConverter.Write(buffer, Offset, IntEnum.One);
            Assert.Equal(ValueBytes, buffer);

            // Undefined
            enumConverter.Write(buffer, Offset, (IntEnum)(-1));
            Assert.Equal(MinusBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // enum?
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadToNullableIntEnum()
        {
            // Null
            Assert.Null(nullableEnumConverter.Read(EmptyBytes, Offset));

            // Value
            Assert.Equal(IntEnum.One, nullableEnumConverter.Read(ValueBytes, Offset));

            // Undefined
            Assert.Equal((IntEnum)(-1), nullableEnumConverter.Read(MinusBytes, Offset));
        }

        [Fact]
        public void WriteNullableIntEnumToBuffer()
        {
            var buffer = new byte[Length + Offset];

            // Null
            nullableEnumConverter.Write(buffer, Offset, null);
            Assert.Equal(EmptyBytes, buffer);
        }
    }
}
