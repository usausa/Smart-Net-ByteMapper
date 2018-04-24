namespace Smart.IO.ByteMapper.Expressions
{
    using System;
    using System.Text;

    using Xunit;

    public class MapDecimalExpressionTest
    {
        //--------------------------------------------------------------------------------
        // Expression
        //--------------------------------------------------------------------------------

        [Fact]
        public void MapByDecimalExpression()
        {
            var mapperFactory = new MapperFactoryConfig()
                .UseOptionsDefault()
                .DefaultDelimiter(null)
                .DefaultZeroFill(false)
                .DefaultNumberPadding(Padding.Left)
                .DefaultNumberFiller(0x20)
                .CreateMapByExpression<DecimalExpressionObject>(28, config => config
                    .ForMember(x => x.DecimalValue, m => m.Decimal(10, 2).UseGrouping(true).GroupingSize(3))
                    .ForMember(x => x.NullableDecimalValue, m => m.Decimal(6).Filler((byte)'_'))
                    .ForMember(x => x.PaddingRightDecimalValue, m => m.Decimal(6).Padding(Padding.Right))
                    .ForMember(x => x.ZeroFillDecimalValue, m => m.Decimal(6).ZeroFill(true)))
                .ToMapperFactory();
            var mapper = mapperFactory.Create<DecimalExpressionObject>();

            var buffer = new byte[mapper.Size];
            var obj = new DecimalExpressionObject
            {
                DecimalValue = 1234.5m,
                PaddingRightDecimalValue = -1m,
                ZeroFillDecimalValue = 1m
            };

            // Write
            mapper.ToByte(buffer, 0, obj);
            Assert.Equal(
                Encoding.ASCII.GetBytes(
                    "  1,234.50" +
                    "______" +
                    "-1    " +
                    "000001"),
                buffer);

            // Read
            mapper.FromByte(
                Encoding.ASCII.GetBytes(
                    "   2,345.6" +
                    "_____2" +
                    "-2    " +
                    "000002"),
                obj);

            Assert.Equal(2345.60m, obj.DecimalValue);
            Assert.Equal(2m, obj.NullableDecimalValue);
            Assert.Equal(-2m, obj.PaddingRightDecimalValue);
            Assert.Equal(2m, obj.ZeroFillDecimalValue);

            mapper.FromByte(
                Encoding.ASCII.GetBytes(
                    "          " +
                    "______" +
                    "      " +
                    "000000"),
                obj);

            Assert.Equal(0m, obj.DecimalValue);
            Assert.Null(obj.NullableDecimalValue);
            Assert.Equal(0m, obj.PaddingRightDecimalValue);
            Assert.Equal(0m, obj.ZeroFillDecimalValue);
        }

        //--------------------------------------------------------------------------------
        // Fix
        //--------------------------------------------------------------------------------

        [Fact]
        public void CoverageFix()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new MapDecimalExpression(-1));
            Assert.Throws<ArgumentException>(() => new MapDecimalExpression(0, 1));
        }

        //--------------------------------------------------------------------------------
        // Expression
        //--------------------------------------------------------------------------------

        internal class DecimalExpressionObject
        {
            public decimal DecimalValue { get; set; }

            public decimal? NullableDecimalValue { get; set; }

            public decimal PaddingRightDecimalValue { get; set; }

            public decimal ZeroFillDecimalValue { get; set; }
        }
    }
}
