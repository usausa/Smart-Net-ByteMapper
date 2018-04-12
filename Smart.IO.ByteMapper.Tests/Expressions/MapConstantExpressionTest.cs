namespace Smart.IO.ByteMapper.Expressions
{
    using System;
    using System.Text;

    using Smart.Functional;

    using Xunit;

    public class MapConstantExpressionTest
    {
        //--------------------------------------------------------------------------------
        // Expression
        //--------------------------------------------------------------------------------

        [Fact]
        public void MapByConstantExpression()
        {
            var mapperFactory = new MapperFactoryConfig()
                .DefaultDelimiter(0x0D, 0x0A)
                .Also(config =>
                {
                    config
                        .CreateMapByExpression<ConstExpressionObject>(6)
                        .UseDelimitter(true)
                        .Constant(0, new byte[] { 0x31, 0x32 })
                        .Constant(new byte[] { 0x33, 0x34 });
                })
                .ToMapperFactory();
            var mapper = mapperFactory.Create<ConstExpressionObject>();

            var buffer = new byte[mapper.Size];
            var obj = new ConstExpressionObject();

            // Write
            mapper.ToByte(buffer, 0, obj);

            Assert.Equal(Encoding.ASCII.GetBytes("1234\r\n"), buffer);
        }

        //--------------------------------------------------------------------------------
        // Fix
        //--------------------------------------------------------------------------------

        [Fact]
        public void CoverageFix()
        {
            Assert.Throws<ArgumentNullException>(() => new MapConstantExpression(null));
        }

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        internal class ConstExpressionObject
        {
        }
    }
}
