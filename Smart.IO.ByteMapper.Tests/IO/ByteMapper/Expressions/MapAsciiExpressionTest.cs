namespace Smart.IO.ByteMapper.Expressions
{
    using System;
    using System.Text;

    using Smart.Functional;

    using Xunit;

    public class MapAsciiExpressionTest
    {
        //--------------------------------------------------------------------------------
        // Expression
        //--------------------------------------------------------------------------------

        [Fact]
        public void MapByAsciiExpression()
        {
            var mapperFactory = new MapperFactoryConfig()
                .DefaultDelimiter(null)
                .DefaultTrim(true)
                .DefaultTextPadding(Padding.Right)
                .DefaultTextFiller(0x20)
                .Also(config =>
                {
                    config
                        .CreateMapByExpression<AsciiExpressionObject>(8)
                        .ForMember(
                            x => x.StringValue,
                            m => m.Ascii(4))
                        .ForMember(
                            x => x.CustomStringValue,
                            m => m.Ascii(4).Trim(false).Padding(Padding.Left).Filler((byte)'_'));
                })
                .ToMapperFactory();
            var mapper = mapperFactory.Create<AsciiExpressionObject>();

            var buffer = new byte[mapper.Size];
            var obj = new AsciiExpressionObject();

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
            Assert.Throws<ArgumentOutOfRangeException>(() => new MapAsciiExpression(-1));
        }

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        internal class AsciiExpressionObject
        {
            public string StringValue { get; set; }

            public string CustomStringValue { get; set; }
        }
    }
}
