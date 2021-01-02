namespace Smart.IO.ByteMapper.Converters
{
    using System;
    using System.Text;

    using Smart.IO.ByteMapper.Mock;

    using Xunit;

    public class Int64ConverterTest
    {
        private const int Offset = 1;

        private const int Length = 10;

        private static readonly byte[] EmptyBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes(string.Empty.PadLeft(Length, ' ')));

        private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes("1".PadLeft(Length, ' ')));

        private static readonly byte[] MinusBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes("-1".PadLeft(Length, ' ')));

        private readonly Int64Converter int64Converter;

        private readonly Int64Converter nullableInt64Converter;

        private readonly Int64Converter enumConverter;

        private readonly Int64Converter nullableEnumConverter;

        public Int64ConverterTest()
        {
            int64Converter = CreateConverter(typeof(long));
            nullableInt64Converter = CreateConverter(typeof(long?));
            enumConverter = CreateConverter(typeof(LongEnum));
            nullableEnumConverter = CreateConverter(typeof(LongEnum?));
        }

        private static Int64Converter CreateConverter(Type type)
        {
            return new(Length, Padding.Left, false, 0x20, type);
        }

        //--------------------------------------------------------------------------------
        // long
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadToLong()
        {
            // Default
            Assert.Equal(0L, int64Converter.Read(EmptyBytes, Offset));

            // Value
            Assert.Equal(1L, int64Converter.Read(ValueBytes, Offset));
        }

        [Fact]
        public void WriteLongToBuffer()
        {
            var buffer = new byte[Length + Offset];

            // Value
            int64Converter.Write(buffer, Offset, 1L);
            Assert.Equal(ValueBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // long?
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadToNullableLong()
        {
            // Null
            Assert.Null(nullableInt64Converter.Read(EmptyBytes, Offset));

            // Value
            Assert.Equal(1L, nullableInt64Converter.Read(ValueBytes, Offset));
        }

        [Fact]
        public void WriteNullLongToBuffer()
        {
            var buffer = new byte[Length + Offset];

            // Null
            nullableInt64Converter.Write(buffer, Offset, null);
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
