namespace Smart.IO.Mapper.Attributes
{
    using System;
    using System.Globalization;
    using System.Text;

    using Xunit;

    public class MapNumberTextAttributeTest
    {
        //--------------------------------------------------------------------------------
        // Attribute
        //--------------------------------------------------------------------------------

        [Fact]
        public void MapByNumberTextAttribute()
        {
            var mapperFactory = new MapperFactoryConfig()
                .DefaultDelimiter(null)
                .DefaultEncoding(Encoding.ASCII)
                .DefaultNumberProvider(CultureInfo.InvariantCulture)
                .DefaultTrim(true)
                .DefaultNumberPadding(Padding.Left)
                .DefaultNumberFiller(0x20)
                .DefaultNumberStyle(NumberStyles.Integer)
                .DefaultDecimalStyle(NumberStyles.Any)
                .CreateMapByAttribute<NumberTextAttributeObject>()
                .ToMapperFactory();
            var mapper = mapperFactory.Create<NumberTextAttributeObject>();

            var buffer = new byte[mapper.Size];
            var obj = new NumberTextAttributeObject
            {
                IntValue = 1,
                CustomIntValue = -1,
                LongValue = 1L,
                CustomLongValue = -1L,
                ShortValue = 1,
                CustomShortValue = -1,
                DecimalValue = 12.34m,
                CustomDecimalValue = -12.3m
            };

            // Write
            mapper.ToByte(buffer, 0, obj);

            Assert.Equal(
                Encoding.ASCII.GetBytes(
                    "   1" +
                    "    " +
                    "-1__" +
                    "     1" +
                    "      " +
                    "-1____" +
                    " 1" +
                    "  " +
                    "-1" +
                    " 12.34" +
                    "      " +
                    "-12.3_"),
                buffer);

            // Read
            mapper.FromByte(
                Encoding.ASCII.GetBytes(
                    "   2" +
                    "   2" +
                    "-2__" +
                    "     2" +
                    "     2" +
                    "-2____" +
                    " 2" +
                    " 2" +
                    "-2" +
                    "  23.4" +
                    "  23.4" +
                    "-23.45"),
                obj);

            Assert.Equal(2, obj.IntValue);
            Assert.Equal(2, obj.NullableIntValue);
            Assert.Equal(-2, obj.CustomIntValue);
            Assert.Equal(2L, obj.LongValue);
            Assert.Equal(2L, obj.NullableLongValue);
            Assert.Equal(-2L, obj.CustomLongValue);
            Assert.Equal((short)2, obj.ShortValue);
            Assert.Equal((short)2, obj.NullableShortValue);
            Assert.Equal((short)-2, obj.CustomShortValue);
            Assert.Equal(23.4m, obj.DecimalValue);
            Assert.Equal(23.4m, obj.NullableDecimalValue);
            Assert.Equal(-23.45m, obj.CustomDecimalValue);
        }

        //--------------------------------------------------------------------------------
        // Fix
        //--------------------------------------------------------------------------------

        [Fact]
        public void CoverageFix()
        {
            var attribute = new MapNumberTextAttribute(0, 0);

            Assert.Throws<NotSupportedException>(() => attribute.CodePage);
            Assert.Throws<NotSupportedException>(() => attribute.EncodingName);
            Assert.Throws<NotSupportedException>(() => attribute.Trim);
            Assert.Throws<NotSupportedException>(() => attribute.Padding);
            Assert.Throws<NotSupportedException>(() => attribute.Filler);
            Assert.Throws<NotSupportedException>(() => attribute.Style);
            Assert.Throws<NotSupportedException>(() => attribute.Culture);
        }

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        [Map(54)]
        internal class NumberTextAttributeObject
        {
            // int

            [MapNumberText(0, 4)]
            public int IntValue { get; set; }

            [MapNumberText(4, 4)]
            public int? NullableIntValue { get; set; }

            [MapNumberText(8, 4, EncodingName = "ASCII", Trim = true, Padding = Padding.Right, Filler = (byte)'_', Style = NumberStyles.Any, Culture = Culture.Invaliant)]
            public int CustomIntValue { get; set; }

            // long

            [MapNumberText(12, 6)]
            public long LongValue { get; set; }

            [MapNumberText(18, 6)]
            public long? NullableLongValue { get; set; }

            [MapNumberText(24, 6, CodePage = 20127, Trim = true, Padding = Padding.Right, Filler = (byte)'_', Style = NumberStyles.Any, Culture = Culture.Invaliant)]
            public long CustomLongValue { get; set; }

            // short

            [MapNumberText(30, 2)]
            public short ShortValue { get; set; }

            [MapNumberText(32, 2)]
            public short? NullableShortValue { get; set; }

            [MapNumberText(34, 2, EncodingName = "ASCII", Trim = true, Padding = Padding.Right, Filler = (byte)'_', Style = NumberStyles.Any, Culture = Culture.Invaliant)]
            public short CustomShortValue { get; set; }

            // decimal

            [MapNumberText(36, 6)]
            public decimal DecimalValue { get; set; }

            [MapNumberText(42, 6)]
            public decimal? NullableDecimalValue { get; set; }

            [MapNumberText(48, 6, CodePage = 20127, Trim = true, Padding = Padding.Right, Filler = (byte)'_', Style = NumberStyles.Any, Culture = Culture.Invaliant)]
            public decimal CustomDecimalValue { get; set; }
        }
    }
}
