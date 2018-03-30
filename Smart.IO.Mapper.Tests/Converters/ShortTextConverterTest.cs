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

        private static readonly byte[] EmptyBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes(string.Empty.PadLeft(Length, ' ')));

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
        public void ReadToShort()
        {
            // Default
            Assert.Equal((short)0, shortConverter.Read(EmptyBytes, Offset));

            // Value
            Assert.Equal((short)1, shortConverter.Read(ValueBytes, Offset));
        }

        [Fact]
        public void WriteShortToBuffer()
        {
            var buffer = new byte[Length + Offset];

            // Value
            shortConverter.Write(buffer, Offset, (short)1);
            Assert.Equal(ValueBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // short?
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadToNullableShort()
        {
            // Null
            Assert.Null(nullableShortConverter.Read(EmptyBytes, Offset));

            // Value
            Assert.Equal((short)1, nullableShortConverter.Read(ValueBytes, Offset));
        }

        [Fact]
        public void WriteNullShortToBuffer()
        {
            var buffer = new byte[Length + Offset];

            // Null
            nullableShortConverter.Write(buffer, Offset, null);
            Assert.Equal(EmptyBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // enum
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadToShortEnum()
        {
            // Default
            Assert.Equal(ShortEnum.Zero, enumConverter.Read(EmptyBytes, Offset));

            // Value
            Assert.Equal(ShortEnum.One, enumConverter.Read(ValueBytes, Offset));

            // Undefined
            Assert.Equal((ShortEnum)(-1), enumConverter.Read(MinusBytes, Offset));
        }

        [Fact]
        public void WriteShortEnumToBuffer()
        {
            var buffer = new byte[Length + Offset];

            // Value
            enumConverter.Write(buffer, Offset, ShortEnum.One);
            Assert.Equal(ValueBytes, buffer);

            // Undefined
            enumConverter.Write(buffer, Offset, (ShortEnum)(-1));
            Assert.Equal(MinusBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // enum?
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadToNullableShortEnum()
        {
            // Null
            Assert.Null(nullableEnumConverter.Read(EmptyBytes, Offset));

            // Value
            Assert.Equal(ShortEnum.One, nullableEnumConverter.Read(ValueBytes, Offset));

            // Undefined
            Assert.Equal((ShortEnum)(-1), nullableEnumConverter.Read(MinusBytes, Offset));
        }

        [Fact]
        public void WriteNullableShortEnumToBuffer()
        {
            var buffer = new byte[Length + Offset];

            // Null
            nullableEnumConverter.Write(buffer, Offset, null);
            Assert.Equal(EmptyBytes, buffer);
        }
    }
}
