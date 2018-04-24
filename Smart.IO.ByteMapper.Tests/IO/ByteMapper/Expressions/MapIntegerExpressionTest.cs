namespace Smart.IO.ByteMapper.Expressions
{
    using System;
    using System.Text;

    using Xunit;

    public class MapIntegerExpressionTest
    {
        //--------------------------------------------------------------------------------
        // Expression
        //--------------------------------------------------------------------------------

        [Fact]
        public void MapByIntegerExpression()
        {
            var mapperFactory = new MapperFactoryConfig()
                .UseOptionsDefault()
                .DefaultDelimiter(null)
                .DefaultZeroFill(false)
                .DefaultNumberPadding(Padding.Left)
                .DefaultNumberFiller(0x20)
                .CreateMapByExpression<IntegerExpressionObject>(48, config => config
                    .ForMember(x => x.IntValue, m => m.Integer(4))
                    .ForMember(x => x.NullableIntValue, m => m.Integer(4).Filler((byte)'_'))
                    .ForMember(x => x.PaddingRightIntValue, m => m.Integer(4).Padding(Padding.Right))
                    .ForMember(x => x.ZeroFillIntValue, m => m.Integer(4).ZeroFill(true))
                    .ForMember(x => x.LongValue, m => m.Integer(6))
                    .ForMember(x => x.NullableLongValue, m => m.Integer(6).Filler((byte)'_'))
                    .ForMember(x => x.PaddingRightLongValue, m => m.Integer(6).Padding(Padding.Right))
                    .ForMember(x => x.ZeroFillLongValue, m => m.Integer(6).ZeroFill(true))
                    .ForMember(x => x.ShortValue, m => m.Integer(2))
                    .ForMember(x => x.NullableShortValue, m => m.Integer(2).Filler((byte)'_'))
                    .ForMember(x => x.PaddingRightShortValue, m => m.Integer(2).Padding(Padding.Right))
                    .ForMember(x => x.ZeroFillShortValue, m => m.Integer(2).ZeroFill(true)))
                .ToMapperFactory();
            var mapper = mapperFactory.Create<IntegerExpressionObject>();

            var buffer = new byte[mapper.Size];
            var obj = new IntegerExpressionObject
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

            mapper.FromByte(
                Encoding.ASCII.GetBytes(
                    "    " +
                    "____" +
                    "    " +
                    "0000" +
                    "      " +
                    "______" +
                    "      " +
                    "000000" +
                    "  " +
                    "__" +
                    "  " +
                    "00"),
                obj);

            Assert.Equal(0, obj.IntValue);
            Assert.Null(obj.NullableIntValue);
            Assert.Equal(0, obj.PaddingRightIntValue);
            Assert.Equal(0, obj.ZeroFillIntValue);
            Assert.Equal(0L, obj.LongValue);
            Assert.Null(obj.NullableLongValue);
            Assert.Equal(0L, obj.PaddingRightLongValue);
            Assert.Equal(0L, obj.ZeroFillLongValue);
            Assert.Equal((short)0, obj.ShortValue);
            Assert.Null(obj.NullableShortValue);
            Assert.Equal(0, obj.PaddingRightShortValue);
            Assert.Equal((short)0, obj.ZeroFillShortValue);
        }

        //--------------------------------------------------------------------------------
        // Fix
        //--------------------------------------------------------------------------------

        [Fact]
        public void CoverageFix()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new MapIntegerExpression(-1));
        }

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        internal class IntegerExpressionObject
        {
            // int

            public int IntValue { get; set; }

            public int? NullableIntValue { get; set; }

            public int PaddingRightIntValue { get; set; }

            public int ZeroFillIntValue { get; set; }

            // long

            public long LongValue { get; set; }

            public long? NullableLongValue { get; set; }

            public long PaddingRightLongValue { get; set; }

            public long ZeroFillLongValue { get; set; }

            // short

            public short ShortValue { get; set; }

            public short? NullableShortValue { get; set; }

            public short PaddingRightShortValue { get; set; }

            public short ZeroFillShortValue { get; set; }
        }
    }
}
