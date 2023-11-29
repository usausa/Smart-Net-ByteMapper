namespace Smart.IO.ByteMapper.Expressions;

using Xunit;

public class MapByteExpressionTest
{
    //--------------------------------------------------------------------------------
    // Expression
    //--------------------------------------------------------------------------------

    [Fact]
    public void MapByByteExpression()
    {
        var mapperFactory = new MapperFactoryConfig()
            .DefaultDelimiter(null)
            .CreateMapByExpression<ByteExpressionObject>(1, config => config
                .ForMember(x => x.ByteValue, m => m.Byte()))
            .ToMapperFactory();
        var mapper = mapperFactory.Create<ByteExpressionObject>();

        var buffer = new byte[mapper.Size];
        var obj = new ByteExpressionObject
        {
            ByteValue = 1
        };

        // Write
        mapper.ToByte(buffer, 0, obj);

        Assert.Equal([0x01], buffer);

        // Read
        buffer[0] = 0x02;

        mapper.FromByte(buffer, 0, obj);

        Assert.Equal(2, obj.ByteValue);
    }

    //--------------------------------------------------------------------------------
    // Helper
    //--------------------------------------------------------------------------------

    internal sealed class ByteExpressionObject
    {
        public byte ByteValue { get; set; }
    }
}
