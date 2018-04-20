namespace Smart.IO.ByteMapper.Converters
{
    using System;
    using System.Globalization;
    using System.Text;

    using Smart.IO.ByteMapper.Mock;

    using Xunit;

    public class Int32TextConverterTest
    {
        private const int Offset = 1;

        private const int Length = 8;

        private static readonly byte[] EmptyBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes(string.Empty.PadLeft(Length, ' ')));

        private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes("1".PadLeft(Length, ' ')));

        private static readonly byte[] MinusBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes("-1".PadLeft(Length, ' ')));

        private readonly Int32TextConverter int32Converter;

        private readonly Int32TextConverter nullableInt32Converter;

        private readonly Int32TextConverter enumConverter;

        private readonly Int32TextConverter nullableEnumConverter;

        public Int32TextConverterTest()
        {
            int32Converter = CreateConverter(typeof(int));
            nullableInt32Converter = CreateConverter(typeof(int?));
            enumConverter = CreateConverter(typeof(IntEnum));
            nullableEnumConverter = CreateConverter(typeof(IntEnum?));
        }

        private static Int32TextConverter CreateConverter(Type type)
        {
            return new Int32TextConverter(
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
            Assert.Equal(0, int32Converter.Read(EmptyBytes, Offset));

            // Value
            Assert.Equal(1, int32Converter.Read(ValueBytes, Offset));
        }

        [Fact]
        public void WriteIntToBuffer()
        {
            var buffer = new byte[Length + Offset];

            // Value
            int32Converter.Write(buffer, Offset, 1);
            Assert.Equal(ValueBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // int?
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadToNullableInt()
        {
            // Null
            Assert.Null(nullableInt32Converter.Read(EmptyBytes, Offset));

            // Value
            Assert.Equal(1, nullableInt32Converter.Read(ValueBytes, Offset));
        }

        [Fact]
        public void WriteNullIntToBuffer()
        {
            var buffer = new byte[Length + Offset];

            // Null
            nullableInt32Converter.Write(buffer, Offset, null);
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
