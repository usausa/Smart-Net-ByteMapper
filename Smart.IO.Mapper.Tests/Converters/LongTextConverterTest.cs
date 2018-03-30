namespace Smart.IO.Mapper.Converters
{
    using System;
    using System.Globalization;
    using System.Text;

    using Smart.IO.Mapper.Mock;

    using Xunit;

    public class LongTextConverterTest
    {
        private const int Offset = 1;

        private const int Length = 10;

        private static readonly byte[] EmptyBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes(string.Empty.PadLeft(Length, ' ')));

        private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes("1".PadLeft(Length, ' ')));

        private static readonly byte[] MinusBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes("-1".PadLeft(Length, ' ')));

        private readonly LongTextConverter longConverter;

        private readonly LongTextConverter nullableLongConverter;

        private readonly LongTextConverter enumConverter;

        private readonly LongTextConverter nullableEnumConverter;

        public LongTextConverterTest()
        {
            longConverter = CreateConverter(typeof(long));
            nullableLongConverter = CreateConverter(typeof(long?));
            enumConverter = CreateConverter(typeof(LongEnum));
            nullableEnumConverter = CreateConverter(typeof(LongEnum?));
        }

        private static LongTextConverter CreateConverter(Type type)
        {
            return new LongTextConverter(
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
        public void ReadToLong()
        {
            // Default
            Assert.Equal(0L, longConverter.Read(EmptyBytes, Offset));

            // Value
            Assert.Equal(1L, longConverter.Read(ValueBytes, Offset));
        }

        [Fact]
        public void WriteLongToBuffer()
        {
            var buffer = new byte[Length + Offset];

            // Value
            longConverter.Write(buffer, Offset, 1L);
            Assert.Equal(ValueBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // long?
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadToNullableLong()
        {
            // Null
            Assert.Null(nullableLongConverter.Read(EmptyBytes, Offset));

            // Value
            Assert.Equal(1L, nullableLongConverter.Read(ValueBytes, Offset));
        }

        [Fact]
        public void WriteNullLongToBuffer()
        {
            var buffer = new byte[Length + Offset];

            // Null
            nullableLongConverter.Write(buffer, Offset, null);
            Assert.Equal(EmptyBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // enum
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadToLongEnum()
        {
            // Default
            Assert.Equal(LongEnum.Zero, enumConverter.Read(EmptyBytes, Offset));

            // Value
            Assert.Equal(LongEnum.One, enumConverter.Read(ValueBytes, Offset));

            // Undefined
            Assert.Equal((LongEnum)(-1L), enumConverter.Read(MinusBytes, Offset));
        }

        [Fact]
        public void WriteLongEnumToBuffer()
        {
            var buffer = new byte[Length + Offset];

            // Value
            enumConverter.Write(buffer, Offset, LongEnum.One);
            Assert.Equal(ValueBytes, buffer);

            // Undefined
            enumConverter.Write(buffer, Offset, (LongEnum)(-1));
            Assert.Equal(MinusBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // enum?
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadToNullableLongEnum()
        {
            // Null
            Assert.Null(nullableEnumConverter.Read(EmptyBytes, Offset));

            // Value
            Assert.Equal(LongEnum.One, nullableEnumConverter.Read(ValueBytes, Offset));

            // Undefined
            Assert.Equal((LongEnum)(-1L), nullableEnumConverter.Read(MinusBytes, Offset));
        }

        [Fact]
        public void WriteNullableLongEnumToBuffer()
        {
            var buffer = new byte[Length + Offset];

            // Null
            nullableEnumConverter.Write(buffer, Offset, null);
            Assert.Equal(EmptyBytes, buffer);
        }
    }
}
