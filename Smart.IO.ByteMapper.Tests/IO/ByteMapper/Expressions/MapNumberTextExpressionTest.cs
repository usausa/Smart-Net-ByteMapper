namespace Smart.IO.ByteMapper.Expressions
{
    using System;
    using System.Globalization;
    using System.Text;

    using Smart.Functional;

    using Xunit;

    public class MapNumberTextExpressionTest
    {
        //--------------------------------------------------------------------------------
        // Expression
        //--------------------------------------------------------------------------------

        [Fact]
        public void MapByNumberTextExpression()
        {
            var mapperFactory = new MapperFactoryConfig()
                .UseOptionsDefault()
                .DefaultDelimiter(null)
                .DefaultTrim(true)
                .DefaultNumberTextEncoding(Encoding.ASCII)
                .DefaultNumberTextProvider(CultureInfo.InvariantCulture)
                .DefaultNumberTextNumberStyle(NumberStyles.Integer)
                .DefaultNumberTextDecimalStyle(NumberStyles.Any)
                .DefaultNumberTextPadding(Padding.Left)
                .DefaultNumberTextFiller(0x20)
                .Also(config =>
                {
                    config
                        .CreateMapByExpression<NumberTextExpressionObject>(54)
                        .ForMember(x => x.IntValue, m => m.NumberText(4))
                        .ForMember(x => x.NullableIntValue, m => m.NumberText(4))
                        .ForMember(
                            x => x.CustomIntValue,
                            m => m.NumberText(4, "D2").Encoding(Encoding.ASCII).Trim(true).Padding(Padding.Right).Filler((byte)'_').Style(NumberStyles.Any).Provider(CultureInfo.InvariantCulture))
                        .ForMember(x => x.LongValue, m => m.NumberText(6))
                        .ForMember(x => x.NullableLongValue, m => m.NumberText(6))
                        .ForMember(
                            x => x.CustomLongValue,
                            m => m.NumberText(6, "D2").Encoding(Encoding.ASCII).Trim(true).Padding(Padding.Right).Filler((byte)'_').Style(NumberStyles.Any).Provider(CultureInfo.InvariantCulture))
                        .ForMember(x => x.ShortValue, m => m.NumberText(2))
                        .ForMember(x => x.NullableShortValue, m => m.NumberText(2))
                        .ForMember(
                            x => x.CustomShortValue,
                            m => m.NumberText(2, "D1").Encoding(Encoding.ASCII).Trim(true).Padding(Padding.Right).Filler((byte)'_').Style(NumberStyles.Any).Provider(CultureInfo.InvariantCulture))
                        .ForMember(x => x.DecimalValue, m => m.NumberText(6))
                        .ForMember(x => x.NullableDecimalValue, m => m.NumberText(6))
                        .ForMember(
                            x => x.CustomDecimalValue,
                            m => m.NumberText(6, "0.00").Encoding(Encoding.ASCII).Trim(true).Padding(Padding.Right).Filler((byte)'_').Style(NumberStyles.Any).Provider(CultureInfo.InvariantCulture));
                })
                .ToMapperFactory();
            var mapper = mapperFactory.Create<NumberTextExpressionObject>();

            var buffer = new byte[mapper.Size];
            var obj = new NumberTextExpressionObject
            {
                IntValue = 1,
                CustomIntValue = -1,
                LongValue = 1L,
                CustomLongValue = -1L,
                ShortValue = 1,
                CustomShortValue = -1,
                DecimalValue = 12.34m,
                CustomDecimalValue = -1.2m
            };

            // Write
            mapper.ToByte(buffer, 0, obj);
            Assert.Equal(
                Encoding.ASCII.GetBytes(
                    "   1" +
                    "    " +
                    "-01_" +
                    "     1" +
                    "      " +
                    "-01___" +
                    " 1" +
                    "  " +
                    "-1" +
                    " 12.34" +
                    "      " +
                    "-1.20_"),
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
                    " -23.4"),
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
            Assert.Equal(-23.4m, obj.CustomDecimalValue);
        }

        //--------------------------------------------------------------------------------
        // Fix
        //--------------------------------------------------------------------------------

        [Fact]
        public void CoverageFix()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new MapNumberTextTextExpression(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new MapNumberTextTextExpression(-1, null));
            Assert.Throws<ArgumentNullException>(() => new MapNumberTextTextExpression(0, null).Encoding(null));
            Assert.Throws<ArgumentNullException>(() => new MapNumberTextTextExpression(0, null).Provider(null));
        }

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        internal class NumberTextExpressionObject
        {
            // int

            public int IntValue { get; set; }

            public int? NullableIntValue { get; set; }

            public int CustomIntValue { get; set; }

            // long

            public long LongValue { get; set; }

            public long? NullableLongValue { get; set; }

            public long CustomLongValue { get; set; }

            // short

            public short ShortValue { get; set; }

            public short? NullableShortValue { get; set; }

            public short CustomShortValue { get; set; }

            // decimal

            public decimal DecimalValue { get; set; }

            public decimal? NullableDecimalValue { get; set; }

            public decimal CustomDecimalValue { get; set; }
        }
    }
}
