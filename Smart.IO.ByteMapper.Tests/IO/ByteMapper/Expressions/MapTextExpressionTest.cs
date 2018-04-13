namespace Smart.IO.ByteMapper.Expressions
{
    using System;
    using System.Text;

    using Smart.Functional;

    using Xunit;

    public class MapTextExpressionTest
    {
        //--------------------------------------------------------------------------------
        // Expression
        //--------------------------------------------------------------------------------

        [Fact]
        public void MapByTextExpression()
        {
            var mapperFactory = new MapperFactoryConfig()
                .DefaultDelimiter(null)
                .DefaultEncoding(Encoding.ASCII)
                .DefaultTrim(true)
                .DefaultTextPadding(Padding.Right)
                .DefaultTextFiller(0x20)
                .Also(config =>
                {
                    config
                        .CreateMapByExpression<TextExpressionObject>(8)
                        .ForMember(
                            x => x.StringValue,
                            m => m.Text(4))
                        .ForMember(
                            x => x.CustomStringValue,
                            m => m.Text(4).Encoding(Encoding.ASCII).Trim(false).Padding(Padding.Left).Filler((byte)'_'));
                })
                .ToMapperFactory();
            var mapper = mapperFactory.Create<TextExpressionObject>();

            var buffer = new byte[mapper.Size];
            var obj = new TextExpressionObject();

            // Write
            mapper.ToByte(buffer, 0, obj);

            Assert.Equal(Encoding.ASCII.GetBytes("    ____"), buffer);

            // Read
            mapper.FromByte(Encoding.ASCII.GetBytes("12  __AB"), 0, obj);

            Assert.Equal("12", obj.StringValue);
            Assert.Equal("__AB", obj.CustomStringValue);
        }

        //--------------------------------------------------------------------------------
        // Fix
        //--------------------------------------------------------------------------------

        [Fact]
        public void CoverageFix()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new MapTextExpression(-1));
            Assert.Throws<ArgumentNullException>(() => new MapTextExpression(0).Encoding(null));
        }

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        internal class TextExpressionObject
        {
            public string StringValue { get; set; }

            public string CustomStringValue { get; set; }
        }
    }
}
