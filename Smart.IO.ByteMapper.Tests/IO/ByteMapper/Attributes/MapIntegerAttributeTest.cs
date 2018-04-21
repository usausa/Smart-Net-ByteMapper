namespace Smart.IO.ByteMapper.Attributes
{
    using System;
    using System.Text;

    using Xunit;

    public class MapIntegerAttributeTest
    {
        //--------------------------------------------------------------------------------
        // Attribute
        //--------------------------------------------------------------------------------

        [Fact]
        public void MapByIntegerAttribute()
        {
            var mapperFactory = new MapperFactoryConfig()
                .UseOptionsDefault()
                .DefaultDelimiter(null)
                .DefaultZeroFill(false)
                .DefaultNumberPadding(Padding.Left)
                .DefaultNumberFiller(0x20)
                .CreateMapByAttribute<IntegerAttributeObject>()
                .ToMapperFactory();
            var mapper = mapperFactory.Create<IntegerAttributeObject>();

            var buffer = new byte[mapper.Size];
            var obj = new IntegerAttributeObject
            {
                IntValue = 1,
                PaddingRightIntValue = -1,
                ZeroFillIntValue = 1,
                LongValue = 1L,
                PaddingRightLongValue = -1L,
                ZeroFillLongValue = 1,
                ShortValue = 1,
                PaddingRightShortValue = -1,
                ZeroFillShortValue = 1,
            };

            // Write
            mapper.ToByte(buffer, 0, obj);
            Assert.Equal(
                Encoding.ASCII.GetBytes(
                    "   1" +
                    "____" +
                    "-1  " +
                    "0001" +
                    "     1" +
                    "______" +
                    "-1    " +
                    "000001" +
                    " 1" +
                    "__" +
                    "-1" +
                    "01"),
                buffer);

            // Read
            mapper.FromByte(
                Encoding.ASCII.GetBytes(
                    "   2" +
                    "___2" +
                    "-2  " +
                    "0002" +
                    "     2" +
                    "_____2" +
                    "-2    " +
                    "000002" +
                    " 2" +
                    "_2" +
                    "-2" +
                    "02"),
                obj);

            Assert.Equal(2, obj.IntValue);
            Assert.Equal(2, obj.NullableIntValue);
            Assert.Equal(-2, obj.PaddingRightIntValue);
            Assert.Equal(2, obj.ZeroFillIntValue);
            Assert.Equal(2L, obj.LongValue);
            Assert.Equal(2L, obj.NullableLongValue);
            Assert.Equal(-2L, obj.PaddingRightLongValue);
            Assert.Equal(2L, obj.ZeroFillLongValue);
            Assert.Equal((short)2, obj.ShortValue);
            Assert.Equal((short)2, obj.NullableShortValue);
            Assert.Equal(-2, obj.PaddingRightShortValue);
            Assert.Equal((short)2, obj.ZeroFillShortValue);
        }

        //--------------------------------------------------------------------------------
        // Fix
        //--------------------------------------------------------------------------------

        [Fact]
        public void CoverageFix()
        {
            var attribute = new MapIntegerAttribute(0, 0);

            Assert.Throws<NotSupportedException>(() => attribute.Padding);
            Assert.Throws<NotSupportedException>(() => attribute.ZeroFill);
            Assert.Throws<NotSupportedException>(() => attribute.Filler);

            Assert.Throws<ArgumentOutOfRangeException>(() => new MapIntegerAttribute(-1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new MapIntegerAttribute(0, -1));
        }

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        [Map(48)]
        internal class IntegerAttributeObject
        {
            // int

            [MapInteger(0, 4)]
            public int IntValue { get; set; }

            [MapInteger(4, 4, Filler = (byte)'_')]
            public int? NullableIntValue { get; set; }

            [MapInteger(8, 4, Padding = Padding.Right)]
            public int PaddingRightIntValue { get; set; }

            [MapInteger(12, 4, ZeroFill = true)]
            public int ZeroFillIntValue { get; set; }

            // long

            [MapInteger(16, 6)]
            public long LongValue { get; set; }

            [MapInteger(22, 6, Filler = (byte)'_')]
            public long? NullableLongValue { get; set; }

            [MapInteger(28, 6, Padding = Padding.Right)]
            public long PaddingRightLongValue { get; set; }

            [MapInteger(34, 6, ZeroFill = true)]
            public long ZeroFillLongValue { get; set; }

            // short

            [MapInteger(40, 2)]
            public short ShortValue { get; set; }

            [MapInteger(42, 2, Filler = (byte)'_')]
            public short? NullableShortValue { get; set; }

            [MapInteger(44, 2, Padding = Padding.Right)]
            public short PaddingRightShortValue { get; set; }

            [MapInteger(46, 2, ZeroFill = true)]
            public short ZeroFillShortValue { get; set; }
        }
    }
}
