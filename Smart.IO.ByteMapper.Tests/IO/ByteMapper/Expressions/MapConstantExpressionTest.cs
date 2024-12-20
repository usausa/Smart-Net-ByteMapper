// ReSharper disable UseUtf8StringLiteral
namespace Smart.IO.ByteMapper.Expressions;

#pragma warning disable IDE0320
public sealed class MapConstantExpressionTest
{
    //--------------------------------------------------------------------------------
    // Expression
    //--------------------------------------------------------------------------------

    [Fact]
    public void MapByConstantExpression()
    {
#pragma warning disable IDE0230
        var mapperFactory = new MapperFactoryConfig()
            .DefaultDelimiter(0x0D, 0x0A)
            .CreateMapByExpression<ConstExpressionObject>(6, config => config
                .UseDelimiter(true)
                .Constant(0, [0x31, 0x32])
                .Constant([0x33, 0x34]))
            .ToMapperFactory();
#pragma warning restore IDE0230
        var mapper = mapperFactory.Create<ConstExpressionObject>();

        var buffer = new byte[mapper.Size];
        var obj = new ConstExpressionObject();

        // Write
        mapper.ToByte(buffer, 0, obj);

        Assert.Equal("1234\r\n"u8.ToArray(), buffer);
    }

    //--------------------------------------------------------------------------------
    // Helper
    //--------------------------------------------------------------------------------

    internal sealed class ConstExpressionObject
    {
    }
}
