namespace Smart.IO.ByteMapper.Expressions
{
    using Smart.Functional;

    using Xunit;

    public class MapByteExpressionTest
    {
        //--------------------------------------------------------------------------------
        // Expression
        //--------------------------------------------------------------------------------

        [Fact]
        public void MapByBinaryExpression()
        {
            var mapperFactory = new MapperFactoryConfig()
                .DefaultDelimiter(null)
                .Also(config =>
                {
                    config
                        .CreateMapByExpression<ByteExpressionObject>(1)
                        .ForMember(x => x.ByteValue, c => c.Byte());
                })
                .ToMapperFactory();
            var mapper = mapperFactory.Create<ByteExpressionObject>();

            var buffer = new byte[mapper.Size];
            var obj = new ByteExpressionObject
            {
                ByteValue = 1,
            };

            // Write
            mapper.ToByte(buffer, 0, obj);

            Assert.Equal(new byte[] { 0x01 }, buffer);

            // Read
            buffer[0] = 0x02;

            mapper.FromByte(buffer, 0, obj);

            Assert.Equal(2, obj.ByteValue);
        }

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        internal class ByteExpressionObject
        {
            public byte ByteValue { get; set; }
        }
    }
}
